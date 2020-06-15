namespace JoyBrick.Walkio.Game.Common.Utility
{
    using System.Collections.Generic;
    using Unity.Mathematics;

    public static partial class MortonCode2d32Bit
    {
        // Pack into 32 bits to save size
        public struct Index2d
        {
            //
            public short X;
            public short Y;

            //
            public static implicit operator int2(Index2d b) => new int2(b.X, b.Y);
            public static implicit operator Index2d(int2 v) => new Index2d { X = (short)v.x, Y = (short)v.y };

            //
            public override string ToString()
            {
                var desc = $"Index2d({X}, {Y})";
                return desc;
            }
        }
        
        private static int EncodeMagicBit(short v)
        {
            int x = v;
            x = (x | x << 16) & 0x0000FFFF;
            x = (x | x << 8)  & 0x00FF00FF;
            x = (x | x << 4)  & 0x0F0F0F0F;
            x = (x | x << 2)  & 0x33333333;
            x = (x | x << 1)  & 0x55555555;

            return x;
        }

        public static int Encode(Index2d index)
        {
            return (EncodeMagicBit(index.X) | (EncodeMagicBit(index.Y) << 1));
        }

        private static int DecodeMagicBit(int v)
        {
            v = v & 0x55555555;
            v = (v | (v >> 1)) & 0x33333333;
            v = (v | (v >> 2)) & 0x0F0F0F0F;
            v = (v | (v >> 4)) & 0x00FF00FF;
            v = (v | (v >> 8)) & 0x0000FFFF;

            return v;
        }

        public static Index2d Decode(int v)
        {
            return new Index2d
            {
                X = (short) DecodeMagicBit(v),
                Y = (short) DecodeMagicBit(v >> 1)
            };
        }

        // It is the caller's responsibility to supply proper lut for both x and y
        public static int RestoredIndexFromLut(
            IReadOnlyList<int> mortonCodeLutX, IReadOnlyList<int> mortonCodeLutY,
            MortonCode2d32Bit.Index2d index)
        {
            return (mortonCodeLutX[index.X] + mortonCodeLutY[index.Y]);
        }
    }
}
