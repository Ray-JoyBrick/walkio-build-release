namespace Game
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    
    using JoyBrick.Walkio.Game.Common.Utility;
    
    public class MakeLookUpTableTexture
    {
        private static void MakeMortonCodeLut(int width, int height)
        {
            var total = width * height;
            
            //
            var indices = new int[total];
            for (var x = 0; x < total; ++x)
            {
                indices[x] = MortonCode2d32Bit.Encode(new MortonCode2d32Bit.Index2d {X = (short) x});
            }

            for (var y = 0; y < total; ++y)
            {
                indices[total + y] = MortonCode2d32Bit.Encode(new MortonCode2d32Bit.Index2d {Y = (short) y});
            }
            
            //
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (var h = 0; h < height; ++h)
            {
                for (var w = 0; w < width; ++w)
                {
                    var index = h * width + w;

                    var c = (new Color32()).FromInt(indices[index]);
                    
                    texture.SetPixel(w, h, c);
                }
            }

            var bytes = texture.EncodeToPNG();
            
            var textureFolderPath = Path.Combine("Packages", "com.walkio.game.common", "Textures", "_Generated");
            var lutFilePath = Path.Combine(textureFolderPath, $"Morton Code LUT {width}.png");
            
            var lutFileFullPath = Path.GetFullPath(lutFilePath);
            
            File.WriteAllBytes(lutFileFullPath, bytes);
            AssetDatabase.Refresh();

            // // "Packages/com.walkio.game.common/Data Assets/Game Settings.asset"
            // var relativePath = Path.Combine("Packages", "com.walkio.game.common", "Data Assets", "Morton Code LUT.png");
            var textureImporter = AssetImporter.GetAtPath(lutFilePath) as TextureImporter;

            if (textureImporter != null)
            {
                textureImporter.filterMode = FilterMode.Point;
                textureImporter.maxTextureSize = width;

                textureImporter.mipmapEnabled = false;

                textureImporter.isReadable = true;
                EditorUtility.SetDirty(textureImporter);
                textureImporter.SaveAndReimport();
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            
        }
        
        [MenuItem("Assets/Walkio/LookUp Table/Make Morton Code LUT 16 (256)")]
        public static void MakeMortonCodeLut016()
        {
            MakeMortonCodeLut(16, 16);
        }

        [MenuItem("Assets/Walkio/LookUp Table/Make Morton Code LUT 32 (1024)")]
        public static void MakeMortonCodeLut032()
        {
            MakeMortonCodeLut(32, 32);
        }

        [MenuItem("Assets/Walkio/LookUp Table/Make Morton Code LUT 64 (4096)")]
        public static void MakeMortonCodeLut064()
        {
            MakeMortonCodeLut(64, 64);
        }
        
        [MenuItem("Assets/Walkio/LookUp Table/Make Morton Code LUT 128 (16384)")]
        public static void MakeMortonCodeLut128()
        {
            MakeMortonCodeLut(128, 128);
        }
    }
}
