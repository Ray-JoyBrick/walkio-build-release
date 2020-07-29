namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
    using GameLevel = JoyBrick.Walkio.Game.Level;

    [DisableAutoCreation]
    // [UpdateAfter(typeof(CheckTargetAtTileChangeSystem))]
    public class SystemM02 : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemM02));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _gridWorldEntityQuery;

        //
        private bool _canUpdate;

        //
        public GameFlowControl.IFlowControl FlowControl { get; set; }
        public IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

        public void Construct()
        {
            _logger.Debug($"Module - SystemA - Construct");

#if WALKIO_FLOWCONTROL
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - FlowField - SystemM02 - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            _gridWorldEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<GameLevel.GridWorld>(),
                    ComponentType.ReadOnly<GameLevel.GridWorldProperty>()
                }
            });

            RequireForUpdate(_gridWorldEntityQuery);
        }

        private void UpdateEachChaseTarget(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int forWhichGroupId, float3 changeToPosition,
            Entity forWhichLeaderEntity)
        {
            using (var tileHashTable = new NativeHashMap<int2, int>(100, Allocator.TempJob))
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
                            // _logger.Debug($"Module - SystemM02 - UpdateEachChaseTarget - chase target entity: {entity} atTileIndex: {atTileIndex}");

                            var count = 0;
                            var hasKey = tileHashTable.TryGetValue(atTileIndex, out count);
                            if (!hasKey)
                            {
                                tileHashTable.Add(atTileIndex, 1);
                            }
                        }
                    })
                    .WithoutBurst()
                    .Run();

                using (var tileIndices = tileHashTable.GetKeyArray(Allocator.TempJob))
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
                    FlowFieldWorldProvider?.CalculateLeadingTilePath(forWhichGroupId, forWhichLeaderEntity, changeToPosition, positions);
                }
            }
        }

        protected override void OnUpdate()
        {
            // if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var gridWorldProperty = _gridWorldEntityQuery.GetSingleton<GameLevel.GridWorldProperty>();

            var gridCellCount = gridWorldProperty.CellCount;
            var gridCellSize = gridWorldProperty.CellSize;
            var flowFieldWorldData = FlowFieldWorldProvider.FlowFieldWorldData as Template.FlowFieldWorldData;
            var tileCellCount = new int2(flowFieldWorldData.tileCellCount.x, flowFieldWorldData.tileCellCount.y);
            var tileCellSize = (float2)flowFieldWorldData.tileCellSize;

            Entities
                .WithAll<AtTileChange>()
                .ForEach((Entity entity, AtTileChangeProperty atTileChangeProperty) =>
                // .ForEach((Entity entity, int entityInQueryIndex, AtTileChangeProperty atTileChangeProperty) =>
                {
                    // _logger.Debug($"Module - SystemM02 - OnUpdate - event entity: {entity}");

                    UpdateEachChaseTarget(
                        gridCellCount, gridCellSize,
                        tileCellCount, tileCellSize,
                        atTileChangeProperty.GroupId, atTileChangeProperty.ChangeToPosition,
                        atTileChangeProperty.ForWhichLeader);

                    // Destroy the event so it won't be processed again
                    commandBuffer.DestroyEntity(entity);
                    // concurrentCommandBuffer.DestroyEntity(entityInQueryIndex, entity);
                })
                .WithoutBurst()
                .Run();
                // .WithBurst()
                // .Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
