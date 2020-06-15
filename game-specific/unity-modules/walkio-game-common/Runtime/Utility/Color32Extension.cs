namespace JoyBrick.Walkio.Game.Common.Utility
{
    using UnityEngine;

    public static class Color32Helper
    {
        public static int ConvertToInt(Color32 c)
        {
            return ((c.a << 24) | (c.r << 16) | (c.g << 8) | c.b);
        }

        public static Color32 ConvertFromInt(int v)
        {
            return new Color32
            {
                b = (byte) ((v) & 0xFF),
                g = (byte) ((v >> 8) & 0xFF),
                r = (byte) ((v >> 16) & 0xFF),
                a = (byte) ((v >> 24) & 0xFF)
            };
        }
    }

    public static class Color32Extension
    {
        public static int ToInt(this Color32 c)
        {
            return Color32Helper.ConvertToInt(c);
        }

        public static Color32 FromInt(this Color32 c, int v)
        {
            return Color32Helper.ConvertFromInt(v);
        }
    }
}
