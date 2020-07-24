namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;

    [DisableAutoCreation]
    [UpdateAfter(typeof(SystemA))]
    public class SystemB : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemB));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        public IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

        public void Construct()
        {
            _logger.Debug($"Module - SystemB - Construct");
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
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
            using (var hashTable = new NativeHashMap<int2, bool>(capacity, Allocator.TempJob))
            {
                _logger.Debug($"Module - SystemB - OnUpdate - event entity: {entity}");

                var goalTileIndex = new int2(-1, -1);

                // for (var i = 0; i < pathPointSeparationBuffers.Length; ++i)
                // {
                //     var index = pathPointSeparationBuffers[i];
                //
                //     var startIndex = index.Value.x;
                //     var endIndex = index.Value.y;
                //
                //     for (var j = startIndex; j < endIndex; ++j)
                //     {
                //         var pathPoint = pathPointBuffers[j];
                //         var tileIndex = Utility.FlowFieldTileHelper.PositionToTileIndexAtGrid2D(
                //             gridCellCount, gridCellSize,
                //             tileCellCount, tileCellSize,
                //             new float2(pathPoint.Value.x, pathPoint.Value.z));
                //
                //         if (j == endIndex - 1)
                //         {
                //             if (goalTileIndex.x != -1 && goalTileIndex.y != -1)
                //             {
                //                 if (goalTileIndex.x == tileIndex.x && goalTileIndex.y == tileIndex.y)
                //                 {
                //                     
                //                 }
                //                 else
                //                 {
                //                     _logger.Warning($"Module - SystemB - ProcessPathPoint: goalTileIndex {goalTileIndex} is not the same as {tileIndex}");
                //                 }
                //             }
                //             
                //             goalTileIndex = tileIndex;
                //         }
                //         
                //         var added = hashTable.TryAdd(tileIndex, true);
                //         // if (added)
                //         // {
                //         //     _logger.Debug($"Module - SystemB - OnUpdate - tileIndex: {tileIndex} is added");
                //         // }
                //     }
                // }
                
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

                    var pathTileInfos =
                        Utility.FlowFieldTileHelper.GetTilePairInfoOnPath(
                            gridCellCount, gridCellSize,
                            tileCellCount, tileCellSize,
                            reversePoints);

                    for (var infoIndex = 0; infoIndex < pathTileInfos.Length; ++infoIndex)
                    {
                        var a = pathTileInfos[infoIndex].InTileIndex;
                    }

                    // for (var j = startIndex; j < endIndex; ++j)
                    // {
                    //     var pathPoint = pathPointBuffers[j];
                    //     var tileIndex = Utility.FlowFieldTileHelper.GetTilePairInfoOnPath(
                    //         gridCellCount, gridCellSize,
                    //         tileCellCount, tileCellSize,
                    //         new float2(pathPoint.Value.x, pathPoint.Value.z));
                    //
                    //     
                    //     var added = hashTable.TryAdd(tileIndex, true);
                    //     // if (added)
                    //     // {
                    //     //     _logger.Debug($"Module - SystemB - OnUpdate - tileIndex: {tileIndex} is added");
                    //     // }
                    // }
                }                

                using (var tileIndices = hashTable.GetKeyArray(Allocator.TempJob))
                {
                    var leadingToSetEntity = commandBuffer.CreateEntity(leadingToSetEntityArchetype);

                    var leadingToTileBuffer = commandBuffer.AddBuffer<LeadingToTileBuffer>(leadingToSetEntity);

                    var tileCount = tileIndices.Length;
                    leadingToTileBuffer.ResizeUninitialized(tileCount);

                    // var goalTileIndex = tileIndices[tileCount - 1];
                    
                    for (var ti = 0; ti < tileCount; ++ti)
                    {
                        var flowFieldTileEntity = commandBuffer.CreateEntity(flowFieldTileEntityArchetype);

                        var tileIndex = tileIndices[ti];
                        
                        commandBuffer.SetComponent(flowFieldTileEntity, new FlowFieldTileProperty
                        {
                            WorldId = 0,
                            GroupId = groupId,
                            TileIndex = tileIndex
                        });
                        
                        // Set flow tile cell buffer for this tile
                        var gridCellIndices =
                            Utility.FlowFieldTileHelper.GetGridCellIndicesInTile(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                tileIndex);

                        var tileCellBuffer = commandBuffer.AddBuffer<FlowFieldTileCellBuffer>(flowFieldTileEntity);
                        var totalTileCellCount = tileCellCount.x * tileCellCount.y;
                        tileCellBuffer.ResizeUninitialized(totalTileCellCount);
                        
                        //
                        var baseCosts = new NativeArray<int>(totalTileCellCount, Allocator.Temp);

                        for (var i = 0; i < totalTileCellCount; ++i)
                        {
                            // Assign grid cell index into tile cell for now
                            // Can also assign the content of that grid cell into tile cell
                            baseCosts[i] = GetBaseCostByGridCellIndex(gridCellIndices[i]);

                        }

                        var directions =
                            Utility.FlowFieldTileHelper.GetIntegrationCostForTile(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                goalTileIndex, baseCosts);
                        // var directions =
                        //     Utility.FlowFieldTileHelper.GetDirectionForTile(
                        //         gridCellCount, gridCellSize,
                        //         tileCellCount, tileCellSize,
                        //         goalTileIndex, baseCosts);

                        for (var i = 0; i < totalTileCellCount; ++i)
                        {
                            // Assign grid cell index into tile cell for now
                            // Can also assign the content of that grid cell into tile cell
                            // tileCellBuffer[i] = gridCellIndices[i];
                            tileCellBuffer[i] = new TileCellContent
                            {
                                CellIndex = gridCellIndices[i],
                                BaseCost = baseCosts[i],
                                Direction = directions[i]
                            };
                        }

                        //
                        leadingToTileBuffer[ti] = flowFieldTileEntity;
                    }
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
