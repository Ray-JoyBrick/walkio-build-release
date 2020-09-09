namespace JoyBrick.Walkio.Game.Level.Utility
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public static partial class GridWorldHelper
    {
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
            // var result =
            //     ((tileCount2D.x * tileCellCount2D.x * tileCellCount2D.y * tileIndex2D.y) + (tileCellCount2D.x * tileIndex2D.x)) +
            //     (tileCount2D.x * tileCellCount2D.x * pixelIndex2D.x) + (tileCellCount2D.x - pixelIndex2D.y - 1);
            var result =
                ((tileCount2D.x * tileCellCount2D.x * tileCellCount2D.y * tileIndex2D.y) + (tileCellCount2D.x * tileIndex2D.x)) +
                (tileCount2D.x * tileCellCount2D.x * pixelIndex2D.y) + pixelIndex2D.x;
            return result;
        }

        public static List<int> GetGridObstacleIndicesFromTextures(
            Dictionary<Color32, int> lookupTable,
            IList<Texture2D> texture2Ds,
            Vector2Int gridTileCount2D,
            Vector2Int gridTileCellCount2D)
        {
            var gridCellCount = new Vector2Int(
                gridTileCount2D.x * gridTileCellCount2D.x,
                gridTileCount2D.y * gridTileCellCount2D.y);

            var gridCells = new int[(gridCellCount.x * gridCellCount.y)];
            // var gridCells = new NativeArray<int>((gridCellCount.x * gridCellCount.y), Allocator.Temp);

            for (var hGridTileIndex = 0; hGridTileIndex < gridTileCount2D.y; ++hGridTileIndex)
            {
                for (var wGridTileIndex = 0; wGridTileIndex < gridTileCount2D.x; ++wGridTileIndex)
                {
                    var textureIndex2D = new Vector2Int(wGridTileIndex, hGridTileIndex);

                    var textureIndex = hGridTileIndex * gridTileCount2D.x + wGridTileIndex;
                    var texture2D = texture2Ds[textureIndex];

                    var texturePixels = texture2D.GetPixels32();

                    for (var hPixelIndex = 0; hPixelIndex < texture2D.height; ++hPixelIndex)
                    {
                        for (var wPixelIndex = 0; wPixelIndex < texture2D.width; ++ wPixelIndex)
                        {
                            var pixelIndex2D = new Vector2Int(wPixelIndex, hPixelIndex);

                            var pixelIndex = pixelIndex2D.y * texture2D.width + pixelIndex2D.x;
                            var pixelColor = texturePixels[pixelIndex];
                            var gridIndex =
                                GetGridIndexFromTexturePixelIndex2D(
                                    gridTileCount2D, gridTileCellCount2D, textureIndex2D, pixelIndex2D);

                            // gridCells[gridIndex] = ConvertColor32ToInt(pixelColor);
                            var hasKey = lookupTable.ContainsKey(pixelColor);
                            if (hasKey)
                            {
                                var lookupValue = lookupTable[pixelColor];
                                gridCells[gridIndex] = lookupValue;
                            }
                            else
                            {
                                Debug.Log($"No key for - {pixelColor}");
                            }
                        }
                    }
                }
            }

            return gridCells.ToList();
        }
    }
}
