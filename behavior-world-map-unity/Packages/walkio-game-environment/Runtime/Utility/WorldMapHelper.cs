namespace JoyBrick.Walkio.Game.Environment.Utility
{
    using Unity.Mathematics;

    public static class WorldMapHelper
    {
        public static int GetTileTypeIndex(int r, int g, int b)
        {
            var index = -1;
            var all255 = (r == 255) && (g == 255) && (b == 255);
            if (all255)
            {
                index = 0;
            }
            else
            {
                var adjustedR = (int)math.floor((float)r / (float)20);
                var adjustedG = (int)math.floor((float)g / (float)20);
                var adjustedB = (int)math.floor((float)b / (float)20);

                if (adjustedR < 12)
                {
                    index = adjustedR;
                }
                else if (adjustedR == 12)
                {
                    index = adjustedR + adjustedG;
                }
                else if (adjustedR == 12 && adjustedG == 12)
                {
                    index = adjustedR + adjustedG + adjustedB;
                }
                else
                {
                    index = 0;
                }
            }

            return index;
        }

        public static int GetAdjustedLength(int originalLength, int tileLength)
        {
            return (int) math.ceil((float) originalLength / (float) tileLength);
        }

        public static bool IsOutBoundary(int originalLength, int adjustedLength, int index)
        {
            var outBoundary = false;
            var v = index % adjustedLength;
            if (v >= originalLength)
            {
                outBoundary = true;
            }

            return outBoundary;
        }

        public static int GetOriginIndexInTile(
            int worldWidth, int worldHeight,
            int tileWidth, int tileHeight,
            int xTileIndex, int yTileIndex)
        {
            return (yTileIndex * tileHeight * worldWidth) + (xTileIndex * tileWidth);
        }
        
        public static int GetTileIndex(
            int worldWidth, int worldHeight,
            int tileWidth, int tileHeight,
            int xTileIndex, int yTileIndex,
            int xIndex, int yIndex)
        {
            var originIndexInTile = GetOriginIndexInTile(worldWidth, worldHeight, tileWidth, tileHeight, xTileIndex, yTileIndex);
            var inTileIndex = ((yIndex * tileWidth) + xIndex);
            var index = (originIndexInTile + inTileIndex);

            return index;
        }
    }
}