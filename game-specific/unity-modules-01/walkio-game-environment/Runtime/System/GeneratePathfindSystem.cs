namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    [DisableAutoCreation]
    [UpdateAfter(typeof(GenerateZoneSystem))]
    public class GeneratePathfindSystem :
        SystemBase
    {
        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            //
            _entityCommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();            
        }
        
        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var theEnvironmentQuery = EntityManager.CreateEntityQuery(typeof(TheEnvironment));
            var theEnvironmentEntity = theEnvironmentQuery.GetSingletonEntity();
            var theEnvironment = EntityManager.GetComponentData<TheEnvironment>(theEnvironmentEntity);
            
            var zoneEntityQuery = EntityManager.CreateEntityQuery(typeof(Zone));
            var zoneEntity = zoneEntityQuery.GetSingletonEntity();
            var zone = EntityManager.GetComponentData<Zone>(zoneEntity);

            var pathfindBoardArchetype = EntityManager.CreateArchetype(
                typeof(PathfindBoard),
                typeof(PathfindBoardBuffer));

            var pathfindTileArchetype = EntityManager.CreateArchetype(
                typeof(PathfindTile),
                typeof(PathfindTileBuffer));

            Entities
                .WithAll<GeneratePathfind>()
                // .ForEach((Entity entity, int entityInQueryIndex) =>
                .ForEach((Entity entity) =>
                {
                    var hGridCellCount = zone.Width;
                    var vGridCellCount = zone.Height;

                    var hGridCellSize = theEnvironment.GridCellSize.x;
                    var vGridCellSize = theEnvironment.GridCellSize.y;

                    var hTileCellCount = theEnvironment.TileCellCount.x;
                    var vTileCellCount = theEnvironment.TileCellCount.y;
                    
                    var hTileCellSize = theEnvironment.TileCellSize.x;
                    var vTileCellSize = theEnvironment.TileCellSize.y;

                    var hTileCount = 
                        Utility.PathTileHelper.TileCount1d(
                            hGridCellCount, hGridCellSize, hTileCellCount, hTileCellSize);
                    
                    var vTileCount = 
                        Utility.PathTileHelper.TileCount1d(
                            vGridCellCount, vGridCellSize, vTileCellCount, vTileCellSize);

                    var totalTileCount = hTileCount * vTileCount;

                    // var pathfindTileEntity = concurrentCommandBuffer.CreateEntity(entityInQueryIndex, pathfindTileArchetype);
                    var pathfindBoardEntity = commandBuffer.CreateEntity(pathfindBoardArchetype);
                    
                    // concurrentCommandBuffer.SetComponent(entityInQueryIndex, pathfindTileEntity, new PathfindTile
                    commandBuffer.SetComponent(pathfindBoardEntity, new PathfindBoard
                    {
                        HorizontalCount = hTileCount,
                        VerticalCount = vTileCount
                    });

                    // var buffer = concurrentCommandBuffer.AddBuffer<PathfindTileBuffer>(entityInQueryIndex, pathfindTileEntity);
                    var buffer = commandBuffer.AddBuffer<PathfindBoardBuffer>(pathfindBoardEntity);
                    
                    buffer.ResizeUninitialized(totalTileCount);
                    
                    Debug.Log($"{hTileCount} {vTileCount}");

                    // Extract out the assignment to make it less complicated
                    for (var v = 0; v < vTileCount; ++v)
                    {
                        for (var h = 0; h < hTileCount; ++h)
                        {
                            var index = v * hTileCount + h;
                            
                            Debug.Log($"index: {index}");
                            
                            var pathfindTileEntity =
                                // concurrentCommandBuffer.CreateEntity(entityInQueryIndex, pathfindSingleTileArchetype);
                                commandBuffer.CreateEntity(pathfindTileArchetype);
                            
                            commandBuffer.SetComponent(pathfindTileEntity, new PathfindTile
                            {
                                Index = index,
                                Index2d = new int2(h, v),
                                HorizontalCount = hTileCellCount,
                                VerticalCount = vTileCellCount
                            });
                            var tileBuffer = commandBuffer.AddBuffer<PathfindTileBuffer>(pathfindTileEntity);

                            var totalTileCellCount = hTileCellCount * vTileCellCount;
                            
                            tileBuffer.ResizeUninitialized(totalTileCellCount);

                            SetupPathTile(
                                hGridCellCount, vGridCellCount,
                                hGridCellSize, vGridCellSize,
                                hTileCellCount, vTileCellCount,
                                hTileCellSize, vTileCellSize,
                                h, v,
                                tileBuffer);
                            
                            buffer[index] = pathfindTileEntity;
                        }
                    }
                    
                    // concurrentCommandBuffer.DestroyEntity(entityInQueryIndex, entity);
                    commandBuffer.DestroyEntity(entity);
                })
                .WithoutBurst()
                .Run();
                // .Schedule();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        private static void SetupPathTile(
            int hGridCellCount, int vGridCellCount,
            float hGridCellSize, float vGridCellSize,
            int hTileCellCount, int vTileCellCount,
            float hTileCellSize, float vTileCellSize,
            int hTileIndex, int vTileIndex,
            DynamicBuffer<PathfindTileBuffer> tileBuffer)
        {
            for (var tv = 0; tv < vTileCellCount; ++tv)
            {
                for (var th = 0; th < hTileCellCount; ++th)
                {
                    var tileCellIndex = tv * hTileCellCount + th;

                    var gridCellIndex =
                        Utility.PathTileHelper.TileAndTileCellIndexToGridIndex1d(
                            hGridCellCount, vGridCellCount,
                            hGridCellSize, vGridCellSize,
                            hTileCellCount, vTileCellCount,
                            hTileCellSize, vTileCellSize,
                            hTileIndex, vTileIndex,
                            th, tv);

                    tileBuffer[tileCellIndex] = gridCellIndex;
                }
            }
        }
    }
}
