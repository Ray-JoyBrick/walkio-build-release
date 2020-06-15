namespace JoyBrick.Walkio.Game.Common.Utility
{
    using System.Linq;
    using UnityEngine;

    public static class RestoreMortonCodeLut
    {
        private static int[] CreateXPartFromTexture(Texture2D texture, int count)
        {
            var array = texture.GetRawTextureData<Color32>();

            return array.Take(count).Select(x => x.ToInt()).ToArray();
        }
        
        private static int[] CreateYPartFromTexture(Texture2D texture, int count)
        {
            var array = texture.GetRawTextureData<Color32>();

            // Very inefficient and lots of GC, but it is ok since for the entire project,
            // this only gets called once during the time project is initialized.
            // Just make sure the count is not too big so the memory holds.
            // Most likely will be after addressable is ready.
            return array.Reverse().Take(count).Reverse().Select(x => x.ToInt()).ToArray();
        }

        // Assume count is 2048 for this particular project
        public static (int[], int[]) CreateFromTexture(Texture2D texture, int count)
        {
            return (CreateXPartFromTexture(texture, count), CreateYPartFromTexture(texture, count));
        }
    }
}
