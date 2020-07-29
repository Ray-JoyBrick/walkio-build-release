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
    public class SystemM02_2 : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemM02_2));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _gridWorldEntityQuery;

        //
        private bool _canUpdate;
        private EntityQuery _someEntityQuery;

        //
        public GameFlowControl.IFlowControl FlowControl { get; set; }
        public IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

        public void Construct()
        {
            _logger.Debug($"Module - SystemM02_2 - Construct");

#if WALKIO_FLOWCONTROL
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - FlowField - SystemM02_2 - Construct - Receive AllDoneSettingAsset");
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

            _someEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<FlowFieldTileCellBuffer>()
                }
            });

            RequireForUpdate(_gridWorldEntityQuery);
            RequireForUpdate(_someEntityQuery);
        }

        private void UpdateForFlowFieldTile(
            int totalTileCellCount,
            NativeArray<int> gridCellIndices,
            NativeArray<int> costs,
            NativeArray<int> directions,
            Entity flowFieldTileEntity)
        {
            Entities
                .WithAll<FlowFieldTile>()
                .ForEach((Entity entity, ref DynamicBuffer<FlowFieldTileCellBuffer> flowFieldTileCellBuffers) =>
                {
                    if (entity == flowFieldTileEntity)
                    {
                        // _logger.Debug($"Module - SystemM02_2 - UpdateForFlowFieldTile - find matched flow field tile: {entity}");

                        // flowFieldTileCellBuffers.ResizeUninitialized(totalTileCellCount);
                        
                        for (var m = 0; m < totalTileCellCount; ++m)
                        {
                            // Assign grid cell index into tile cell for now
                            // Can also assign the content of that grid cell into tile cell
                            // tileCellBuffer[i] = gridCellIndices[i];
                            flowFieldTileCellBuffers[m] = new TileCellContent
                            {
                                CellIndex = gridCellIndices[m],
                                BaseCost = costs[m],
                                Direction = directions[m]
                            };
                        }
                    }
                })
                .WithoutBurst()
                .Run();            
        }

        private void UpdateEachChaseTarget(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int forWhichGroupId,
            int2 changeToTileCellIndex,
            float3 changeToPosition,
            GameLevel.GridWorldProperty gridWorldProperty)
            // Entity forWhichLeaderEntity)
        {
            Entities
                .WithAll<LeadingToSet>()
                .WithNone<ToBeDeletedLeadingToSet>()
                .ForEach((Entity entity, LeadingToSetProperty leadingToSetProperty, DynamicBuffer<LeadingToTileBuffer> leadingToTileBuffers) =>
                {
                    var matchGroupId = (leadingToSetProperty.GroupId == forWhichGroupId);
                    if (matchGroupId)
                    {
                        _logger.Debug($"Module - SystemM02_2 - UpdateEachChaseTarget - find matched group: {forWhichGroupId} for tile cell change");
                        
                        var gridCellIndices =
                            Utility.FlowFieldTileHelper.GetGridCellIndicesInTile(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                // inTileIndex);
                                leadingToSetProperty.TileIndex);                        
                        
                        var totalTileCellCount = tileCellCount.x * tileCellCount.y;
                        var baseCosts = new NativeArray<int>(totalTileCellCount, Allocator.Temp);
                        
                        var gridCellIndicesInOutTile =
                            Utility.FlowFieldTileHelper.GetGridCellIndicesInTile(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                leadingToSetProperty.TileIndex);
                        for (var g = 0; g < baseCosts.Length; ++g)
                        {
                            var gridCellIndex = gridCellIndicesInOutTile[g];
                        
                            if (gridCellIndex != -1)
                            {
                                var gridMapContext = gridWorldProperty.GridMapBlobAssetRef.Value.GridMapContextArray[gridCellIndex];
                                if (gridMapContext == 1)
                                {
                                    baseCosts[g] = 1000;
                                }
                            }
                            else
                            {
                                _logger.Debug($"Module - Move - FlowField - SystemM02_2 - OnUpdate - gridCellIndex -1 at tileIndex: {leadingToSetProperty.TileIndex} for tileCellIndex: {g}");
                            }
                        }
                        
                        var costs =
                            Utility.FlowFieldTileHelper.GetIntegrationCostForTile(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                changeToTileCellIndex,
                                // inTileCellIndex,
                                baseCosts);
                        // var neighborDirection = Utility.FlowFieldTileHelper.NeighborTileDirection(outTileIndex, inTileIndex);
                        var neighborDirection = 4;
                        var directions =
                            Utility.FlowFieldTileHelper.GetDirectionForTile(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                changeToTileCellIndex,
                                // inTileCellIndex,
                                neighborDirection,
                                baseCosts);

                        // _logger.Debug($"Module - SystemM02_2 - UpdateEachChaseTarget - change to tile cell index: {changeToTileCellIndex}");
                        
                        UpdateForFlowFieldTile(totalTileCellCount, gridCellIndices, costs, directions, leadingToSetProperty.LeadingToTile);
                    }
                })
                .WithoutBurst()
                .Run();
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
                .WithAll<AtTileCellChange>()
                .ForEach((Entity entity, AtTileCellChangeProperty atTileCellChangeProperty) =>
                // .ForEach((Entity entity, int entityInQueryIndex, AtTileChangeProperty atTileChangeProperty) =>
                {
                    _logger.Debug($"Module - SystemM02_2 - OnUpdate - receive AtTileCellChange event entity: {entity} to tileCellIndex: {atTileCellChangeProperty.ChangeToTileCellIndex}");

                    UpdateEachChaseTarget(
                        gridCellCount, gridCellSize,
                        tileCellCount, tileCellSize,
                        atTileCellChangeProperty.GroupId,
                        atTileCellChangeProperty.ChangeToTileCellIndex,
                        atTileCellChangeProperty.ChangeToPosition,
                        gridWorldProperty);
                        // atTileCellChangeProperty.ForWhichLeader);

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
