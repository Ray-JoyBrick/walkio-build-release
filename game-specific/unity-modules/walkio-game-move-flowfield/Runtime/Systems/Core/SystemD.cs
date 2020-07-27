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

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    using GameLevel = JoyBrick.Walkio.Game.Level;

    [DisableAutoCreation]
    [UpdateAfter(typeof(CheckTargetAtTileChangeSystem))]
    public class SystemD : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemD));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _gridWorldEntityQuery;

        //
        private bool _canUpdate;

        //
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif
        public IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

        public void Construct()
        {
            _logger.Debug($"Module - SystemD - Construct");

#if WALKIO_FLOWCONTROL
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - FlowField - SystemD - Construct - Receive AllDoneSettingAsset");
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

        private void CreateTileFromEachChaseTarget(
            EntityCommandBuffer commandBuffer,
            EntityArchetype flowFieldTileEntityArchetype,
            EntityArchetype leadingToSetPlaceholderEntityArchetype,
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int forWhichGroupId, float3 changeToPosition)
        {
            using (var tileHashTable = new NativeHashMap<int2, Entity>(100, Allocator.TempJob))
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
                            _logger.Debug($"Module - SystemD - UpdateEachChaseTarget - chase target entity: {entity} atTileIndex: {atTileIndex}");

                            // var buffer = new DynamicBuffer<ChaseTargetAtTileBuffer>();
                            var outEntity = Entity.Null;
                            var hasKey = tileHashTable.TryGetValue(atTileIndex, out outEntity);
                            if (!hasKey)
                            {
                                _logger.Debug($"Module - SystemD - UpdateEachChaseTarget - no key, adding");
                                var flowFieldTileEntity = commandBuffer.CreateEntity(flowFieldTileEntityArchetype);
                                commandBuffer.SetComponent(flowFieldTileEntity, new FlowFieldTileProperty
                                {
                                    TileIndex = atTileIndex
                                });
                                var chaseTargetAtTileBuffer = commandBuffer.AddBuffer<ChaseTargetAtTileBuffer>(flowFieldTileEntity);
                                // var bufferCount = 20;
                                // chaseTargetAtTileBuffer.ResizeUninitialized(bufferCount);
                                chaseTargetAtTileBuffer.EnsureCapacity(20);

                                tileHashTable.Add(atTileIndex, flowFieldTileEntity);
                            }

                            hasKey = tileHashTable.TryGetValue(atTileIndex, out outEntity);
                            _logger.Debug($"Module - SystemD - UpdateEachChaseTarget - append to buffer");
                            commandBuffer.AppendToBuffer<ChaseTargetAtTileBuffer>(outEntity, entity);
                            // buffer
                            //
                            // buffer.Add(entity)

                        }
                    })
                    .WithoutBurst()
                    .Run();

                var leadingToSetPlaceholderEntity = commandBuffer.CreateEntity(leadingToSetPlaceholderEntityArchetype);
                using (var tileIndices = tileHashTable.GetKeyArray(Allocator.TempJob))
                {
                    commandBuffer.SetComponent(leadingToSetPlaceholderEntity, new LeadingToSetPlaceholderProperty
                    {
                        GroupId = forWhichGroupId,
                        ChangeToPosition = changeToPosition
                    });

                    using (var tileEntities = tileHashTable.GetValueArray(Allocator.TempJob))
                    {
                        var leadingToSetPlaceholderTileBuffer = commandBuffer.AddBuffer<LeadingToSetPlaceholderTileBuffer>(leadingToSetPlaceholderEntity);

                        leadingToSetPlaceholderTileBuffer.ResizeUninitialized(tileEntities.Length);

                        for (var i = 0; i < tileEntities.Length; ++i)
                        {
                            leadingToSetPlaceholderTileBuffer[i] = tileEntities[i];
                        }
                    }
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

            var flowFieldTileEntityArchetype = EntityManager.CreateArchetype(
                typeof(FlowFieldTile),
                typeof(FlowFieldTileProperty),
                typeof(FlowFieldTileCellBuffer),
                typeof(ChaseTargetAtTileBuffer));

            var leadingToSetPlaceholderEntityArchetype = EntityManager.CreateArchetype(
                typeof(LeadingToSetPlaceholder),
                typeof(LeadingToSetPlaceholderProperty),
                typeof(LeadingToSetPlaceholderTileBuffer));

            Entities
                .WithAll<AtTileChange>()
                .ForEach((Entity entity, AtTileChangeProperty atTileChangeProperty) =>
                {
                    _logger.Debug($"Module - SystemD - OnUpdate - event entity: {entity}");

                    _logger.Debug($"Module - SystemD - OnUpdate - destroy event entity");
                    // Destroy the event so it won't be processed again
                    commandBuffer.DestroyEntity(entity);

                    CreateTileFromEachChaseTarget(
                        commandBuffer,
                        flowFieldTileEntityArchetype,
                        leadingToSetPlaceholderEntityArchetype,
                        gridCellCount, gridCellSize,
                        tileCellCount, tileCellSize,
                        atTileChangeProperty.GroupId, atTileChangeProperty.ChangeToPosition);

                    // _logger.Debug($"Module - SystemD - OnUpdate - destroy event entity");
                    // // Destroy the event so it won't be processed again
                    // commandBuffer.DestroyEntity(entity);
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
