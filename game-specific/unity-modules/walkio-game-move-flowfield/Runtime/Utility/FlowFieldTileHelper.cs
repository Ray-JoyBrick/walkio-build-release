namespace JoyBrick.Walkio.Game.Move.FlowField.Utility
{
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    public static partial class FlowFieldTileHelper
    {
        //
        private static int TileCount1D(
            int gridCellCount, float gridCellSize,
            int tileCellCount, float tileCellSize) =>
            (int) math.ceil((gridCellCount * gridCellSize) / (tileCellCount * tileCellSize));
    
        public static int2 GetTileCountFromGrid2D(
            int2 gridCellCount,
            float2 gridCellSize,
            int2 tileCellCount,
            float2 tileCellSize)
        {
            var hCount = TileCount1D(gridCellCount.x, gridCellSize.x, tileCellCount.x, tileCellSize.x);
            var vCount = TileCount1D(gridCellCount.y, gridCellSize.y, tileCellCount.y, tileCellSize.y);

            return new int2(hCount, vCount);
        }

        public static int GetTileCountFromGrid1D(
            int2 gridCellCount,
            float2 gridCellSize,
            int2 tileCellCount,
            float2 tileCellSize)
        {
            var hCount = TileCount1D(gridCellCount.x, gridCellSize.x, tileCellCount.x, tileCellSize.x);
            var vCount = TileCount1D(gridCellCount.y, gridCellSize.y, tileCellCount.y, tileCellSize.y);

            return (hCount * vCount);
        }
        
        //
        private static int PositionToTileIndex1D(int tileCellCount, float tileCellSize, float position) =>
            (int)math.floor(position / (tileCellCount * tileCellSize));

        private static int PositionToTileCellIndex1D(float tileCellSize, float basePosition, float position) =>
            (int)math.floor(math.abs(position - basePosition) / tileCellSize);


        // This does not check if it is out of boundary or not
        public static int2 PositionToTileIndexAtGrid2D(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            float2 position)
        {
            var hIndex = PositionToTileIndex1D(tileCellCount.x, tileCellSize.x, position.x);
            var vIndex = PositionToTileIndex1D(tileCellCount.y, tileCellSize.y, position.y);

            var gridCellIndex = WorldMapGridHelper.PositionToGridCellIndex1D(gridCellCount, gridCellSize, position);

            var result = (gridCellIndex < 0) ? new int2(-1, -1) : new int2(hIndex, vIndex);
            
            return result;
        }
        
        public static int PositionToTileIndexAtGrid1D(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            float2 position)
        {
            // var hIndex = PositionToTileIndex1D(tileCellCount.x, tileCellSize.x, position.x);
            // var vIndex = PositionToTileIndex1D(tileCellCount.y, tileCellSize.y, position.y);
            //
            var convert = PositionToTileIndexAtGrid2D(gridCellCount, gridCellSize, tileCellCount, tileCellSize, position);

            if (convert.x < 0 || convert.y < 0)
            {
                return -1;
            }

            var hCount = TileCount1D(gridCellCount.x, gridCellSize.x, tileCellCount.x, tileCellSize.x);
            var result = convert.y * hCount + convert.x;
            
            return result;
        }

        public static int4 PositionToTileAndTileCellIndexAtGridTo2DIndex(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            float2 position)
        {
            var hIndex = PositionToTileIndex1D(tileCellCount.x, tileCellSize.x, position.x);
            var vIndex = PositionToTileIndex1D(tileCellCount.y, tileCellSize.y, position.y);
            // var hCount = TileCount1D(hGridCellCount, hGridCellSize, hTileCellCount, hTileCellSize);
            var hCount = TileCount1D(gridCellCount.x, gridCellSize.x, tileCellCount.x, tileCellSize.x);

            var hBasePosition = hIndex * (tileCellCount.x * tileCellSize.x);
            var vBasePosition = vIndex * (tileCellCount.y * tileCellSize.y);
        
            var hTileCellIndex = PositionToTileCellIndex1D(tileCellSize.x, hBasePosition, position.x);
            var vTileCellIndex = PositionToTileCellIndex1D(tileCellSize.y, vBasePosition, position.y);
        
            var tileIndex = (vIndex * hCount) + hIndex; 
            var tileCellIndex = vTileCellIndex * tileCellCount.x + hTileCellIndex;
            
            return new int4(hIndex, vIndex, hTileCellIndex, vTileCellIndex);            
        }

        public static int2 PositionToTileAndTileCellIndexAtGridTo1DIndex(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            float2 position)
        {
            var hIndex = PositionToTileIndex1D(tileCellCount.x, tileCellSize.x, position.x);
            var vIndex = PositionToTileIndex1D(tileCellCount.y, tileCellSize.y, position.y);
            // var hCount = TileCount1D(hGridCellCount, hGridCellSize, hTileCellCount, hTileCellSize);
            var hCount = TileCount1D(gridCellCount.x, gridCellSize.x, tileCellCount.x, tileCellSize.x);

            var hBasePosition = hIndex * (tileCellCount.x * tileCellSize.x);
            var vBasePosition = vIndex * (tileCellCount.y * tileCellSize.y);
        
            var hTileCellIndex = PositionToTileCellIndex1D(tileCellSize.x, hBasePosition, position.x);
            var vTileCellIndex = PositionToTileCellIndex1D(tileCellSize.y, vBasePosition, position.y);
        
            var tileIndex = (vIndex * hCount) + hIndex; 
            var tileCellIndex = vTileCellIndex * tileCellCount.x + hTileCellIndex;
            
            return new int2(tileIndex, tileCellIndex);
        }

        //
        public static int2 FromGridCellIndexToTileIndex2D(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int2 gridCellIndex)
        {
            var position =
                WorldMapGridHelper.GridCellIndexToPosition2D(gridCellCount, gridCellSize, gridCellIndex);

            var result =
                PositionToTileIndexAtGrid1D(gridCellCount, gridCellSize, tileCellCount, tileCellSize, position);

            return result;
        }

        //
        public static int2 FromTileAndTileCellIndexToGridCellIndex2D(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int2 tileIndex, int2 tileCellIndex)
        {
            // Not consider both grid cell size and tile size size
            // For now, just think the size is 1.0 for both
            var gridCellIndex = tileIndex * tileCellCount + tileCellIndex;
            // var vGridCellIndex = vTileIndex * vTileCellCount + vTileCellIndex;
            
            // return new int2(gridCellIndex.x, gridCellIndex.y);
            return gridCellIndex;
        }
        
        public static int TileAndTileCellIndexToGridIndex1D(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int2 tileIndex, int2 tileCellIndex)
        {
            var index2d =
                FromTileAndTileCellIndexToGridCellIndex2D(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    tileIndex, tileCellIndex);

            var gridIndex = -1;

            if (index2d.x < 0 || index2d.y < 0 || index2d.x >= gridCellCount.x || index2d.y >= gridCellCount.y)
            {
                // No need to assign as it is assigned to -1 already
            }
            else
            {
                gridIndex = (index2d.y * gridCellCount.x + index2d.x);
            }
            
            return gridIndex;
        }

        //
        public static NativeArray<int> GetNeighborTileIndex(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int2 tileIndex)
        {
            var possibleNeighbors = new NativeArray<int>(8, Allocator.Temp);

            return possibleNeighbors;
        }
        
        public static NativeArray<int> GetNeighborTileCellIndex(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int2 tileCellIndex)
        {
            var possibleNeighbors = new NativeArray<int>(8, Allocator.Temp);

            return possibleNeighbors;
        }

        public static bool TileCellIndexInsideTile(
            int2 tileCellCount, int2 tileCellIndex)
        {
            if (tileCellIndex.x < 0
                || tileCellIndex.x >= tileCellCount.x
                || tileCellIndex.y < 0
                || tileCellIndex.y >= tileCellCount.y)
            {
                return false;
            }

            return true;
        }

        private static int ConvertToTileCellIndex1D(
            int2 tileCellCount, int2 tileCellIndex)
        {
            return (tileCellIndex.y * tileCellCount.x + tileCellIndex.x);
        }
        
        public static NativeArray<int> GetNeighborTileCellIndex(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int tileCellIndex)
        {
            var possibleNeighbors = new NativeArray<int>(8, Allocator.Temp);

            for (var i = 0; i < possibleNeighbors.Length; ++i)
            {
                possibleNeighbors[i] = -1;
            }

            var hTileCellIndex = tileCellIndex % tileCellCount.x;
            var vTileCellIndex = tileCellIndex / tileCellCount.x;
            
            var left = new int2(hTileCellIndex - 1, vTileCellIndex + 0);
            var right = new int2(hTileCellIndex + 1, vTileCellIndex + 0);
            var up = new int2(hTileCellIndex + 0, vTileCellIndex + 1);
            var down = new int2(hTileCellIndex + 0, vTileCellIndex - 1);
            
            var upLeft = new int2(hTileCellIndex - 1, vTileCellIndex + 1);
            var upRight = new int2(hTileCellIndex + 1, vTileCellIndex + 1);
            var downLeft = new int2(hTileCellIndex - 1, vTileCellIndex - 1);
            var downRight = new int2(hTileCellIndex + 1, vTileCellIndex - 1);

            if (TileCellIndexInsideTile(tileCellCount, upLeft))
            {
                possibleNeighbors[0] = ConvertToTileCellIndex1D(tileCellCount, upLeft);
            }

            if (TileCellIndexInsideTile(tileCellCount, up))
            {
                possibleNeighbors[1] = ConvertToTileCellIndex1D(tileCellCount, up);
            }

            if (TileCellIndexInsideTile(tileCellCount, upRight))
            {
                possibleNeighbors[2] = ConvertToTileCellIndex1D(tileCellCount, upRight);
            }

            if (TileCellIndexInsideTile(tileCellCount, left))
            {
                possibleNeighbors[3] = ConvertToTileCellIndex1D(tileCellCount, left);
            }
            
            if (TileCellIndexInsideTile(tileCellCount, right))
            {
                possibleNeighbors[4] = ConvertToTileCellIndex1D(tileCellCount, right);
            }

            if (TileCellIndexInsideTile(tileCellCount, downLeft))
            {
                possibleNeighbors[5] = ConvertToTileCellIndex1D(tileCellCount, downLeft);
            }

            if (TileCellIndexInsideTile(tileCellCount, down))
            {
                possibleNeighbors[6] = ConvertToTileCellIndex1D(tileCellCount, down);
            }

            if (TileCellIndexInsideTile(tileCellCount, downRight))
            {
                possibleNeighbors[7] = ConvertToTileCellIndex1D(tileCellCount, downRight);
            }

            return possibleNeighbors;
        }

        public static NativeArray<int2> GetNeighborTileCellIndex2D(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int tileCellIndex)
        {
            var possibleNeighbors = new NativeArray<int2>(8, Allocator.Temp);

            for (var i = 0; i < possibleNeighbors.Length; ++i)
            {
                possibleNeighbors[i] = -1;
            }

            var hTileCellIndex = tileCellIndex % tileCellCount.x;
            var vTileCellIndex = tileCellIndex / tileCellCount.x;
            
            var left = new int2(hTileCellIndex - 1, vTileCellIndex + 0);
            var right = new int2(hTileCellIndex + 1, vTileCellIndex + 0);
            var up = new int2(hTileCellIndex + 0, vTileCellIndex + 1);
            var down = new int2(hTileCellIndex + 0, vTileCellIndex - 1);
            
            var upLeft = new int2(hTileCellIndex - 1, vTileCellIndex + 1);
            var upRight = new int2(hTileCellIndex + 1, vTileCellIndex + 1);
            var downLeft = new int2(hTileCellIndex - 1, vTileCellIndex - 1);
            var downRight = new int2(hTileCellIndex + 1, vTileCellIndex - 1);

            if (TileCellIndexInsideTile(tileCellCount, upLeft))
            {
                possibleNeighbors[0] = new int2(0, ConvertToTileCellIndex1D(tileCellCount, upLeft));
            }

            if (TileCellIndexInsideTile(tileCellCount, up))
            {
                possibleNeighbors[1] = new int2(1, ConvertToTileCellIndex1D(tileCellCount, up));
            }

            if (TileCellIndexInsideTile(tileCellCount, upRight))
            {
                possibleNeighbors[2] = new int2(2, ConvertToTileCellIndex1D(tileCellCount, upRight));
            }

            if (TileCellIndexInsideTile(tileCellCount, left))
            {
                possibleNeighbors[3] = new int2(3, ConvertToTileCellIndex1D(tileCellCount, left));
            }
            
            if (TileCellIndexInsideTile(tileCellCount, right))
            {
                possibleNeighbors[4] = new int2(5, ConvertToTileCellIndex1D(tileCellCount, right));
            }

            if (TileCellIndexInsideTile(tileCellCount, downLeft))
            {
                possibleNeighbors[5] = new int2(6, ConvertToTileCellIndex1D(tileCellCount, downLeft));
            }

            if (TileCellIndexInsideTile(tileCellCount, down))
            {
                possibleNeighbors[6] = new int2(7, ConvertToTileCellIndex1D(tileCellCount, down));
            }

            if (TileCellIndexInsideTile(tileCellCount, downRight))
            {
                possibleNeighbors[7] = new int2(8, ConvertToTileCellIndex1D(tileCellCount, downRight));
            }

            return possibleNeighbors;
        }

        private static bool ContainValue(NativeQueue<int> indexQueue, int value)
        {
            var indexArray = indexQueue.ToArray(Allocator.Temp);
            bool contained = indexArray.Contains(value);

            if (indexArray.IsCreated)
            {
                indexArray.Dispose();
            }

            return contained;
        }

        private static int GetNeighborKindByTileCellIndex(
            int2 tileCellCount,
            int tileCellIndex, int neighborCellIndex)
        {
            var hTileCellIndex = tileCellIndex % tileCellCount.x;
            var vTileCellIndex = tileCellIndex / tileCellCount.x;

            var hNeightborCellIndex = neighborCellIndex % tileCellCount.x;
            var vNeightborCellIndex = neighborCellIndex / tileCellCount.x;

            if (hTileCellIndex == hNeightborCellIndex)
            {
                if (vTileCellIndex != vNeightborCellIndex)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }

            if (vTileCellIndex == vNeightborCellIndex)
            {
                if (hTileCellIndex != hNeightborCellIndex)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }

            return 1;
        }

        private static NativeArray<int> GetGridCellIndicesInTile(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int2 tileIndex)
        {
            var count = tileCellCount.x * tileCellCount.y;
            var indices = new NativeArray<int>(count, Allocator.Temp);

            return indices;
        }
        
        public static NativeArray<int> GetIntegrationCostForTile(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int2 goalTileCellIndex, NativeArray<int> baseCosts)
        {
            //
            var accumulateCosts = new NativeArray<int>(baseCosts.Length, Allocator.Temp);

            var openIndexQueue = new NativeQueue<int>(Allocator.Temp);
            
            // Might be better to use hash map?
            var deadendIndexHashMap = new NativeHashMap<int, int>(baseCosts.Length, Allocator.Temp);

            var count = tileCellCount.x * tileCellCount.y;
            for (var i = 0; i < count; ++i)
            {
                deadendIndexHashMap.Add(i, 1);
            }

            var goalTileCellIndex1D = goalTileCellIndex.y * tileCellCount.x + goalTileCellIndex.x;

            for (var i = 0; i < accumulateCosts.Length; ++i)
            {
                accumulateCosts[i] = 65535;
            }

            accumulateCosts[goalTileCellIndex1D] = 0;
            openIndexQueue.Enqueue(ConvertToTileCellIndex1D(tileCellCount, goalTileCellIndex));
            deadendIndexHashMap.Remove(goalTileCellIndex1D);
            
            while (openIndexQueue.Count > 0)
            {
                var index = openIndexQueue.Dequeue();
                
                // Debug.Log($"index: {index}");
                
                var neighborIndices = GetNeighborTileCellIndex(gridCellCount, gridCellSize, tileCellCount, tileCellSize, index);

                for (var i = 0; i < neighborIndices.Length; ++i)
                {
                    var neighborIndex = neighborIndices[i];

                    // Debug.Log($"neighborIndex: {neighborIndex}");

                    if (neighborIndex >= 0)
                    {
                        
                        // Need to know if neighbor is diagonal or not.
                        var neighborKind = GetNeighborKindByTileCellIndex(tileCellCount, index, neighborIndex);
                        var neighborToAddCost = (neighborKind == 0) ? 10 : 14;

                        var neighborBaseCost = baseCosts[neighborIndex];
                        var currentAccumulateCost = accumulateCosts[index];
                        var neighborAccumulateCost = accumulateCosts[neighborIndex];

                        var calculatedCost = currentAccumulateCost + neighborBaseCost + neighborToAddCost;
                        
                        Debug.Log($"index: {index} neighborIndex: {neighborIndex} calculatedCost: {calculatedCost} neighborBaseCost: {neighborBaseCost} currentAccumulateCost: {currentAccumulateCost}  neighborAccumulateCost: {neighborAccumulateCost}");

                        if (calculatedCost < neighborAccumulateCost)
                        {
                            var contained = ContainValue(openIndexQueue, neighborIndex);
                            if (!contained)
                            {
                                openIndexQueue.Enqueue(neighborIndex);

                                deadendIndexHashMap.Remove(neighborIndex);
                            }
                        
                            // Store the smallest value back
                            accumulateCosts[neighborIndex] = calculatedCost;
                        }
                    }
                }
            }

            // if (accumulateCosts.IsCreated)
            // {
            //     accumulateCosts.Dispose();
            // }

            if (openIndexQueue.IsCreated)
            {
                openIndexQueue.Dispose();
            }

            if (deadendIndexHashMap.IsCreated)
            {
                deadendIndexHashMap.Dispose();
            }

            return accumulateCosts;
        }

        public static NativeArray<int> GetIntegrationCostForTile(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int goalTileCellIndex, NativeArray<int> baseCosts)
        {
            var hGoalTileCellIndex = goalTileCellIndex % tileCellCount.x;
            var vGoalTileCellIndex = goalTileCellIndex / tileCellCount.y;

            var goalTileCellIndex2D = new int2(hGoalTileCellIndex, vGoalTileCellIndex);

            return GetIntegrationCostForTile(
                gridCellCount, gridCellSize,
                tileCellCount, tileCellSize,
                goalTileCellIndex2D, baseCosts);
        }

        public static NativeArray<int> GetDirectionForTile(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int2 goalTileCellIndex, NativeArray<int> baseCosts)
        {
            var integrationCosts =
                GetIntegrationCostForTile(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    goalTileCellIndex, baseCosts);
            
            var directions = new NativeArray<int>(integrationCosts.Length, Allocator.Temp);

            for (var i = 0; i < directions.Length; ++i)
            {
                directions[i] = -1;
            }

            var goalTileCellIndex1D = goalTileCellIndex.y * tileCellCount.x + goalTileCellIndex.x;
            
            for (var i = 0; i < integrationCosts.Length; ++i)
            {
                var cost = integrationCosts[i];
                var minCost = new int2(-1, int.MaxValue);
                
                if (goalTileCellIndex1D == i)
                {
                    directions[i] = 4;
                }
                else
                {
                    var neighborDirectionIndices =
                        GetNeighborTileCellIndex2D(gridCellCount, gridCellSize, tileCellCount, tileCellSize, i);

                    // Debug.Log($"i: {i} neighborIndices.Length: {neighborDirectionIndices.Length}");
                    for (var j = 0; j < neighborDirectionIndices.Length; ++j)
                    {
                        var neighborDirectionAndIndex = neighborDirectionIndices[j];

                        if (neighborDirectionAndIndex.x >= 0)
                        {
                            var neighborDirection = neighborDirectionAndIndex.x;
                            var neighborCost = integrationCosts[neighborDirectionAndIndex.y];
                        
                            // Debug.Log($"i: {i} cost: {cost} neighborDirection: {neighborDirection} neighborCost: {neighborCost}");
                        
                            if (neighborCost < minCost.y)
                            {
                                minCost.x = neighborDirection;
                                minCost.y = neighborCost;
                            }

                            // Debug.Log($"i: {i} cost: {cost} minCost: {minCost}");
                        
                            if (minCost.y <= cost)
                            {
                                directions[i] = minCost.x;
                            }
                        }
                    }
                }
            }

            if (integrationCosts.IsCreated)
            {
                integrationCosts.Dispose();
            }

            return directions;
        }

        //
        public static NativeArray<int2> GetTileIndexPairOnPath(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            NativeArray<float2> points)
        {
            // var cachedIndices = new NativeHashMap<int, int>();
            var tileIndices = new NativeList<int2>(Allocator.Persistent);

            // var hasPreviousPoint = false;
            var previousPoint = new float2(0, 0);
            var previousTileIndex = -1;

            for (var i = 0; i < points.Length; ++i)
            {
                var currentPoint = points[i];
                var tileIndex =
                    PositionToTileIndexAtGrid1D(
                        gridCellCount, gridCellSize,
                        tileCellCount, tileCellSize,
                        currentPoint);
                
                Debug.Log($"GetTileIndexPairOnPath - currentPoint: {currentPoint} tileIndex: {tileIndex}");

                // var inCache = cachedIndices.ContainsKey(tileIndex);
                // if (!inCache)
                // {
                //     cachedIndices.Add(tileIndex, 0);
                // }

                if (i == 0)
                {
                    // The first point, no previous point
                }
                else
                {
                    // Should have previous tileIndex
                    if (previousTileIndex != tileIndex)
                    {
                        // First tile index indicates out tile where
                        // the second tile index indicates in tile
                        tileIndices.Add(new int2(previousTileIndex, tileIndex));
                    }
                }

                previousPoint = currentPoint;
                previousTileIndex = tileIndex;
            }

            return tileIndices.AsArray();
        }

        public struct TilePairInfo
        {
            public int OutTileIndex;
            public int InTileIndex;
            public int OutTileCellIndex;
            public int InTileCellIndex;

            public override string ToString()
            {
                var desc =
                    $"OutTileIndex: {OutTileIndex} InTileIndex: {InTileIndex} OutTileCellIndex: {OutTileCellIndex} InTileCellIndex: {InTileCellIndex}";
                return desc;
            }
        }

        //
        public static NativeArray<TilePairInfo> GetTilePairInfoOnPath(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            NativeArray<float2> points)
        {
            var tilePairInfos = new NativeList<TilePairInfo>(Allocator.Persistent);

            var previousPoint = new float2(0, 0);
            var previousTileCellIndex = -1;
            var previousTileIndex = -1;

            for (var i = 0; i < points.Length; ++i)
            {
                var currentPoint = points[i];
                var tileIndex =
                    PositionToTileAndTileCellIndexAtGridTo1DIndex(
                        gridCellCount, gridCellSize,
                        tileCellCount, tileCellSize,
                        currentPoint);
                
                Debug.Log($"GetTileIndexPairOnPath - currentPoint: {currentPoint} tileIndex: {tileIndex}");

                if (i == 0)
                {
                    // The first point, no previous point
                }
                else
                {
                    // Should have previous tileIndex
                    if (previousTileIndex != tileIndex.x)
                    {
                        // First tile index indicates out tile where
                        // the second tile index indicates in tile
                        tilePairInfos.Add(new TilePairInfo
                        {
                            OutTileIndex = previousTileIndex,
                            InTileIndex = tileIndex.x,
                            OutTileCellIndex = previousTileCellIndex,
                            InTileCellIndex = tileIndex.y
                        });
                    }
                }

                previousPoint = currentPoint;
                previousTileCellIndex = tileIndex.y;
                previousTileIndex = tileIndex.x;
            }

            return tilePairInfos.AsArray();
        }

        // public static NativeArray<TilePairInfo> GetTilePairInfoOnPath(
        //     int2 gridCellCount, float2 gridCellSize,
        //     int2 tileCellCount, float2 tileCellSize,
        //     NativeArray<float2> points)
        // {
        // }

        public enum ETileCellAtDirection8
        {
            UpLeft,
            Up,
            UpRight,
            Left,
            Right,
            DownLeft,
            Down,
            DownRight
        }

        public enum ETileCellAtSide4
        {
            NotAtSide,
            Up,
            Right,
            Down,
            Left
        }
        
        // Will have 4 edge cases
        // At 0, tileCellCount.x - 1,
        // tileCellCount.y * (tileCellCount.x - 1)
        // (tileCellCount.x * tileCellCount.y) - 1
        // For example, if tile is 10 * 10 cells,
        // At indices 0, 9, 90, and 99 will be considered to both side. Need assist in these edge cases.
        // For these 4 cases, need opposite cell index to assist the check
        public static ETileCellAtSide4 GetSideFromTileCellIndex(
            int2 tileCellCount,
            int tileCellIndex, int oppositeTileCellIndex)
        {
            var tileCellAtDirection4 = ETileCellAtSide4.NotAtSide;
            var dividedValue = (tileCellIndex / tileCellCount.x);
            var moduloValue = (tileCellIndex % tileCellCount.x);

            var atDownSide = (dividedValue == 0);
            var atUpSide = (dividedValue == tileCellCount.y - 1);
            var atLeftSide = (moduloValue == 0);
            var atRightSide = (moduloValue == tileCellCount.x - 1);

            var atBothSide =
                (atDownSide && atLeftSide)
                || (atDownSide && atRightSide)
                || (atUpSide && atLeftSide)
                || (atUpSide && atRightSide);

            if (atBothSide)
            {
                // Short circuit the calculation if not at the edge case
                var oppositeTileCellAtDirection4 = ETileCellAtSide4.NotAtSide;
                var oppositeDividedValue = (oppositeTileCellIndex / tileCellCount.x);
                var oppositeModuloValue = (oppositeTileCellIndex % tileCellCount.x);

                var oppositeAtDownSide = (oppositeDividedValue == 0);
                var oppositeAtUpSide = (oppositeDividedValue == tileCellCount.y - 1);
                var oppositeAtLeftSide = (oppositeModuloValue == 0);
                var oppositeAtRightSide = (oppositeModuloValue == tileCellCount.x - 1);
                // Handle edge cases before normal cases
                if (atDownSide && atLeftSide)
                {
                    // Need oppositeTileCellIndex to know the puzzle
                    if (oppositeAtDownSide || oppositeAtRightSide)
                    {
                        tileCellAtDirection4 = ETileCellAtSide4.Left;
                    }
                    else if (oppositeAtUpSide || oppositeAtLeftSide)
                    {
                        tileCellAtDirection4 = ETileCellAtSide4.Down;
                    }
                }
                else if (atDownSide && atRightSide)
                {
                    if (oppositeAtUpSide || oppositeAtRightSide)
                    {
                        tileCellAtDirection4 = ETileCellAtSide4.Down;
                    }
                    else if (oppositeAtDownSide || oppositeAtLeftSide)
                    {
                        tileCellAtDirection4 = ETileCellAtSide4.Right;
                    }
                }
                else if (atUpSide && atLeftSide)
                {
                    if (oppositeAtUpSide || oppositeAtRightSide)
                    {
                        tileCellAtDirection4 = ETileCellAtSide4.Left;
                    }
                    else if (oppositeAtDownSide || oppositeAtLeftSide)
                    {
                        tileCellAtDirection4 = ETileCellAtSide4.Up;
                    }
                }
                else if (atUpSide && atRightSide)
                {
                    if (oppositeAtUpSide || oppositeAtLeftSide)
                    {
                        tileCellAtDirection4 = ETileCellAtSide4.Right;
                    }
                    else if (oppositeAtDownSide || oppositeAtRightSide)
                    {
                        tileCellAtDirection4 = ETileCellAtSide4.Up;
                    }
                }
            }
            // Normal cases below
            else if (atDownSide)
            {
                tileCellAtDirection4 = ETileCellAtSide4.Down;
            }
            else if (atUpSide)
            {
                tileCellAtDirection4 = ETileCellAtSide4.Up;
            }
            else if (atLeftSide)
            {
                tileCellAtDirection4 = ETileCellAtSide4.Left;
            }
            else if (atRightSide)
            {
                tileCellAtDirection4 = ETileCellAtSide4.Right;
            }

            return tileCellAtDirection4;
        }

        public static ETileCellAtSide4 GetOppositeSide(ETileCellAtSide4 side)
        {
            var oppositeSide = ETileCellAtSide4.NotAtSide;
            if (side == ETileCellAtSide4.Up)
            {
                oppositeSide = ETileCellAtSide4.Down;
            }
            else if (side == ETileCellAtSide4.Down)
            {
                oppositeSide = ETileCellAtSide4.Up;
            }
            else if (side == ETileCellAtSide4.Left)
            {
                oppositeSide = ETileCellAtSide4.Right;
            }
            else if (side == ETileCellAtSide4.Right)
            {
                oppositeSide = ETileCellAtSide4.Left;
            }

            return oppositeSide;
        }

        public static NativeArray<int> TileCellIndicesAtSide(
            int2 tileCellCount,
            ETileCellAtSide4 tileCellAtSide)
        {
            var sideIndices = new NativeArray<int>(tileCellCount.x, Allocator.Temp);

            for (var i = 0; i < tileCellCount.x; ++i)
            {
                if (tileCellAtSide == ETileCellAtSide4.Down)
                {
                    sideIndices[i] = i;
                }
                else if (tileCellAtSide == ETileCellAtSide4.Up)
                {
                    sideIndices[i] = ((tileCellCount.y - 1) * tileCellCount.x) + i;
                }
                else if (tileCellAtSide == ETileCellAtSide4.Left)
                {
                    sideIndices[i] = i * tileCellCount.x;
                }
                else if (tileCellAtSide == ETileCellAtSide4.Right)
                {
                    sideIndices[i] = ((i + 1) * tileCellCount.x) - 1;
                }
            }

            return sideIndices;
        }

        // assume it is square tile where x and y are equal
        public static NativeArray<int> GetPromotedTileCellDirection(
            int2 tileCellCount,
            DynamicBuffer<FlowFiledTileCellBuffer> outFlowFiledTileCellBuffer,
            int outTileIndex,
            int outTileCellIndex,
            ETileCellAtSide4 outTileCellAtSide,
            DynamicBuffer<FlowFiledTileCellBuffer> inFlowFiledTileCellBuffer,
            int inTileIndex,
            int inTileCellIndex,
            ETileCellAtSide4 inTileCellAtSide)
        {
            var outSideIndices = TileCellIndicesAtSide(tileCellCount, outTileCellAtSide);
            var inSideIndices = TileCellIndicesAtSide(tileCellCount, inTileCellAtSide);
            
            // These two cached index list should be equal size all the time
            var cachedOutSideIndices = new NativeList<int>();
            var cachedOutTileCellIndices = new NativeList<int>();

            for (var outSideIndex = 0; outSideIndex < outSideIndices.Length; ++outSideIndex)
            {
                var tileCellIndexOfOutSide = outSideIndices[outSideIndex];
                var tileCellInfoIndex = outFlowFiledTileCellBuffer[tileCellIndexOfOutSide];

                // Just assume 1 is obstacle, will alert this later
                if (tileCellInfoIndex == 1)
                {
                    var containingOutTileCellIndex = cachedOutTileCellIndices.AsArray().Contains(outTileCellIndex);
                    if (!containingOutTileCellIndex)
                    {
                        cachedOutSideIndices.Clear();
                        cachedOutTileCellIndices.Clear();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    cachedOutSideIndices.Add(outSideIndex);
                    cachedOutTileCellIndices.Add(tileCellIndexOfOutSide);
                }
            }
            
            var adjustedOutSideIndices = new NativeList<int>();
            // Now cachedOutTileCellIndices should have indices containing outTileCellIndex
            for (var i = 0; i < cachedOutSideIndices.Length; ++i)
            {
                var cachedOutTileCellIndex = cachedOutTileCellIndices[i];
                var tileCellIndexOfInSide = cachedOutSideIndices[i];
                var tileCellInfoIndex = inFlowFiledTileCellBuffer[tileCellIndexOfInSide];
                if (tileCellInfoIndex == 1)
                {
                    // Obstacle
                    adjustedOutSideIndices.Clear();
                }
                else
                {
                    // Not obstacle
                    adjustedOutSideIndices.Add(cachedOutTileCellIndex);
                }
            }

            if (outSideIndices.IsCreated)
            {
                outSideIndices.Dispose();
            }
            
            if (inSideIndices.IsCreated)
            {
                inSideIndices.Dispose();
            }

            // This indices may or may not contain original outTileCellIndex, this should be fine.
            // As no matter what, the tile cell at outTileCellIndex will have the correct direction,
            // be promoted or not won't affect its correctness
            return adjustedOutSideIndices.AsArray();
        }

        public static void PromoteTileCellDirection(
            // Environment environment,
            int2 tileCellCount,
            WorldMapGridCellKindBlobAsset worldMapGridCellKindBlobAsset,
            WorldMapGridBlobAsset worldMapGridBlobAsset,
            NativeArray<Entity> tiles,
            EntityManager entityManager,
            TilePairInfo tilePairInfo)
        {
            var outTile = tiles[tilePairInfo.OutTileIndex];
            var inTile = tiles[tilePairInfo.InTileIndex];

            //
            // tilePairInfo.OutTileCellIndex
            var outTileCellAtSide4 = GetSideFromTileCellIndex(tileCellCount, tilePairInfo.OutTileCellIndex, tilePairInfo.InTileCellIndex);
            var inTileCellAtSide4 = GetOppositeSide(outTileCellAtSide4);

            var outTileCellBuffer = entityManager.GetBuffer<FlowFiledTileCellBuffer>(outTile);
            var inTileCellBuffer = entityManager.GetBuffer<FlowFiledTileCellBuffer>(inTile);

            var directionPromotedOutTileCellIndices =
                GetPromotedTileCellDirection(
                    tileCellCount,
                    outTileCellBuffer, tilePairInfo.OutTileIndex, tilePairInfo.OutTileCellIndex, outTileCellAtSide4,
                    inTileCellBuffer, tilePairInfo.InTileIndex, tilePairInfo.InTileCellIndex, inTileCellAtSide4);
            
            var outTileCellDirectionBuffer = entityManager.GetBuffer<FlowFiledTileCellDirectionBuffer>(outTile);
            var outTileCellDirection = outTileCellDirectionBuffer[tilePairInfo.OutTileCellIndex];

            for (var i = 0; i < directionPromotedOutTileCellIndices.Length; ++i)
            {
                var toAlertOutTileCellIndex = directionPromotedOutTileCellIndices[i];
                outTileCellDirectionBuffer[toAlertOutTileCellIndex] = outTileCellDirection;
            }
        }
    }

    public struct Environment : IComponentData
    {
        public int2 TileCellCount;
    }
    
    public struct WorldMapGridCellKindBlobAsset
    {
        public BlobArray<int> KindIndices;
    }
    
    public struct WorldMapGridBlobAsset
    {
        // public BlobArray<WaypointPath> WaypointPaths;
        // public BlobArray<float3> Waypoints;

        public BlobArray<int> CellKindIndices;
    }

    public struct FlowFieldTileBlobAsset
    {
        // public BlobArray<Entity>
    }

    public struct FlowFieldTile : IComponentData
    {
    }

    // Should the naming need adjustment? or make this blobasset instead
    public struct FlowFiledTileCellBuffer : IBufferElementData
    {
        public int Value;
        
        public static implicit operator int(FlowFiledTileCellBuffer b) => b.Value;
        public static implicit operator FlowFiledTileCellBuffer(int v) => new FlowFiledTileCellBuffer { Value = v };
    }
    
    public struct FlowFiledTileCellDirectionBuffer : IBufferElementData
    {
        public int Value;
        
        public static implicit operator int(FlowFiledTileCellDirectionBuffer b) => b.Value;
        public static implicit operator FlowFiledTileCellDirectionBuffer(int v) => new FlowFiledTileCellDirectionBuffer { Value = v };
    }
}
