namespace JoyBrick.Walkio.Tool.LevelDesign.EditorPart
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public partial class LevelTable
    {
        private static Texture2D MakeTextureBasedOnLayer(
            int width, int height, Vector3 basePosition,
            string layerName,
            bool checkTag, string tag,
            Color32 hitColor, Color32 noHitColor)
        {
            Debug.Log($"HandleSceneOpenedAffair - MakeTextureBasedOnLayer - layerName: {layerName}");

            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var colors = new Color32[width * height];

            for (var z = 0; z < height; ++z)
            {
                for (var x = 0; x < width; ++x)
                {
                    var adjustedX = basePosition.x + x + 0.5f;
                    var adjustedZ = basePosition.z + z + 0.5f;

                    var w = adjustedX;
                    var h = adjustedZ;
                    var tx = x;
                    var ty = z;

                    var colorIndex = ty * width + tx;

                    var layerMask = LayerMask.GetMask(layerName);
                    var raycastHits =
                        Physics.RaycastAll(
                            new Ray(new Vector3(w, 100.0f, h), Vector3.down),
                            150.0f, layerMask);

                    var castedRaycastHits = raycastHits.ToList();
                    if (castedRaycastHits.Count > 0)
                    {
                        // Debug.Log($"HandleSceneOpenedAffair - MakeTextureBasedOnInBoundaryFloor - width: {width} height: {height} is in boundary floor");

                        // Use the first one as there should be one
                        var raycastHit = castedRaycastHits[0];

                        // var color = new Color32(0, 0, 0, 255);
                        if (checkTag)
                        {
                            var havingTag = castedRaycastHits
                                .Where(crh => string.CompareOrdinal(crh.collider.tag, tag) == 0);
                            if (havingTag.Any())
                            {
                                colors[colorIndex] = hitColor;
                            }
                            else
                            {
                                colors[colorIndex] = noHitColor;
                            }
                        }
                        else
                        {
                            colors[colorIndex] = hitColor;
                        }
                    }
                    else
                    {
                        // var color = new Color32(0, 0, 0, 255);

                        colors[colorIndex] = noHitColor;
                    }
                }
            }

            texture.SetPixels32(colors);

            return texture;
        }

        private static Texture2D CombineTextures(List<Texture2D> textures)
        {
            if (!textures.Any()) return null;

            var width = textures.First().width;
            var height = textures.First().height;

            // All format should be the same
            var texture = new Texture2D(width, height, textures.First().format, false);

            var combinedColors = new Color32[width * height];

            for (var tIndex = 0; tIndex < textures.Count; ++tIndex)
            {
                var t = textures[tIndex];

                var colors = t.GetPixels32();

                for (var i = 0; i < combinedColors.Length; ++i)
                {
                    var cc = combinedColors[i];
                    var c = colors[i];

                    var r = cc.r + c.r;
                    var g = cc.g + c.g;
                    var b = cc.b + c.b;
                    var a = cc.a + c.a;

                    combinedColors[i].r = (r < 255) ? (byte) r : (byte) 255;
                    combinedColors[i].g = (g < 255) ? (byte) g : (byte) 255;
                    combinedColors[i].b = (b < 255) ? (byte) b : (byte) 255;
                    // combinedColors[i].a = (a < 255) ? (byte) a : (byte) 255;
                    combinedColors[i].a = 255;
                }
            }

            texture.SetPixels32(combinedColors);

            return texture;
        }

        private static string HandleObstacleForSubScene(
             string levelName,
             int index,
             Scene subScene,
             Vector3 basePosition,
             int wCellCount,
             int hCellCount,
             IEnumerable<Area> areas)
         {
             Debug.Log($"HandleObstacleForSubScene - level: {levelName} sub-scene: {subScene}");

              // var inBoundaryFloorTexture = MakeTextureBasedOnInBoundaryFloor(wCellCount, hCellCount, basePosition);
              // var outBoundaryFloorTexture = MakeTextureBasedOnOutBoundaryFloor(wCellCount, hCellCount, basePosition);
              // var obstacleTexture = MakeTextureBasedOnObstacle(wCellCount, hCellCount, basePosition);
              // var areaTexture = MakeTextureBasedOnArea(wCellCount, hCellCount, basePosition);

              var baseColor = new Color32(0, 0, 0, 255);
              var obstacleColor = new Color32(255, 255, 255, 255);

              var groundBaseTexture = MakeTextureBasedOnLayer(
                  wCellCount, hCellCount, basePosition,
                "Ground Base", false, "",
                  baseColor, baseColor);
              var obstacleTexture = MakeTextureBasedOnLayer(
                  wCellCount, hCellCount, basePosition,
                  "Obstacle", false, "",
                  obstacleColor, baseColor);
              var areaTextures =
                  areas.Select(area =>
                  {
                      var color = area.materialColor;
                      var areaTexture = MakeTextureBasedOnLayer(
                          wCellCount, hCellCount, basePosition,
                          "Area", true, area.tag,
                          color, baseColor);

                      return areaTexture;
                  }).ToList();

              var toBeCombinedTextures = new List<Texture2D>
              {
                  groundBaseTexture,
                  obstacleTexture
              };
              toBeCombinedTextures.AddRange(areaTextures);
              var combinedTexture = CombineTextures(toBeCombinedTextures);

              // Save the texture
              // var generatedDirectoryPath = Path.Combine(Application.dataPath, "_", "1 - Game - Level Design - Generated");
              var generatedDirectoryPath = Path.Combine(Application.dataPath, "_", "_Generated - Level");
              // var levelDirectoryPath = Path.Combine(generatedDirectoryPath, "Module - Environment - Level", "Levels");
              var levelDirectoryPath = generatedDirectoryPath;
              GameCommon.EditorPart.Utility.FileHelper.CreateDirectoryIfNotExisted(generatedDirectoryPath);
              // GameCommon.EditorPart.Utility.FileHelper.CreateDirectoryIfNotExisted(levelDirectoryPath);

              var specificLevelDirectoryPath = Path.Combine(levelDirectoryPath, levelName);
              GameCommon.EditorPart.Utility.FileHelper.CreateDirectoryIfNotExisted(specificLevelDirectoryPath);

              var obstacleTextureDirectoryPath = Path.Combine(specificLevelDirectoryPath, "obstacle-texture");
              GameCommon.EditorPart.Utility.FileHelper.CreateDirectoryIfNotExisted(obstacleTextureDirectoryPath);

              var obstacleTextureAssetPath = Path.Combine(obstacleTextureDirectoryPath, $"obstacle{index:0000}.png");

              var bytes = combinedTexture.EncodeToPNG();
              File.WriteAllBytes(obstacleTextureAssetPath, bytes);
              AssetDatabase.Refresh();

              // var obstacleTextureAssetLevelPath = Path.Combine("Assets", "_", "1 - Game - Level Design - Generated",
              //     "Module - Environment - Level",
              //     "Levels", levelName, "obstacle-texture");

              var obstacleTextureAssetLevelPath = Path.Combine("Assets", "_", "_Generated - Level",
                  levelName,
                  "obstacle-texture");

              var relativeObstacleTextureAssetPath = Path.Combine(obstacleTextureAssetLevelPath, $"obstacle{index:0000}.png");

              var textureImporter = AssetImporter.GetAtPath(relativeObstacleTextureAssetPath) as TextureImporter;

              if (textureImporter != null)
              {
                  textureImporter.filterMode = FilterMode.Point;

                  // Check this discussion to know how to get the next power of 2 number for
                  // any given number
                  // https://stackoverflow.com/questions/466204/rounding-up-to-next-power-of-2
                  var nextPowerOfTwo = Mathf.Pow(2, Mathf.Ceil((Mathf.Log(wCellCount) / Mathf.Log(2))));
                  textureImporter.maxTextureSize = (int)nextPowerOfTwo;

                  textureImporter.mipmapEnabled = false;

                  textureImporter.isReadable = true;

                  EditorUtility.SetDirty(textureImporter);
                  textureImporter.SaveAndReimport();
              }

              return relativeObstacleTextureAssetPath;
         }

         private static IEnumerable<string> CreateObstacleTexture(
             string levelName, Scene masterScene,
             IEnumerable<Area> areas)
         {
             var levelOperator = GameCommon.Utility.SceneHelper.GetComponentAtScene<LevelOperator>(masterScene);
             if (levelOperator == null) return Enumerable.Empty<string>();

             var texturePaths = new List<string>();
             for (var hSubSceneIndex = 0; hSubSceneIndex < levelOperator.zSubSceneCount; ++hSubSceneIndex)
             {
                 for (var wSubSceneIndex = 0; wSubSceneIndex < levelOperator.xSubSceneCount; ++wSubSceneIndex)
                 {
                     var i = hSubSceneIndex * levelOperator.xSubSceneCount + wSubSceneIndex;

                     var sceneAsset = levelOperator.subScenes[i];
                     var subScene = EditorSceneManager.GetSceneByName(sceneAsset.name);

                     var basePosition = new Vector3(
                         wSubSceneIndex * levelOperator.gridCount,
                         0,
                         hSubSceneIndex * levelOperator.gridCount);
                     var texturePath =
                         HandleObstacleForSubScene(
                             levelName,
                             i, subScene, basePosition,
                             levelOperator.gridCount,
                             levelOperator.gridCount,
                             areas);

                     texturePaths.Add(texturePath);
                 }
             }

             return texturePaths;
         }
    }
}
