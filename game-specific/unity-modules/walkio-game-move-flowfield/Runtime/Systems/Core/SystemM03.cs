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
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemB));

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
                    _logger.Debug($"Module - Move - FlowField - SystemM03 - Construct - Receive AllDoneSettingAsset");
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

        private void ProcessPathPoint(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            EntityCommandBuffer commandBuffer,
            Entity entity,
            int groupId,
            DynamicBuffer<PathPointSeparationBuffer> pathPointSeparationBuffers, DynamicBuffer<PathPointBuffer> pathPointBuffers,
            EntityArchetype leadingToSetEntityArchetype, EntityArchetype flowFieldTileEntityArchetype)
        {
            var capacity = pathPointBuffers.Length;
            // using (var hashTable = new NativeHashMap<int2, bool>(capacity, Allocator.TempJob))
            using (var hashTable = new NativeHashMap<int2, Entity>(capacity, Allocator.TempJob))
            {
                _logger.Debug($"Module - Move - FlowField - SystemM03 - OnUpdate - event entity: {entity}");

                // var goalTileIndex = new int2(-1, -1);
                var tileCount = Utility.FlowFieldTileHelper.GetTileCountFromGrid2D(
                    gridCellCount, gridCellSize, tileCellCount, tileCellSize);

                var leadingToSetEntity = commandBuffer.CreateEntity(leadingToSetEntityArchetype);

                var leadingToTileBuffer = commandBuffer.AddBuffer<LeadingToTileBuffer>(leadingToSetEntity);
                leadingToTileBuffer.EnsureCapacity(20);

                for (var i = 0; i < pathPointSeparationBuffers.Length; ++i)
                {
                    var index = pathPointSeparationBuffers[i];

                    var startIndex = index.Value.x;
                    var endIndex = index.Value.y;

                    var count = endIndex - startIndex;

                    var reversePoints = new NativeArray<float2>(count, Allocator.Temp);
                    var k = 0;
                    for (var j = endIndex - 1; j > startIndex; --j)
                    {
                        var pathPoint = pathPointBuffers[j];
                        reversePoints[k] = new float2(pathPoint.Value.x, pathPoint.Value.z);
                        ++k;
                    }

                    // for (var j = startIndex; j > endIndex; ++j)
                    // {
                    //     var pathPoint = pathPointBuffers[j];
                    //     reversePoints[k] = new float2(pathPoint.Value.x, pathPoint.Value.z);
                    //     ++k;
                    // }

                    var pathTileInfos =
                        Utility.FlowFieldTileHelper.GetTilePairInfoOnPath(
                            gridCellCount, gridCellSize,
                            tileCellCount, tileCellSize,
                            reversePoints);

                    for (var infoIndex = 0; infoIndex < pathTileInfos.Length; ++infoIndex)
                    {
                        // var inTileIndex = pathTileInfos[infoIndex].InTileIndex;
                        var inTileIndex = new int2(
                            pathTileInfos[infoIndex].InTileIndex % tileCount.x,
                            pathTileInfos[infoIndex].InTileIndex / tileCount.x);
                        var inTileCellIndex = new int2(
                            pathTileInfos[infoIndex].InTileCellIndex % tileCellCount.x,
                            pathTileInfos[infoIndex].InTileCellIndex / tileCellCount.x);

                        var outTileIndex = new int2(
                            pathTileInfos[infoIndex].OutTileIndex % tileCount.x,
                            pathTileInfos[infoIndex].OutTileIndex / tileCount.x);
                        var outTileCellIndex = new int2(
                            pathTileInfos[infoIndex].OutTileCellIndex % tileCellCount.x,
                            pathTileInfos[infoIndex].OutTileCellIndex / tileCellCount.x);

                        var cahcedEntity = Entity.Null;

                        // var cached = hashTable.TryGetValue(outTileIndex, out cahcedEntity);
                        var cached = hashTable.TryGetValue(inTileIndex, out cahcedEntity);
                        if (cached) continue;

                        var flowFieldTileEntity = commandBuffer.CreateEntity(flowFieldTileEntityArchetype);

                        commandBuffer.SetComponent(flowFieldTileEntity, new FlowFieldTileProperty
                        {
                            WorldId = 0,
                            GroupId = groupId,
                            // TileIndex = outTileIndex
                            TileIndex = inTileIndex
                        });

                        var gridCellIndices =
                            Utility.FlowFieldTileHelper.GetGridCellIndicesInTile(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                inTileIndex);
                                // outTileIndex);

                        var tileCellBuffer = commandBuffer.AddBuffer<FlowFieldTileCellBuffer>(flowFieldTileEntity);
                        var totalTileCellCount = tileCellCount.x * tileCellCount.y;
                        tileCellBuffer.ResizeUninitialized(totalTileCellCount);

                        var baseCosts = new NativeArray<int>(totalTileCellCount, Allocator.Temp);

                        var costs =
                            Utility.FlowFieldTileHelper.GetIntegrationCostForTile(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                // outTileCellIndex,
                                inTileCellIndex,
                                baseCosts);
                        var directions =
                            Utility.FlowFieldTileHelper.GetDirectionForTile(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                // outTileCellIndex,
                                inTileCellIndex,
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

                        commandBuffer.AppendToBuffer<LeadingToTileBuffer>(
                            leadingToSetEntity,
                            new LeadingToTileContent
                            {
                                // TileIndex = outTileIndex,
                                TileIndex = inTileIndex,
                                Tile = flowFieldTileEntity
                            });

                        // hashTable.Add(outTileIndex, flowFieldTileEntity);
                        hashTable.Add(inTileIndex, flowFieldTileEntity);
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
                typeof(LeadingToTileBuffer));

            var flowFieldTileEntityArchetype = EntityManager.CreateArchetype(
                typeof(FlowFieldTile),
                typeof(FlowFieldTileProperty),
                typeof(FlowFieldTileCellBuffer));

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
                        pathPointSeparationBuffers, pathPointBuffers,
                        leadingToSetEntityArchetype,
                        flowFieldTileEntityArchetype);

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
