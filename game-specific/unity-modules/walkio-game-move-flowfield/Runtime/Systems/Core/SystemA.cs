namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    [DisableAutoCreation]
    [UpdateAfter(typeof(CheckTargetAtTileChangeSystem))]
    public class SystemA : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemA));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        //
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif
        public IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

        public void Construct()
        {
            _logger.Debug($"Module - SystemA - Construct");
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        private void UpdateEachChaseTarget(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int forWhichGroupId, float3 changeToPosition)
        {
            using (var hashTable = new NativeHashMap<int2, int>(100, Allocator.TempJob))
            {
                Entities
                    .WithAll<ChaseTarget>()
                    .ForEach((Entity entity, ChaseTargetProperty chaseTargetProperty, LocalToWorld localToWorld) =>
                    {
                        var matchGroupId = (chaseTargetProperty.BelongToGroup == forWhichGroupId);
                        if (matchGroupId)
                        {
                            var atTileIndex =
                                Utility.FlowFieldTileHelper.PositionToTileIndexAtGrid2D(
                                    gridCellCount, gridCellSize,
                                    tileCellCount, tileCellSize,
                                    new float2(localToWorld.Position.x, localToWorld.Position.z));
                            _logger.Debug($"Module - SystemA - UpdateEachChaseTarget - chase target entity: {entity} atTileIndex: {atTileIndex}");

                            var count = 0;
                            var hasKey = hashTable.TryGetValue(atTileIndex, out count);
                            if (!hasKey)
                            {
                                hashTable.Add(atTileIndex, 1);
                            }
                        }
                    })
                    .WithoutBurst()
                    .Run();

                using (var tileIndices = hashTable.GetKeyArray(Allocator.TempJob))
                {
                    var positions = new List<float3>();
                    for (var i = 0; i < tileIndices.Length; ++i)
                    {
                        var tileIndex = tileIndices[i];
                        var positionXZ =
                            Utility.FlowFieldTileHelper.TileIndexToPosition2D(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                tileIndex);

                        var position = new float3(positionXZ.x, 0, positionXZ.y);

                        // _logger.Debug($"Module - SystemA - UpdateEachChaseTarget - tile center position {i}: {position}");

                        positions.Add(position);
                    }

                    // var positions = tileIndices.ToList()
                    FlowFieldWorldProvider?.CalculateLeadingTilePath(forWhichGroupId, changeToPosition, positions);
                }
            }
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var gridCellCount = new int2(256, 192);
            var gridCellSize = new float2(1.0f, 1.0f);
            var flowFieldWorldData = FlowFieldWorldProvider.FlowFieldWorldData as Template.FlowFieldWorldData;
            var tileCellCount = new int2(flowFieldWorldData.tileCellCount.x, flowFieldWorldData.tileCellCount.y);
            var tileCellSize = (float2)flowFieldWorldData.tileCellSize;

            Entities
                .WithAll<AtTileChange>()
                .ForEach((Entity entity, AtTileChangeProperty atTileChangeProperty) =>
                {
                    _logger.Debug($"Module - SsytemA - OnUpdate - event entity: {entity}");

                    UpdateEachChaseTarget(
                        gridCellCount, gridCellSize,
                        tileCellCount, tileCellSize,
                        atTileChangeProperty.GroupId, atTileChangeProperty.ChangeToPosition);

                    // Destroy the event so it won't be processed again
                    commandBuffer.DestroyEntity(entity);
                })
                .WithoutBurst()
                .Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
