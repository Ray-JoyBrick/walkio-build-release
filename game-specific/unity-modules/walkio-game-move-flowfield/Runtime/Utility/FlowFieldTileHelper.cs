namespace JoyBrick.Walkio.Game.Move.FlowField.Utility
{
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    public static partial class FlowFieldTileHelper
    {
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

            var gridCellIndex = GridWorldHelper.PositionToGridCellIndex1D(gridCellCount, gridCellSize, position);

            var result = (gridCellIndex < 0) ? new int2(-1, -1) : new int2(hIndex, vIndex);

            return result;
        }

        public static int PositionToTileIndexAtGrid1D(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            float2 position)
        {
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

        public static float2 TileIndexToPosition2D(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int2 tileIndex)
        {
            var centerOfTile = tileIndex * tileCellCount * tileCellSize + (0.5f * (tileCellCount * tileCellSize));

            return centerOfTile;
        }

        //
        public static NativeArray<int> GetGridCellIndicesInTile(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int2 tileIndex)
        {
            var count = tileCellCount.x * tileCellCount.y;
            var indices = new NativeArray<int>(count, Allocator.Temp);

            var baseIndex = tileCellCount * tileIndex;

            for (var ty = 0; ty < tileCellCount.y; ++ty)
            {
                for (var tx = 0; tx < tileCellCount.x; ++tx)
                {
                    var i = (ty * tileCellCount.x) + tx;
                    var checkIndex = baseIndex + new int2(tx, ty);

                    var cellIndex = -1;

                    // Debug.Log($"baseIndex: {baseIndex} checkIndex: {checkIndex}");

                    if (checkIndex.x < gridCellCount.x && checkIndex.y < gridCellCount.y)
                    {
                        cellIndex = (checkIndex.y * gridCellCount.x) + checkIndex.x;
                    }

                    indices[i] = cellIndex;
                }
            }

            return indices;
        }

        private static int ConvertToTileCellIndex1D(
            int2 tileCellCount, int2 tileCellIndex)
        {
            return (tileCellIndex.y * tileCellCount.x + tileCellIndex.x);
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

                        // Debug.Log($"index: {index} neighborIndex: {neighborIndex} calculatedCost: {calculatedCost} neighborBaseCost: {neighborBaseCost} currentAccumulateCost: {currentAccumulateCost}  neighborAccumulateCost: {neighborAccumulateCost}");

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

        public static int NeighborTileDirection(int2 selfTileIndex, int2 neighborTileIndex)
        {
            var x = neighborTileIndex.x - selfTileIndex.x;
            var y = neighborTileIndex.y - selfTileIndex.y;

            if (x == -1 && y == 1)
            {
                return 0;
            }
            else if (x == 0 && y == 1)
            {
                return 1;
            }
            else if (x == 1 && y == 1)
            {
                return 2;
            }
            else if (x == -1 && y == 0)
            {
                return 3;
            }
            else if (x == 1 && y == 0)
            {
                return 5;
            }
            else if (x == -1 && y == -1)
            {
                return 6;
            }
            else if (x == 0 && y == -1)
            {
                return 7;
            }
            else if (x == 1 && y == -1)
            {
                return 8;
            }

            return 4;
        }

        // The number neighborDirection
        // 0 1 2
        // 3 4 5
        // 6 7 8
        public static NativeArray<int> GetDirectionForTile(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int2 goalTileCellIndex,
            int neighborTileDirection,
            NativeArray<int> baseCosts)
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
                    // This is the goal, which is the out cell index, assign a direction to neighbor
                    // directions[i] = 4;
                    directions[i] = neighborTileDirection;
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
            var tileIndices = new NativeList<int2>(Allocator.Temp);

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
            var tilePairInfos = new NativeList<TilePairInfo>(Allocator.Temp);

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

                public struct TilePairInfo2D
        {
            public int2 OutTileIndex;
            public int2 InTileIndex;
            public int2 OutTileCellIndex;
            public int2 InTileCellIndex;

            public override string ToString()
            {
                var desc =
                    $"OutTileIndex: {OutTileIndex} InTileIndex: {InTileIndex} OutTileCellIndex: {OutTileCellIndex} InTileCellIndex: {InTileCellIndex}";
                return desc;
            }
        }

        public static NativeArray<TilePairInfo2D> GetTilePairInfoOnPath2DArray(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            NativeArray<float2> points)
        {
            var tilePairInfos = new NativeList<TilePairInfo2D>(Allocator.Temp);
            // var tilePairInfos = new NativeList<TilePairInfo2D>(Allocator.Persistent);

            var previousPoint = new float2(0, 0);
            var previousTileCellIndex = new int2(-1, -1);
            var previousTileIndex = new int2(-1, -1);

            for (var i = 0; i < points.Length; ++i)
            {
                var currentPoint = points[i];
                var tileAndTileCellIndex =
                    PositionToTileAndTileCellIndexAtGridTo2DIndex(
                        gridCellCount, gridCellSize,
                        tileCellCount, tileCellSize,
                        currentPoint);

                // Debug.Log($"GetTilePairInfoOnPath2DArray - currentPoint: {currentPoint} tileIndex: {tileAndTileCellIndex.xy} tileCellIndex: {tileAndTileCellIndex.zw}");

                if (i == 0)
                {
                    // The first point, no previous point
                }
                else
                {
                    // Should have previous tileIndex
                    if (previousTileIndex.x == tileAndTileCellIndex.x && previousTileIndex.y == tileAndTileCellIndex.y)
                    {

                    }
                    else
                    {
                        tilePairInfos.Add(new TilePairInfo2D
                        {
                            OutTileIndex = previousTileIndex,
                            InTileIndex = tileAndTileCellIndex.xy,
                            OutTileCellIndex = previousTileCellIndex,
                            InTileCellIndex = tileAndTileCellIndex.zw
                        });
                    }
                }

                previousPoint = currentPoint;
                previousTileIndex = tileAndTileCellIndex.xy;
                previousTileCellIndex = tileAndTileCellIndex.zw;
            }

            // return tilePairInfos.AsArray();
            return tilePairInfos.ToArray(Allocator.Temp);
        }

        public static float3 FromIntDirectionToNormalizedVector(int v)
        {
            var direction = float3.zero;

            if (v == 0)
            {
                direction = math.normalize(new float3(-1.0f, 0, 1.0f));
            }
            else if (v == 1)
            {
                direction = new float3(0.0f, 0, 1.0f);
            }
            else if (v == 2)
            {
                direction = math.normalize(new float3(1.0f, 0, 1.0f));
            }
            else if (v == 3)
            {
                direction = new float3(-1.0f, 0, 0.0f);
            }
            else if (v == 4)
            {
                // direction = math.normalize(new float3(0, 0, 0));
                direction = new float3(0, 0, 0);
            }
            else if (v == 5)
            {
                direction = new float3(1.0f, 0, 0.0f);
            }
            else if (v == 6)
            {
                direction = math.normalize(new float3(-1.0f, 0, -1.0f));
            }
            else if (v == 7)
            {
                direction = new float3(0.0f, 0, -1.0f);
            }
            else if (v == 8)
            {
                direction = math.normalize(new float3(1.0f, 0, -1.0f));
            }

            return direction;
        }
    }
}
