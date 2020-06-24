namespace JoyBrick.Walkio.Game.Move.FlowField.Utility
{
    using Unity.Collections;
    using Unity.Mathematics;
    using UnityEngine;

    public static class PathTileHelper
    {
        //
        public static int TileCount1d(
            int gridCellCount, float gridCellSize,
            int tileCellCount, float tileCellSize) =>
            (int) math.ceil((gridCellCount * gridCellSize) / (tileCellCount * tileCellSize));

        public static int2 TileCount2d(
            int hGridCellCount, int vGridCellCount,
            float hGridCellSize, float vGridCellSize,
            int hTileCellCount, int vTileCellCount,
            float hTileCellSize, float vTileCellSize)
        {
            var hCount = TileCount1d(hGridCellCount, hGridCellSize, hTileCellCount, hTileCellSize);
            var vCount = TileCount1d(vGridCellCount, vGridCellSize, vTileCellCount, vTileCellSize);

            return new int2(hCount, vCount);
        }

        public static int TileCount1d(
            int hGridCellCount, int vGridCellCount,
            float hGridCellSize, float vGridCellSize,
            int hTileCellCount, int vTileCellCount,
            float hTileCellSize, float vTileCellSize)
        {
            var tileCount2d =
                TileCount2d(
                    hGridCellCount, vGridCellCount,
                    hGridCellSize, vGridCellSize,
                    hTileCellCount, vTileCellCount,
                    hTileCellSize, vTileCellSize);

            return (tileCount2d.x * tileCount2d.y);
        }

        private static int PositionToTileIndex1d(int tileCellCount, float tileCellSize, float position) =>
            (int)math.floor(position / (tileCellCount * tileCellSize));
        
        private static int PositionToTileCellIndex1d(float tileCellSize, float basePosition, float position) =>
            (int)math.floor(math.abs(position - basePosition) / tileCellSize);
        
        public static int2 TileIndex2d(
            int hGridCellCount, int vGridCellCount,
            float hGridCellSize, float vGridCellSize,
            int hTileCellCount, int vTileCellCount,
            float hTileCellSize, float vTileCellSize,
            float hPosition, float vPosition)
        {
            var hIndex = PositionToTileIndex1d(hTileCellCount, hTileCellSize, hPosition);
            var vIndex = PositionToTileIndex1d(vTileCellCount, vTileCellSize, vPosition);
            
            return new int2(hIndex, vIndex);
        }
        
        public static int TileIndex1d(
            int hGridCellCount, int vGridCellCount,
            float hGridCellSize, float vGridCellSize,
            int hTileCellCount, int vTileCellCount,
            float hTileCellSize, float vTileCellSize,
            float hPosition, float vPosition)
        {
            var index2d =
                TileIndex2d(
                    hGridCellCount, vGridCellCount,
                    hGridCellSize, vGridCellSize,
                    hTileCellCount, vTileCellCount,
                    hTileCellSize, vTileCellSize,
                    hPosition, vPosition);
            var hCount = TileCount1d(hGridCellCount, hGridCellSize, hTileCellCount, hTileCellSize);
            var index = (index2d.y * hCount) + index2d.x;
            
            return index;
        }

        public static int2 PositionToTileAndTileCellIndex2d(
            int hGridCellCount, int vGridCellCount,
            float hGridCellSize, float vGridCellSize,
            int hTileCellCount, int vTileCellCount,
            float hTileCellSize, float vTileCellSize,
            float hPosition, float vPosition)
        {
            var hIndex = PositionToTileIndex1d(hTileCellCount, hTileCellSize, hPosition);
            var vIndex = PositionToTileIndex1d(vTileCellCount, vTileCellSize, vPosition);
            // var hCount = TileCount1D(hGridCellCount, hGridCellSize, hTileCellCount, hTileCellSize);
            var hCount = TileCount1d(hGridCellCount, hGridCellSize, hTileCellCount, hTileCellSize);

            var hBasePosition = hIndex * (hTileCellCount * hTileCellSize);
            var vBasePosition = vIndex * (vTileCellCount * vTileCellSize);

            var hTileCellIndex = PositionToTileCellIndex1d(hTileCellSize, hBasePosition, hPosition);
            var vTileCellIndex = PositionToTileCellIndex1d(vTileCellSize, vBasePosition, vPosition);

            var index = (vIndex * hCount) + hIndex; 
            var tileCellIndex = vTileCellIndex * hTileCellCount + hTileCellIndex;
            
            return new int2(index, tileCellIndex);            
        }

        public static int2 TileAndTileCellIndexToGridIndex2d(
            int hGridCellCount, int vGridCellCount,
            float hGridCellSize, float vGridCellSize,
            int hTileCellCount, int vTileCellCount,
            float hTileCellSize, float vTileCellSize,
            int hTileIndex, int vTileIndex,
            int hTileCellIndex, int vTileCellIndex)
        {
            // Not consider both grid cell size and tile size size
            // For now, just think the size is 1.0 for both
            var hGridCellIndex = hTileIndex * hTileCellCount + hTileCellIndex;
            var vGridCellIndex = vTileIndex * vTileCellCount + vTileCellIndex;
            
            return new int2(hGridCellIndex, vGridCellIndex);
        }

        public static int TileAndTileCellIndexToGridIndex1d(
            int hGridCellCount, int vGridCellCount,
            float hGridCellSize, float vGridCellSize,
            int hTileCellCount, int vTileCellCount,
            float hTileCellSize, float vTileCellSize,
            int hTileIndex, int vTileIndex,
            int hTileCellIndex, int vTileCellIndex)
        {
            var index2d =
                TileAndTileCellIndexToGridIndex2d(
                    hGridCellCount, vGridCellCount,
                    hGridCellSize, vGridCellSize,
                    hTileCellCount, vTileCellCount,
                    hTileCellSize, vTileCellSize,
                    hTileIndex, vTileIndex,
                    hTileCellIndex, vTileCellIndex);

            var gridIndex = -1;

            if (index2d.x < 0 || index2d.y < 0 || index2d.x >= hGridCellCount || index2d.y >= vGridCellCount)
            {
                // No need to assign as it is assigned to -1 already
            }
            else
            {
                gridIndex = (index2d.y * hGridCellCount + index2d.x);
            }
            
            return gridIndex;
        }

        //
        // public static int TileCount1D(
        //     int gridCellCount, float gridCellSize,
        //     int tileCellCount, float tileCellSize) =>
        //     (int) math.ceil((gridCellCount * gridCellSize) / (tileCellCount * tileCellSize));
        
        // public static int TileCount(
        //     int hGridCellCount, int vGridCellCount,
        //     float hGridCellSize, float vGridCellSize,
        //     int hTileCellCount, int vTileCellCount,
        //     // float hMultipleOfGridCellSize, float vMultipleOfGridCellSize,
        //     float hTileCellSize, float vTileCellSize)
        // {
        //     var hCount = TileCount1D(hGridCellCount, hGridCellSize, hTileCellCount, hTileCellSize);
        //     var vCount = TileCount1D(vGridCellCount, vGridCellSize, vTileCellCount, vTileCellSize);
        //
        //     return (hCount * vCount);
        // }

        // private static int TileIndex1D(int tileCellCount, float tileCellSize, float position) =>
        //     (int)math.floor(position / (tileCellCount * tileCellSize));
        //
        // private static int TileCellIndex1D(float tileCellSize, float basePosition, float position) =>
        //     (int)math.floor(math.abs(position - basePosition) / tileCellSize);
        
        // public static int TileIndex(
        //     int hGridCellCount, int vGridCellCount,
        //     float hGridCellSize, float vGridCellSize,
        //     int hTileCellCount, int vTileCellCount,
        //     float hTileCellSize, float vTileCellSize,
        //     float hPosition, float vPosition)
        // {
        //     var hIndex = TileIndex1D(hTileCellCount, hTileCellSize, hPosition);
        //     var vIndex = TileIndex1D(vTileCellCount, vTileCellSize, vPosition);
        //     var hCount = TileCount1D(hGridCellCount, hGridCellSize, hTileCellCount, hTileCellSize);
        //
        //     // Debug.Log($"hIndex: {hIndex} vIndex: {vIndex} hCount: {hCount}");
        //     
        //     var index = (vIndex * hCount) + hIndex;
        //     
        //     return index;
        // }

        // public static int2 TileIndexWithTileCellIndex(
        //     int hGridCellCount, int vGridCellCount,
        //     float hGridCellSize, float vGridCellSize,
        //     int hTileCellCount, int vTileCellCount,
        //     float hTileCellSize, float vTileCellSize,
        //     float hPosition, float vPosition)
        // {
        //     var hIndex = TileIndex1D(hTileCellCount, hTileCellSize, hPosition);
        //     var vIndex = TileIndex1D(vTileCellCount, vTileCellSize, vPosition);
        //     // var hCount = TileCount1D(hGridCellCount, hGridCellSize, hTileCellCount, hTileCellSize);
        //     var hCount = TileCount1d(hGridCellCount, hGridCellSize, hTileCellCount, hTileCellSize);
        //
        //     var hBasePosition = hIndex * (hTileCellCount * hTileCellSize);
        //     var vBasePosition = vIndex * (vTileCellCount * vTileCellSize);
        //
        //     var hTileCellIndex = PositionToTileCellIndex1d(hTileCellSize, hBasePosition, hPosition);
        //     var vTileCellIndex = PositionToTileCellIndex1d(vTileCellSize, vBasePosition, vPosition);
        //
        //     var index = (vIndex * hCount) + hIndex; 
        //     var tileCellIndex = vTileCellIndex * hTileCellCount + hTileCellIndex;
        //     
        //     return new int2(index, tileCellIndex);
        // }
        //
        // public static int TileIndexToGridIndex(
        //     int hGridCellCount, int vGridCellCount,
        //     float hGridCellSize, float vGridCellSize,
        //     int hTileCellCount, int vTileCellCount,
        //     float hTileCellSize, float vTileCellSize,
        //     int tileIndex)
        // {
        //     return 0;
        // }

        public static NativeList<int> OnPathTileIndices1d(
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
                    TileIndex1d(
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
