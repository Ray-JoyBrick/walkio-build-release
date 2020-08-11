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
    // [UpdateAfter(typeof(SystemA))]
    public class SystemM03 : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemM03));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _gridWorldEntityQuery;

#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif
        public IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

        //
        private bool _canUpdate;

        public void Construct()
        {
            _logger.Debug($"Module - Move - FlowField - SystemM03 - Construct");

#if WALKIO_FLOWCONTROL
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - FlowField - SystemM03 - Construct - Receive FlowReadyToStart");
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

        private static int GetBaseCostByGridCellIndex(int gridCellIndex)
        {
            return 0;
        }

        private static Entity GetAssignedFlowFieldTileEntity(int2 gridCellCount, float2 gridCellSize, int2 tileCellCount,
            float2 tileCellSize, EntityCommandBuffer commandBuffer, int groupId, EntityArchetype flowFieldTileEntityArchetype,
            GameLevel.GridWorldProperty gridWorldProperty, int2 outTileIndex, int2 outTileCellIndex, int2 inTileIndex,
            out NativeArray<int> baseCosts)
        {
            var flowFieldTileEntity = commandBuffer.CreateEntity(flowFieldTileEntityArchetype);

            commandBuffer.SetComponent(flowFieldTileEntity, new FlowFieldTileProperty
            {
                WorldId = 0,
                GroupId = groupId,
                TileIndex = outTileIndex
            });

            var gridCellIndices =
                Utility.FlowFieldTileHelper.GetGridCellIndicesInTile(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    outTileIndex);

            var tileCellBuffer = commandBuffer.AddBuffer<FlowFieldTileCellBuffer>(flowFieldTileEntity);
            var totalTileCellCount = tileCellCount.x * tileCellCount.y;
            tileCellBuffer.ResizeUninitialized(totalTileCellCount);

            //
            baseCosts = new NativeArray<int>(totalTileCellCount, Allocator.Temp);

            var gridCellIndicesInOutTile =
                Utility.FlowFieldTileHelper.GetGridCellIndicesInTile(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    outTileIndex);
            for (var g = 0; g < baseCosts.Length; ++g)
            {
                var gridCellIndex = gridCellIndicesInOutTile[g];

                if (gridCellIndex != -1)
                {
                    var gridMapContext = gridWorldProperty.GridMapBlobAssetRef.Value.GridMapContextArray[gridCellIndex];
                    if (gridMapContext == 1)
                    {
                        baseCosts[g] = 50000;
                    }
                }
                else
                {
                    // This might be the case where the tile has some portion outside the boundary
                    baseCosts[g] = 50000;
                    // _logger.Debug($"Module - Move - FlowField - SystemM03 - OnUpdate - gridCellIndex -1 at tileIndex: {outTileIndex} for tileCellIndex: {g}");
                }
            }

            var costs =
                Utility.FlowFieldTileHelper.GetIntegrationCostForTile(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    outTileCellIndex,
                    // inTileCellIndex,
                    baseCosts);
            var neighborDirection = Utility.FlowFieldTileHelper.NeighborTileDirection(outTileIndex, inTileIndex);
            var directions =
                Utility.FlowFieldTileHelper.GetDirectionForTile(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    outTileCellIndex,
                    // inTileCellIndex,
                    neighborDirection,
                    baseCosts);

            for (var m = 0; m < totalTileCellCount; ++m)
            {
                // Assign grid cell index into tile cell for now
                // Can also assign the content of that grid cell into tile cell
                // tileCellBuffer[i] = gridCellIndices[i];
                tileCellBuffer[m] = new TileCellContent
                {
                    CellIndex = gridCellIndices[m],
                    BaseCost = costs[m],
                    Direction = directions[m]
                };
            }

            return flowFieldTileEntity;
        }


        private static void ProceedOneTileOnOnePath(int2 gridCellCount, float2 gridCellSize, int2 tileCellCount,
            float2 tileCellSize, EntityCommandBuffer commandBuffer, int groupId, DynamicBuffer<PathPointSeparationBuffer> pathPointSeparationBuffers,
            DynamicBuffer<PathPointBuffer> pathPointBuffers, EntityArchetype flowFieldTileEntityArchetype, int i, int2 tileCount,
            NativeHashMap<int2, Entity> hashTable,
            int2 atTileIndex,
            Entity atTileEntity,
            Entity leadingToSetEntity,
            GameLevel.GridWorldProperty gridWorldProperty)
        {
            var index = pathPointSeparationBuffers[i];

            var startIndex = index.Value.x;
            var endIndex = index.Value.y;

            var count = (endIndex - startIndex) + 1;

            var k = 0;

            var points = new NativeArray<float2>(count, Allocator.Temp);
            for (var j = startIndex; j <= endIndex; ++j)
            {
                var pathPoint = pathPointBuffers[j];
                points[k] = new float2(pathPoint.Value.x, pathPoint.Value.z);
                ++k;
            }

            k = 0;

            var pathTileInfos =
                Utility.FlowFieldTileHelper.GetTilePairInfoOnPath2DArray(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    points);

            if (points.IsCreated)
            {
                points.Dispose();
            }

            //
            for (var infoIndex = 0; infoIndex < pathTileInfos.Length; ++infoIndex)
            {
                var outTileIndex = pathTileInfos[infoIndex].OutTileIndex;
                var outTileCellIndex = pathTileInfos[infoIndex].OutTileCellIndex;

                var inTileIndex = pathTileInfos[infoIndex].InTileIndex;
                var inTileCellIndex = pathTileInfos[infoIndex].InTileCellIndex;

                _logger.Debug($"Module - Move - FlowField - SystemM03 - ProceedOneTileOnOnePath for length: {pathTileInfos.Length} outtile: {outTileIndex} intile: {inTileIndex}");

                var cahcedEntity = Entity.Null;

                var cached = hashTable.TryGetValue(outTileIndex, out cahcedEntity);
                // var cached = hashTable.TryGetValue(inTileIndex, out cahcedEntity);
                if (cached) continue;

                var outFlowFieldTileEntity = GetAssignedFlowFieldTileEntity(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    commandBuffer,
                    groupId, flowFieldTileEntityArchetype, gridWorldProperty, outTileIndex, outTileCellIndex, inTileIndex, out var baseCostsOut);

                hashTable.Add(outTileIndex, outFlowFieldTileEntity);

                if (baseCostsOut.IsCreated)
                {
                    baseCostsOut.Dispose();
                }
            }
        }


        private void ProcessPathPoint(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            EntityCommandBuffer commandBuffer,
            Entity entity,
            int groupId,
            int2 atTileIndex,
            Entity atTileEntity,
            DynamicBuffer<PathPointSeparationBuffer> pathPointSeparationBuffers, DynamicBuffer<PathPointBuffer> pathPointBuffers,
            EntityArchetype leadingToSetEntityArchetype, EntityArchetype flowFieldTileEntityArchetype,
            EntityArchetype leadingToSetCreatedEventEntityArchetype,
            GameLevel.GridWorldProperty gridWorldProperty)
        {
            var capacity = pathPointBuffers.Length;
            // using (var hashTable = new NativeHashMap<int2, bool>(capacity, Allocator.TempJob))
            using (var hashTable = new NativeHashMap<int2, Entity>(capacity, Allocator.TempJob))
            {
                _logger.Debug($"Module - Move - FlowField - SystemM03 - OnUpdate - event entity: {entity}");

                // var goalTileIndex = new int2(-1, -1);
                var tileCount = Utility.FlowFieldTileHelper.GetTileCountFromGrid2D(
                    gridCellCount, gridCellSize, tileCellCount, tileCellSize);

                var leadingToSetCreatedEventEntity =
                    commandBuffer.CreateEntity(leadingToSetCreatedEventEntityArchetype);

                // var leadingToSetEntity = commandBuffer.CreateEntity(leadingToSetEntityArchetype);

                // var leadingToTileBuffer = commandBuffer.AddBuffer<LeadingToTileBuffer>(leadingToSetEntity);

                commandBuffer.SetComponent(leadingToSetCreatedEventEntity, new LeadingToSetProperty
                {
                    GroupId = groupId,
                    TileIndex = atTileIndex,
                    LeadingToTile = atTileEntity
                });

                var leadingToTileBuffer = commandBuffer.AddBuffer<LeadingToTileBuffer>(leadingToSetCreatedEventEntity);
                leadingToTileBuffer.EnsureCapacity(20);

                // Place atTileEntity into has table first
                hashTable.Add(atTileIndex, atTileEntity);

                //
                for (var i = 0; i < pathPointSeparationBuffers.Length; ++i)
                {
                    ProceedOneTileOnOnePath(gridCellCount, gridCellSize, tileCellCount, tileCellSize, commandBuffer, groupId, pathPointSeparationBuffers, pathPointBuffers, flowFieldTileEntityArchetype, i, tileCount,
                        hashTable,
                        atTileIndex,
                        atTileEntity,
                        leadingToSetCreatedEventEntity,
                        gridWorldProperty);
                        // leadingToSetEntity);
                }

                //
                using (var tileIndices = hashTable.GetKeyArray(Allocator.TempJob))
                using (var entities = hashTable.GetValueArray(Allocator.TempJob))
                {
                    for (var i = 0; i < entities.Length; ++i)
                    {
                        commandBuffer.AppendToBuffer<LeadingToTileBuffer>(
                            // leadingToSetEntity,
                            leadingToSetCreatedEventEntity,
                            new LeadingToTileContent
                            {
                                TileIndex = tileIndices[i],
                                Tile = entities[i]
                            });
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

            var leadingToSetEntityArchetype = EntityManager.CreateArchetype(
                typeof(LeadingToSet),
                typeof(LeadingToSetProperty),
                typeof(LeadingToTileBuffer),
                typeof(GameFlowControl.StageUse));

            var flowFieldTileEntityArchetype = EntityManager.CreateArchetype(
                typeof(FlowFieldTile),
                typeof(FlowFieldTileProperty),
                typeof(FlowFieldTileCellBuffer),
                typeof(GameFlowControl.StageUse));

            var leadingToSetCreatedEventEntityArchetype = EntityManager.CreateArchetype(
                typeof(LeadingToSetCreated),
                typeof(LeadingToSet),
                typeof(LeadingToSetProperty),
                typeof(LeadingToTileBuffer),
                typeof(GameFlowControl.StageUse));

            Entities
                .WithAll<PathPointFound>()
                .ForEach((Entity entity, PathPointFoundProperty pathPointFoundProperty, DynamicBuffer<PathPointSeparationBuffer> pathPointSeparationBuffers, DynamicBuffer<PathPointBuffer> pathPointBuffers) =>
                {
                    ProcessPathPoint(
                        gridCellCount, gridCellSize,
                        tileCellCount, tileCellSize,
                        commandBuffer,
                        entity,
                        pathPointFoundProperty.GroupId,
                        pathPointFoundProperty.AtTileIndex,
                        pathPointFoundProperty.AtTileEntity,
                        pathPointSeparationBuffers, pathPointBuffers,
                        leadingToSetEntityArchetype,
                        flowFieldTileEntityArchetype,
                        leadingToSetCreatedEventEntityArchetype,
                        gridWorldProperty);

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
