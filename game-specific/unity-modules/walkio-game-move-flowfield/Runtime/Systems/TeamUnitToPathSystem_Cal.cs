namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;
    
    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public partial class TeamUnitToPathSystem : SystemBase
    {
        private static List<Vector3> MapTileIndicesToPositions(List<int> tileIndices)
        {
            // This just uses the center position of the tile
            
            // TODO: Remove fake calculation
            var result = tileIndices.Select(tileIndex =>
            {
                var v = tileIndex / 5;
                var h = tileIndex % 5;

                return new Vector3(h * 10 + 5, 0, v * 10 + 5);
            }).ToList();

            return result;
        }        

        private static void SetupFlowFieldTileEntity(
            EntityManager entityManager,
            int teamId, int timeTick, Vector3 targetPosition,
            Dictionary<int, Entity> cachedEntities,
            int tileIndex, int uniformSize,
            Entity entity)
        {
            // _logger.Debug($"TeamUnitToPathSystem - SetupFlowFieldTileEntity - for teamId: {teamId} targetPosition: {targetPosition} entity: {entity}");
            
            // Actual flow field direction setup here

            entityManager.SetComponentData(entity, new FlowFieldTile
            {
                Index = tileIndex,
                            
                HorizontalCount = uniformSize,
                VerticalCount = uniformSize,
                            
                TimeTick = timeTick                        
            });
                    
            var tileBuffer = entityManager.AddBuffer<FlowFieldTileCellBuffer>(entity);
            var tileCellInBuffer = entityManager.AddBuffer<FlowFieldTileInCellBuffer>(entity);
            var tileCellOutBuffer = entityManager.AddBuffer<FlowFieldTileOutCellBuffer>(entity);
        
            var totalTileCellCount = 10 * 10;
            var tileCellInCount = 10 * 4;
            var tileCellOutCount = 10 * 4;
                    
            tileBuffer.ResizeUninitialized(totalTileCellCount);
            tileCellInBuffer.ResizeUninitialized(tileCellInCount);
            tileCellOutBuffer.ResizeUninitialized(tileCellOutCount);

            // For now, just random the direction for each cell
            
            var seed = new System.Random();
            var rnd = new Unity.Mathematics.Random((uint)seed.Next());

            for (var tv = 0; tv < 10; ++tv)
            {
                for (var th = 0; th < 10; ++th)
                {
                    var tileCellIndex = tv * 10 + th;

                    tileBuffer[tileCellIndex] = rnd.NextInt(0, 8);
                }
            }

            for (var i = 0; i < 10 * 4; ++i)
            {
                tileCellInBuffer[i] = -1;
            }

            for (var i = 0; i < 10 * 4; ++i)
            {
                tileCellOutBuffer[i] = -1;
            }

            //
            var hTileCount = 10;
            var vTileCount = 10;

            var topTileIndex = GetAdjacentTileIndex(hTileCount, vTileCount, tileIndex, EDirection.Top);
            var rightTileIndex = GetAdjacentTileIndex(hTileCount, vTileCount, tileIndex, EDirection.Right);
            var downTileIndex = GetAdjacentTileIndex(hTileCount, vTileCount, tileIndex, EDirection.Down);
            var leftTileIndex = GetAdjacentTileIndex(hTileCount, vTileCount, tileIndex, EDirection.Left);

            if (topTileIndex != -1)
            {
                // Has top tile
                var hasTopTile = cachedEntities.ContainsKey(topTileIndex);

                if (hasTopTile)
                {
                    var outTile = cachedEntities[topTileIndex];
                    ModifyFlowFieldTileInOutCell(outTile, entity);
                }
            }

            if (rightTileIndex != -1)
            {
                // Has right tile
                var hasRightTile = cachedEntities.ContainsKey(rightTileIndex);

                if (hasRightTile)
                {
                    var outTile = cachedEntities[rightTileIndex];
                    ModifyFlowFieldTileInOutCell(outTile, entity);
                }
            }

            if (downTileIndex != -1)
            {
                // Has down tile
                var hasDownTile = cachedEntities.ContainsKey(downTileIndex);

                if (hasDownTile)
                {
                    var outTile = cachedEntities[downTileIndex];
                    ModifyFlowFieldTileInOutCell(outTile, entity);
                }
            }

            if (leftTileIndex != -1)
            {
                // Has left tile
                var hasLeftTile = cachedEntities.ContainsKey(leftTileIndex);

                if (hasLeftTile)
                {
                    var outTile = cachedEntities[leftTileIndex];
                    ModifyFlowFieldTileInOutCell(outTile, entity);
                }
            }
        }
        
        // This handles tiles for one path
        private static List<Entity> FromTileIndicesToFlowFieldTileEntities(
            EntityManager entityManager,
            Dictionary<int, Dictionary<int, Entity>> cachedEntities,
            int teamId, int timeTick, Vector3 targetPosition,
            List<int> tileIndices,
            int uniformSize)
        {
            // Buffer has to be added separately
            // var entityArchetype = entityManager.CreateArchetype(
            //     typeof(FlowFieldTile));
            var entityArchetype = entityManager.CreateArchetype(
                typeof(FlowFieldTile),
                typeof(FlowFieldTileCellBuffer),
                typeof(FlowFieldTileInCellBuffer),
                typeof(FlowFieldTileOutCellBuffer));

            // This is the part where flow field tile entity is created, should avoid duplication for
            // the same tile entity that is created previously
            var flowFieldEntities = tileIndices.Select(tileIndex =>
            {
                var entity = Entity.Null;
                
                var table = cachedEntities[teamId];
                if (table.ContainsKey(tileIndex))
                {
                    // Cached, just use it
                    // May have some issue if just use previously defined entity?
                    entity = table[tileIndex];
                }
                else
                {
                    // Not in cache, create entity and cache it
                    entity = entityManager.CreateEntity(entityArchetype);

                    SetupFlowFieldTileEntity(entityManager, teamId, timeTick, targetPosition, table, tileIndex, uniformSize, entity);

                    table.Add(tileIndex, entity);
                }
            
                return entity;
            }).ToList();

            // Unit entity starts to goal
            // 1 -> 2 -> 3 -> 4 -> 5(Goal)
            
            // // Why is the list being reversed?
            // flowFieldEntities.Reverse();
            
            // After reversing, Goal starts to entity
            // 5(Goal) -> 4 -> 3 -> 2 -> 1

            //
            for (var i = 0; i < flowFieldEntities.Count; ++i)
            {
                var lastTileInList = (i == flowFieldEntities.Count - 1);
                if (!lastTileInList)
                {
                    // [at 1, next 2]
                    // [at 2, next 3]
                    // [at 3, next 4]
                    // [at 4, next 5(Goal)]
                    // 5(Goal) should be created and assigned already
                    var flowFieldEntity = flowFieldEntities[i];
                    var nextFlowFieldEntity = flowFieldEntities[i + 1];
                
                    // This seems to create an issue for cached tile entity
                    entityManager.SetComponentData(flowFieldEntity, new FlowFieldTile
                    {
                        HorizontalCount = uniformSize,
                        VerticalCount = uniformSize,
                        TimeTick = timeTick,
                        
                        // This is the only place to use next at this time, is this correct?
                        NextFlowFieldTile = nextFlowFieldEntity
                    });
                }
            }

            return flowFieldEntities;
        }

        private static List<int> GetTileIndicesFromPath(
            int hGridCellCount, int vGridCellCount,
            List<Vector3> path)
        {
            // TODO: Remove fake calculation
            var indices = new List<int>();
            // return new List<int> { };
            path.ForEach(position =>
            {
                var tileIndex =
                    Utility.PathTileHelper.TileIndex1d(
                        hGridCellCount, vGridCellCount, 1.0f, 1.0f, 
                        10, 10, 1.0f, 1.0f, 
                        position.x, position.z);

                var existed = indices.Exists(x => x == tileIndex);
                if (!existed)
                {
                    indices.Add(tileIndex);
                }
            });

            return indices;
        }

        public enum EDirection
        {
            Top,
            Right,
            Down,
            Left
        }
        

        // This is post process for flow field tile
        private static void ModifyFlowFieldTileInOutCell(Entity outTile, Entity inTile)
        {
            
        }

        private static int GetAdjacentTileIndex(
            int hTileCount, int vTileCount,
            int comparedIndex, EDirection direction)
        {
            var index = -1;

            var hComparedIndex = comparedIndex % hTileCount;
            var vComparedIndex = comparedIndex / vTileCount;

            if (direction == EDirection.Top)
            {
                var topIndex = vComparedIndex + 1;
                index = (topIndex < vTileCount) ? topIndex : -1;
            }
            else if (direction == EDirection.Right)
            {
                var rightIndex = hComparedIndex + 1;
                index = (rightIndex < hTileCount) ? rightIndex : -1;
            }
            else if (direction == EDirection.Down)
            {
                var downIndex = vComparedIndex - 1;
                index = (downIndex >= 0) ? downIndex : -1;
            }
            else if (direction == EDirection.Left)
            {
                var leftIndex = hComparedIndex - 1;
                index = (leftIndex >= 0) ? leftIndex : -1;
            }

            return index;
        }
        
        private static int GetTileIndex(float3 pos)
        {
            var a =
                Utility.PathTileHelper.PositionToTileAndTileCellIndex2d(
                    // 128, 192,
                    32, 32,
                    1.0f, 1.0f,
                    -16.0f, -16.0f,
                    10, 10,
                    1.0f, 1.0f,
                    pos.x, pos.z);
                
            return a.x;
        }
    }
}
