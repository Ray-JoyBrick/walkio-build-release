namespace JoyBrick.Walkio.Game.Environment.Utility
{
    using Unity.Collections;
    using Unity.Mathematics;
    using UnityEngine;

    public static class PathTileHelper
    {
        public static int TileCount1D(
            int gridCellCount, float gridCellSize,
            int tileCellCount, float tileCellSize) =>
            (int) math.ceil((gridCellCount * gridCellSize) / (tileCellCount * tileCellSize));

        public static int TileCount(
            int hGridCellCount, int vGridCellCount,
            float hGridCellSize, float vGridCellSize,
            int hTileCellCount, int vTileCellCount,
            // float hMultipleOfGridCellSize, float vMultipleOfGridCellSize,
            float hTileCellSize, float vTileCellSize)
        {
            var hCount = TileCount1D(hGridCellCount, hGridCellSize, hTileCellCount, hTileCellSize);
            var vCount = TileCount1D(vGridCellCount, vGridCellSize, vTileCellCount, vTileCellSize);

            return (hCount * vCount);
        }

        private static int TileIndex1D(int tileCellCount, float tileCellSize, float position) =>
            (int)math.floor(position / (tileCellCount * tileCellSize));

        private static int TileCellIndex1D(float tileCellSize, float basePosition, float position) =>
            (int)math.floor(math.abs(position - basePosition) / tileCellSize);
        
        public static int TileIndex(
            int hGridCellCount, int vGridCellCount,
            float hGridCellSize, float vGridCellSize,
            int hTileCellCount, int vTileCellCount,
            float hTileCellSize, float vTileCellSize,
            float hPosition, float vPosition)
        {
            var hIndex = TileIndex1D(hTileCellCount, hTileCellSize, hPosition);
            var vIndex = TileIndex1D(vTileCellCount, vTileCellSize, vPosition);
            var hCount = TileCount1D(hGridCellCount, hGridCellSize, hTileCellCount, hTileCellSize);

            // Debug.Log($"hIndex: {hIndex} vIndex: {vIndex} hCount: {hCount}");
            
            var index = (vIndex * hCount) + hIndex;
            
            return index;
        }

        public static int2 TileIndexWithTileCellIndex(
            int hGridCellCount, int vGridCellCount,
            float hGridCellSize, float vGridCellSize,
            int hTileCellCount, int vTileCellCount,
            float hTileCellSize, float vTileCellSize,
            float hPosition, float vPosition)
        {
            var hIndex = TileIndex1D(hTileCellCount, hTileCellSize, hPosition);
            var vIndex = TileIndex1D(vTileCellCount, vTileCellSize, vPosition);
            var hCount = TileCount1D(hGridCellCount, hGridCellSize, hTileCellCount, hTileCellSize);

            var hBasePosition = hIndex * (hTileCellCount * hTileCellSize);
            var vBasePosition = vIndex * (vTileCellCount * vTileCellSize);

            var hTileCellIndex = TileCellIndex1D(hTileCellSize, hBasePosition, hPosition);
            var vTileCellIndex = TileCellIndex1D(vTileCellSize, vBasePosition, vPosition);

            var index = (vIndex * hCount) + hIndex; 
            var tileCellIndex = vTileCellIndex * hTileCellCount + hTileCellIndex;
            
            return new int2(index, tileCellIndex);
        }

        public static int TileIndexToGridIndex(
            int hGridCellCount, int vGridCellCount,
            float hGridCellSize, float vGridCellSize,
            int hTileCellCount, int vTileCellCount,
            float hTileCellSize, float vTileCellSize,
            int tileIndex)
        {
            return 0;
        }

        public static NativeList<int> OnPathTiles(
            int hGridCellCount, int vGridCellCount,
            float hGridCellSize, float vGridCellSize,
            int hTileCellCount, int vTileCellCount,
            float hTileCellSize, float vTileCellSize,
            NativeList<float2> path)
        {
            var tileIndices = new NativeList<int>(Allocator.Temp);
            for (var i = 0; i < path.Length; ++i)
            {
                var position = path[i];
                var tileIndex =
                    TileIndex(
                        hGridCellCount, vGridCellCount,
                        hGridCellSize, vGridCellSize,
                        hTileCellCount, vTileCellCount,
                        hTileCellSize, vTileCellSize,
                        position.x, position.y);
                var contained = tileIndices.Contains(tileIndex);
                if (!contained)
                {
                    tileIndices.Add(tileIndex);
                }
            }

            return tileIndices;
        }
    }
}
