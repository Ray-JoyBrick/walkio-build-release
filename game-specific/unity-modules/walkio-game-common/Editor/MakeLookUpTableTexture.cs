namespace Game
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    
    using JoyBrick.Walkio.Game.Common.Utility;
    
    public class MakeLookUpTableTexture
    {
        [MenuItem("Assets/Walkio/LookUp Table/Make Morton Code LUT")]
        public static void MakeMortonCodeLut()
        {
            //
            var indices = new int[64 * 64];
            for (var x = 0; x < 2048; ++x)
            {
                indices[x] = MortonCode2d32Bit.Encode(new MortonCode2d32Bit.Index2d {X = (short) x});
            }

            for (var y = 0; y < 2048; ++y)
            {
                indices[2048 + y] = MortonCode2d32Bit.Encode(new MortonCode2d32Bit.Index2d {Y = (short) y});
            }
            
            //
            var texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            for (var h = 0; h < 64; ++h)
            {
                for (var w = 0; w < 64; ++w)
                {
                    var index = h * 64 + w;

                    var c = (new Color32()).FromInt(indices[index]);
                    
                    texture.SetPixel(w, h, c);
                }
            }

            var bytes = texture.EncodeToPNG();
            
            var textureFolderPath = Path.Combine("Packages", "com.walkio.game.common", "Textures", "_Generated");
            var lutFilePath = Path.Combine(textureFolderPath, "Morton Code LUT.png");
            
            var lutFileFullPath = Path.GetFullPath(lutFilePath);
            
            File.WriteAllBytes(lutFileFullPath, bytes);
            AssetDatabase.Refresh();

            // // "Packages/com.walkio.game.common/Data Assets/Game Settings.asset"
            // var relativePath = Path.Combine("Packages", "com.walkio.game.common", "Data Assets", "Morton Code LUT.png");
            var textureImporter = AssetImporter.GetAtPath(lutFilePath) as TextureImporter;

            if (textureImporter != null)
            {
                textureImporter.filterMode = FilterMode.Point;
                textureImporter.maxTextureSize = 64;

                textureImporter.mipmapEnabled = false;

                textureImporter.isReadable = true;
                EditorUtility.SetDirty(textureImporter);
                textureImporter.SaveAndReimport();
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}
