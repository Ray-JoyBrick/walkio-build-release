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
    }
}
