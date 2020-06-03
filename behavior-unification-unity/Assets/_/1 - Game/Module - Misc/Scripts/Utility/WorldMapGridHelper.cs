namespace JoyBrick.Walkio.Game.Environment.Utility
{
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Mathematics;
    using UnityEngine;

    public static partial class WorldMapGridHelper
    {
        //
        private static int PositionToGridCellIndex1D(float gridCellSize, float position) =>
            (int)math.floor(position / gridCellSize);

        private static float GridCellIndexToPosition1D(float gridCellSize, int gridCellIndex) =>
            (gridCellIndex * gridCellSize) + (gridCellSize * 0.5f);

        public static int2 PositionToGridCellIndex2D(int2 gridCellCount, float2 gridCellSize,
            float2 position)
        {
            var convert = new int2(
                PositionToGridCellIndex1D(gridCellSize.x, position.x),
                PositionToGridCellIndex1D(gridCellSize.y, position.y));

            var result = new int2(
                (convert.x < gridCellCount.x) ? convert.x : -1,
                (convert.y < gridCellCount.y) ? convert.y : -1);

            return result;
        }

        public static int PositionToGridCellIndex1D(int2 gridCellCount, float2 gridCellSize,
            float2 position)
        {
            var index2D = PositionToGridCellIndex2D(gridCellCount, gridCellSize, position);

            var result = (index2D.x < 0 || index2D.y < 0) ? -1 : ((index2D.y * gridCellCount.x) + index2D.x);

            return result;
        }

        // TODO: Checking gridCellIndex outside of grid cell
        public static float2 GridCellIndexToPosition2D(int2 gridCellCount, float2 gridCellSize, int2 gridCellIndex)
        {
            var hPosition = GridCellIndexToPosition1D(gridCellSize.x, gridCellIndex.x);
            var vPosition = GridCellIndexToPosition1D(gridCellSize.y, gridCellIndex.y);

            return new float2(hPosition, vPosition);
        }

        //
        private static int ConvertColor32ToInt(Color32 c)
        {
            var result = c[0] << 24 + c[1] << 16 + c[2] << 8 + c[3];
            return result;
        }

        private static int GetGridIndexFromTexturePixelIndex2D(
            Vector2Int tileCount2D,
            Vector2Int tileCellCount2D,
            Vector2Int tileIndex2D,
            Vector2Int pixelIndex2D)
        {
            var result =
                ((tileCount2D.x * tileCellCount2D.x * tileCellCount2D.y * tileIndex2D.y) + (tileCellCount2D.x * tileIndex2D.x)) +
                (tileCount2D.x * tileCellCount2D.x * pixelIndex2D.y) + pixelIndex2D.x;
            return result;
        }

        private static void AssignIndexToGridCell(Vector2Int gridTileCount2D, Vector2Int gridTileCellCount2D, Color32[] texturePixels,
            Vector2Int textureIndex2D, int[] gridCells)
        {
            for (var hPixelIndex = 0; hPixelIndex < gridTileCellCount2D.y; ++hPixelIndex)
            {
                for (var wPixelIndex = 0; wPixelIndex < gridTileCellCount2D.x; ++wPixelIndex)
                {
                    var pixelIndex2D = new Vector2Int(wPixelIndex, hPixelIndex);

                    var pixelIndex = pixelIndex2D.y * gridTileCellCount2D.x + pixelIndex2D.x;
                    var pixelColor = texturePixels[pixelIndex];
                    var gridIndex =
                        GetGridIndexFromTexturePixelIndex2D(
                            gridTileCount2D, gridTileCellCount2D, textureIndex2D, pixelIndex2D);

                    gridCells[gridIndex] = ConvertColor32ToInt(pixelColor);
                }
            }
        }

        public static int[] GetGridObstacleIndicesFromTextures(
            Texture2D[] texture2Ds,
            Vector2Int gridTileCount2D,
            Vector2Int gridTileCellCount2D)
        {
            var gridCellCount = new Vector2Int(
                gridTileCount2D.x * gridTileCellCount2D.x,
                gridTileCount2D.y * gridTileCellCount2D.y);

            var gridCells = new int[(gridCellCount.x * gridCellCount.y)];
 
            for (var hGridTileIndex = 0; hGridTileIndex < gridTileCount2D.y; ++hGridTileIndex)
            {
                for (var wGridTileIndex = 0; wGridTileIndex < gridTileCount2D.x; ++wGridTileIndex)
                {
                    var textureIndex2D = new Vector2Int(wGridTileIndex, hGridTileIndex);
            
                    var textureIndex = hGridTileIndex * gridTileCount2D.x + wGridTileIndex;
                    var texture2D = texture2Ds[textureIndex];
            
                    var texturePixels = texture2D.GetPixels32();

                    AssignIndexToGridCell(gridTileCount2D, gridTileCellCount2D, texturePixels, textureIndex2D, gridCells);
                }
            }

            return gridCells;
        }
    }
}
