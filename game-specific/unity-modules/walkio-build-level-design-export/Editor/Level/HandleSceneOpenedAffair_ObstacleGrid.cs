// namespace JoyBrick.Walkio.Build.LevelDesignExport.EditorPart
// {
//     using System;
//     using System.Collections.Generic;
//     using System.IO;
//     using System.Linq;
//     using Unity.Entities;
//     using UnityEditor;
//     using UnityEditor.SceneManagement;
//     using UnityEngine;
//     using UnityEngine.SceneManagement;
//
//     //
//     using GameCommon = JoyBrick.Walkio.Game.Common;
//     using GameEnvironment = JoyBrick.Walkio.Game.Environment;
//
//     public static partial class HandleSceneOpenedAffair
//     {
//         private static IEnumerable<string> CreateObstacleGridPart(string levelName, Scene masterScene)
//         {
//             var texturePaths = CreateObstacleTexture(levelName, masterScene);
//
//             return texturePaths;
//         }
//
//         // private static void CreateObstacleGridPart_AuthoringPrefab(Scene masterScene)
//         // {
//         //     var gameObject = CreateGridMapBlobAssetAuthoringGameObject();
//         //     var gridMapBlobAssetAuthoring =
//         //         gameObject.GetComponent<GameEnvironment.GridMapBlobAssetAuthoring>();
//         //
//         //     if (gridMapBlobAssetAuthoring != null)
//         //     {
//         //         // waypointPathBlobAssetAuthoring.waypointDataAsset = waypointDataAsset;
//         //
//         //         var absoluteStartingPath = Application.dataPath;
//         //         var relativeStartingPath = "Assets";
//         //         var assetDirectoryPath = Path.Combine("_", "1 - Game - Level Design - Generated",
//         //             "Levels", "level001", "waypoint-path");
//         //
//         //         //
//         //         {
//         //             var assetName = "Waypoint Data.asset";
//         //
//         //             SaveAssetTo(absoluteStartingPath, relativeStartingPath, assetDirectoryPath, assetName, waypointDataAsset);
//         //         }
//         //         //
//         //         {
//         //             var prefabName = "Waypoint Path BlobAsset Authoring.prefab";
//         //
//         //             SaveGameObjectAsPrefabTo(absoluteStartingPath, relativeStartingPath, assetDirectoryPath, prefabName, gameObject);
//         //             // Remove game object from scene after saving
//         //             GameObject.DestroyImmediate(gameObject);
//         //             EditorSceneManager.SaveScene(currentMasterScene);
//         //         }
//         //     }
//         // }
//
//         private static IEnumerable<string> CreateObstacleTexture(string levelName, Scene masterScene)
//         {
//             var levelOperator = GameCommon.Utility.SceneHelper.GetComponentAtScene<LevelOperator>(masterScene);
//             if (levelOperator == null) return Enumerable.Empty<string>();
//
//             var texturePaths = new List<string>();
//             for (var hSubSceneIndex = 0; hSubSceneIndex < levelOperator.zSubSceneCount; ++hSubSceneIndex)
//             {
//                 for (var wSubSceneIndex = 0; wSubSceneIndex < levelOperator.xSubSceneCount; ++wSubSceneIndex)
//                 {
//                     var i = hSubSceneIndex * levelOperator.xSubSceneCount + wSubSceneIndex;
//
//                     var sceneAsset = levelOperator.subScenes[i];
//                     var subScene = EditorSceneManager.GetSceneByName(sceneAsset.name);
//
//                     // var basePosition = new Vector3(
//                     //     wSubSceneIndex * levelOperator.gridCount,
//                     //     0,
//                     //     hSubSceneIndex * levelOperator.gridCount);
//                     var basePosition = new Vector3(
//                         wSubSceneIndex * levelOperator.gridCount,
//                         0,
//                         hSubSceneIndex * levelOperator.gridCount);
//                     var texturePath =
//                         HandleObstacleForSubScene(
//                             levelName,
//                             i, subScene, basePosition,
//                             levelOperator.gridCount,
//                             levelOperator.gridCount);
//
//                     texturePaths.Add(texturePath);
//                 }
//             }
//
//             return texturePaths;
//         }
//
//         private static string HandleObstacleForSubScene(
//             string levelName,
//             int index,
//             Scene subScene,
//             Vector3 basePosition,
//             int wCellCount,
//             int hCellCount)
//         {
//             Debug.Log($"HandleObstacleForSubScene - {subScene}");
//
//              // var inBoundaryFloorTexture = MakeTextureBasedOnInBoundaryFloor(wCellCount, hCellCount, basePosition);
//              // var outBoundaryFloorTexture = MakeTextureBasedOnOutBoundaryFloor(wCellCount, hCellCount, basePosition);
//              // var obstacleTexture = MakeTextureBasedOnObstacle(wCellCount, hCellCount, basePosition);
//              // var areaTexture = MakeTextureBasedOnArea(wCellCount, hCellCount, basePosition);
//
//              var groundBaseTexture = MakeTextureBasedOnLayer(wCellCount, hCellCount, basePosition, "Ground Base",
//                  new Color32(0, 0, 0, 255), new Color32(0, 0, 0, 255));
//              var obstacleTexture = MakeTextureBasedOnLayer(wCellCount, hCellCount, basePosition, "Obstacle",
//                  new Color32(200, 200, 200, 255), new Color32(0, 0, 0, 255));
//
//              var combinedTexture = CombineTextures(new List<Texture2D>
//              {
//                  // inBoundaryFloorTexture,
//                  // outBoundaryFloorTexture,
//                  groundBaseTexture,
//                  obstacleTexture,
//                  // areaTexture
//              });
//
//              // Save the texture
//              var generatedDirectoryPath = Path.Combine(Application.dataPath, "_", "1 - Game - Level Design - Generated");
//              var levelDirectoryPath = Path.Combine(generatedDirectoryPath, "Module - Environment - Level", "Levels");
//              GameCommon.Utility.FileHelper.CreateDirectoryIfNotExisted(generatedDirectoryPath);
//              GameCommon.Utility.FileHelper.CreateDirectoryIfNotExisted(levelDirectoryPath);
//
//              var specificLevelDirectoryPath = Path.Combine(levelDirectoryPath, levelName);
//              GameCommon.Utility.FileHelper.CreateDirectoryIfNotExisted(specificLevelDirectoryPath);
//
//              var obstacleTextureDirectoryPath = Path.Combine(specificLevelDirectoryPath, "obstacle-texture");
//              GameCommon.Utility.FileHelper.CreateDirectoryIfNotExisted(obstacleTextureDirectoryPath);
//
//              var obstacleTextureAssetPath = Path.Combine(obstacleTextureDirectoryPath, $"obstacle{index:0000}.png");
//
//              var bytes = combinedTexture.EncodeToPNG();
//              File.WriteAllBytes(obstacleTextureAssetPath, bytes);
//              AssetDatabase.Refresh();
//
//              var obstacleTextureAssetLevelPath = Path.Combine("Assets", "_", "1 - Game - Level Design - Generated",
//                  "Module - Environment - Level",
//                  "Levels", levelName, "obstacle-texture");
//              var relativeObstacleTextureAssetPath = Path.Combine(obstacleTextureAssetLevelPath, $"obstacle{index:0000}.png");
//
//              var textureImporter = AssetImporter.GetAtPath(relativeObstacleTextureAssetPath) as TextureImporter;
//
//              if (textureImporter != null)
//              {
//                  textureImporter.filterMode = FilterMode.Point;
//
//                  // Check this discussion to know how to get the next power of 2 number for
//                  // any given number
//                  // https://stackoverflow.com/questions/466204/rounding-up-to-next-power-of-2
//                  var nextPowerOfTwo = Mathf.Pow(2, Mathf.Ceil((Mathf.Log(wCellCount) / Mathf.Log(2))));
//                  textureImporter.maxTextureSize = (int)nextPowerOfTwo;
//
//                  textureImporter.mipmapEnabled = false;
//
//                  textureImporter.isReadable = true;
//
//                  EditorUtility.SetDirty(textureImporter);
//                  textureImporter.SaveAndReimport();
//              }
//
//              return relativeObstacleTextureAssetPath;
//         }
//
//         private static Texture2D MakeTextureBasedOnLayer(
//             int width, int height, Vector3 basePosition,
//             string layerName, Color32 hitColor, Color32 noHitColor)
//         {
//             Debug.Log($"HandleSceneOpenedAffair - MakeTextureBasedOnLayer - layerName: {layerName}");
//
//             var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
//             var colors = new Color32[width * height];
//
//             for (var z = 0; z < height; ++z)
//             {
//                 for (var x = 0; x < width; ++x)
//                 {
//                     var adjustedX = basePosition.x + x + 0.5f;
//                     var adjustedZ = basePosition.z + z + 0.5f;
//
//                     var w = adjustedX;
//                     var h = adjustedZ;
//                     var tx = x;
//                     var ty = z;
//
//                     var colorIndex = ty * width + tx;
//
//                     var layerMask = LayerMask.GetMask(layerName);
//                     var raycastHits =
//                         Physics.RaycastAll(
//                             new Ray(new Vector3(w, 100.0f, h), Vector3.down),
//                             150.0f, layerMask);
//
//                     var castedRaycastHits = raycastHits.ToList();
//                     if (castedRaycastHits.Count > 0)
//                     {
//                         // Debug.Log($"HandleSceneOpenedAffair - MakeTextureBasedOnInBoundaryFloor - width: {width} height: {height} is in boundary floor");
//
//                         // Use the first one as there should be one
//                         var raycastHit = castedRaycastHits[0];
//
//                         // var color = new Color32(0, 0, 0, 255);
//
//                         colors[colorIndex] = hitColor;
//                     }
//                     else
//                     {
//                         // var color = new Color32(0, 0, 0, 255);
//
//                         colors[colorIndex] = noHitColor;
//                     }
//                 }
//             }
//
//             texture.SetPixels32(colors);
//
//             return texture;
//         }
//
//         //
//         private static Texture2D MakeTextureBasedOnInBoundaryFloor(int width, int height, Vector3 basePosition)
//         {
//             Debug.Log($"HandleSceneOpenedAffair - MakeTextureBasedOnInBoundaryFloor");
//
//             var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
//
//             var colors = new Color32[width * height];
//
//             for (var z = 0; z < height; ++z)
//             {
//                 for (var x = 0; x < width; ++x)
//                 {
//                     var adjustedX = basePosition.x + x + 0.5f;
//                     var adjustedZ = basePosition.z + z + 0.5f;
//
//                     var w = adjustedX;
//                     var h = adjustedZ;
//                     var tx = x;
//                     var ty = z;
//
//                     var colorIndex = ty * width + tx;
//
//                     // action(width, height, adjustedX, adjustedZ, basePosition, texture);
//                     var layerMask = LayerMask.GetMask("In Boundary Floor");
//                     var raycastHits =
//                         Physics.RaycastAll(
//                             // new Ray(new Vector3(0.5f, 100.0f, 0.5f), Vector3.down),
//                             new Ray(new Vector3(w, 100.0f, h), Vector3.down),
//                             150.0f, layerMask);
//
//                     var castedRaycastHits = raycastHits.ToList();
//                     if (castedRaycastHits.Count > 0)
//                     {
//                         // Debug.Log($"HandleSceneOpenedAffair - MakeTextureBasedOnInBoundaryFloor - width: {width} height: {height} is in boundary floor");
//
//                         // Use the first one as there should be one
//                         var raycastHit = castedRaycastHits[0];
//                         // var color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
//                         // texture.SetPixel(tx, ty, color);
//
//                         var color = new Color32(0, 0, 0, 255);
//
//                         colors[colorIndex] = color;
//                     }
//                     else
//                     {
//                         // This case should not occur
//                         // var color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
//                         // texture.SetPixel(tx, ty, color);
//
//                         var color = new Color32(0, 0, 0, 255);
//
//                         colors[colorIndex] = color;
//                     }
//                 }
//             }
//
//             texture.SetPixels32(colors);
//
//             // ApplyActionToEachCell(width, height, basePosition, ref texture, (tx, ty, w, h, p, t) =>
//             // {
//             // });
//
//             return texture;
//         }
//
//         private static void ApplyActionToEachCell(
//             int width, int height, Vector3 basePosition, ref Texture2D texture,
//             System.Action<int, int, float, float, Vector3, Texture2D> action)
//         {
//             for (var z = 0; z < height; ++z)
//             {
//                 for (var x = 0; x < width; ++x)
//                 {
//                     var adjustedX = basePosition.x + x + 0.5f;
//                     var adjustedZ = basePosition.z + z + 0.5f;
//
//                     action(width, height, adjustedX, adjustedZ, basePosition, texture);
//                 }
//             }
//         }
//
//         private static Texture2D MakeTextureBasedOnOutBoundaryFloor(int width, int height, Vector3 basePosition)
//         {
//             var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
//
//             var colors = new Color32[width * height];
//
//             for (var z = 0; z < height; ++z)
//             {
//                 for (var x = 0; x < width; ++x)
//                 {
//                     var adjustedX = basePosition.x + x + 0.5f;
//                     var adjustedZ = basePosition.z + z + 0.5f;
//
//                     var w = adjustedX;
//                     var h = adjustedZ;
//                     var tx = x;
//                     var ty = z;
//
//                     var colorIndex = ty * width + tx;
//
//                     var layerMask = LayerMask.GetMask("Out Boundary Floor");
//                     var raycastHits =
//                         Physics.RaycastAll(
//                             // new Ray(new Vector3(0.5f, 100.0f, 0.5f), Vector3.down),
//                             new Ray(new Vector3(w, 100.0f, h), Vector3.down),
//                             150.0f, layerMask);
//
//                     var castedRaycastHits = raycastHits.ToList();
//                     if (castedRaycastHits.Count > 0)
//                     {
//                         // Use the first one as there should be one
//                         var raycastHit = castedRaycastHits[0];
//                         // var color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
//                         // texture.SetPixel(tx, ty, color);
//
//                         var color = new Color32(255, 255, 255, 255);
//
//                         colors[colorIndex] = color;
//                     }
//                     else
//                     {
//                         // var color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
//                         // texture.SetPixel(tx, ty, color);
//                         var color = new Color32(0, 0, 0, 255);
//
//                         colors[colorIndex] = color;
//                     }
//                 }
//             }
//
//             texture.SetPixels32(colors);
//
//             // ApplyActionToEachCell(width, height, basePosition, ref texture, (tx, ty, w, h, p, t) =>
//             // {
//             //
//             // });
//
//             return texture;
//         }
//
//         private static Texture2D MakeTextureBasedOnObstacle(int width, int height, Vector3 basePosition)
//         {
//             var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
//
//             var colors = new Color32[width * height];
//
//             for (var z = 0; z < height; ++z)
//             {
//                 for (var x = 0; x < width; ++x)
//                 {
//                     var adjustedX = basePosition.x + x + 0.5f;
//                     var adjustedZ = basePosition.z + z + 0.5f;
//
//                     var w = adjustedX;
//                     var h = adjustedZ;
//                     var tx = x;
//                     var ty = z;
//
//                     var colorIndex = ty * width + tx;
//
//                     var layerMask = LayerMask.GetMask("Obstacle");
//                     var raycastHits =
//                         Physics.RaycastAll(
//                             // new Ray(new Vector3(0.5f, 100.0f, 0.5f), Vector3.down),
//                             new Ray(new Vector3(w, 100.0f, h), Vector3.down),
//                             150.0f, layerMask);
//
//                     var castedRaycastHits = raycastHits.ToList();
//                     if (castedRaycastHits.Count > 0)
//                     {
//                         // Use the first one as there should be one
//                         var raycastHit = castedRaycastHits[0];
//                         var color = new Color32(100, 0, 0, 255);
//
//                         colors[colorIndex] = color;
//                         //texture.SetPixel(tx, ty, color);
//
//                         // Debug.Log($"width: {width}, height: {height} is obstacle");
//                     }
//                     else
//                     {
//                         // var color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
//                         var color = new Color32(0, 0, 0, 255);
//                         colors[colorIndex] = color;
//                         //texture.SetPixel(tx, ty, color);
//                     }
//                 }
//             }
//
//             texture.SetPixels32(colors);
//
//             // ApplyActionToEachCell(width, height, basePosition, ref texture, (tx, ty, w, h, p, t) =>
//             // {
//             // });
//
//             return texture;
//         }
//
//         private static Texture2D MakeTextureBasedOnArea(int width, int height, Vector3 basePosition)
//         {
//             var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
//
//             var colors = new Color32[width * height];
//
//             for (var z = 0; z < height; ++z)
//             {
//                 for (var x = 0; x < width; ++x)
//                 {
//                     var adjustedX = basePosition.x + x + 0.5f;
//                     var adjustedZ = basePosition.z + z + 0.5f;
//
//                     var w = adjustedX;
//                     var h = adjustedZ;
//                     var tx = x;
//                     var ty = z;
//
//                     var colorIndex = ty * width + tx;
//
//                     var layerMask = LayerMask.GetMask("Area");
//                     var raycastHits =
//                         Physics.RaycastAll(
//                             // new Ray(new Vector3(0.5f, 100.0f, 0.5f), Vector3.down),
//                             new Ray(new Vector3(w, 100.0f, h), Vector3.down),
//                             150.0f, layerMask);
//
//                     var castedRaycastHits = raycastHits.ToList();
//                     if (castedRaycastHits.Count > 0)
//                     {
//                         // Use the first one as there should be one
//                         var raycastHit = castedRaycastHits[0];
//
//                         // Further examine the raycastHit info to know what the object is
//
//                         // var color = new Color(0.0f, 0.2f, 0.0f, 1.0f);
//                         // texture.SetPixel(tx, ty, color);
//                         var color = new Color32(0, 110, 0, 255);
//                         colors[colorIndex] = color;
//                     }
//                     else
//                     {
//                         // var color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
//                         // texture.SetPixel(tx, ty, color);
//                         var color = new Color32(0, 0, 0, 255);
//                         colors[colorIndex] = color;
//                     }
//                 }
//             }
//
//             texture.SetPixels32(colors);
//
//             // ApplyActionToEachCell(width, height, basePosition, ref texture, (tx, ty, w, h, p, t) =>
//             // {
//             // });
//
//             return texture;
//         }
//
//         private static Texture2D CombineTextures(List<Texture2D> textures)
//         {
//             if (!textures.Any()) return null;
//
//             var width = textures.First().width;
//             var height = textures.First().height;
//
//             var texture = new Texture2D(width, height, textures.First().format, false);
//
//             var combinedColors = new Color32[width * height];
//
//             for (var tIndex = 0; tIndex < textures.Count; ++tIndex)
//             {
//                 var t = textures[tIndex];
//
//                 var colors = t.GetPixels32();
//
//                 for (var i = 0; i < combinedColors.Length; ++i)
//                 {
//                     var cc = combinedColors[i];
//                     var c = colors[i];
//
//                     var r = cc.r + c.r;
//                     var g = cc.g + c.g;
//                     var b = cc.b + c.b;
//                     var a = cc.a + c.a;
//
//                     combinedColors[i].r = (r < 255) ? (byte) r : (byte) 255;
//                     combinedColors[i].g = (r < 255) ? (byte) g : (byte) 255;
//                     combinedColors[i].b = (r < 255) ? (byte) b : (byte) 255;
//                     combinedColors[i].a = (r < 255) ? (byte) a : (byte) 255;
//
//                     //combinedColors[i].r = combinedColors[i].r + colors[i].r;
//                 }
//
//                 // var color = t.GetPixel32(w, h);
//                 // // Debug.Log($"Color of tIndex: {tIndex} - At w: {w} h: {h}, color: {color}");
//                 // combinedColor += color;
//             }
//
//             texture.SetPixels32(combinedColors);
//
//             // for (var h = 0; h < height; ++h)
//             // {
//             //     for (var w = 0; w < width; ++w)
//             //     {
//             //         var combinedColor = new Color32(0, 0, 0, 0);
//             //         for (var tIndex = 0; tIndex < textures.Count; ++tIndex)
//             //         {
//             //             var t = textures[tIndex];
//             //
//             //             var color = t.GetPixel32(w, h);
//             //             // Debug.Log($"Color of tIndex: {tIndex} - At w: {w} h: {h}, color: {color}");
//             //             combinedColor += color;
//             //         }
//             //
//             //         combinedColor.r = (combinedColor.r > 255) ? 255 : combinedColor.r;
//             //         combinedColor.g = (combinedColor.g > 255) ? 255 : combinedColor.g;
//             //         combinedColor.b = (combinedColor.b > 255) ? 255 : combinedColor.b;
//             //         combinedColor.a = (combinedColor.a > 255) ? 255 : combinedColor.a;
//             //
//             //         // Debug.Log($"CombineTextures - At w: {w} h: {h}, combinedColor: {combinedColor}");
//             //
//             //         texture.SetPixel(w, h, combinedColor);
//             //     }
//             // }
//
//             return texture;
//         }
//
//         private static Texture2D MakeTextureBasedOnFloor(Vector3 basePosition)
//         {
//             var texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
//
//             // RaycastHit[] An array of RaycastHit objects. Note that the order of the results is undefined.
//             // In such case, the layer has to be turned on or off and merge the result
//             for (var z = 0; z < 64; ++z)
//             {
//                 for (var x = 0; x < 64; ++x)
//                 {
//                     var adjustedX = basePosition.x + x + 0.5f;
//                     var adjustedZ = basePosition.z + z + 0.5f;
//
//                     var raycastHits =
//                         Physics.RaycastAll(
//                             // new Ray(new Vector3(0.5f, 100.0f, 0.5f), Vector3.down),
//                             new Ray(new Vector3(adjustedX, 100.0f, adjustedZ), Vector3.down),
//                             150.0f);
//
//                     var castedRaycastHits = raycastHits.ToList();
//                     if (castedRaycastHits.Count > 0)
//                     {
//                         // Use the first one as there should be one
//                         var floorObject = castedRaycastHits[0];
//                         // floorObject.collider
//                         var color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
//                         texture.SetPixel(x, z, color);
//                     }
//
//                     // raycastHits.ToList().ForEach(rh =>
//                     // {
//                     //     //
//                     //     Debug.Log($"hit: {rh.collider.name}");
//                     // });
//                 }
//             }
//
//             return texture;
//         }
//
//         private static Texture2D MakeTextureBasedOnEnvironment(Vector3 basePosition)
//         {
//             var texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
//
//             // RaycastHit[] An array of RaycastHit objects. Note that the order of the results is undefined.
//             // In such case, the layer has to be turned on or off and merge the result
//             for (var z = 0; z < 64; ++z)
//             {
//                 for (var x = 0; x < 64; ++x)
//                 {
//                     var adjustedX = basePosition.x + x + 0.5f;
//                     var adjustedZ = basePosition.z + z + 0.5f;
//
//                     var raycastHits =
//                         Physics.RaycastAll(
//                             // new Ray(new Vector3(0.5f, 100.0f, 0.5f), Vector3.down),
//                             new Ray(new Vector3(adjustedX, 100.0f, adjustedZ), Vector3.down),
//                             150.0f);
//
//                     var castedRaycastHits = raycastHits.ToList();
//                     if (castedRaycastHits.Count > 0)
//                     {
//                         // Use the first one as there should be one
//                         var floorObject = castedRaycastHits[0];
//                         // floorObject.collider
//                         var color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
//                         texture.SetPixel(x, z, color);
//                     }
//                     else
//                     {
//                         var color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
//                         texture.SetPixel(x, z, color);
//                     }
//                 }
//             }
//
//             return texture;
//         }
//
//         private static Texture2D CombineObstacleTexture(Texture2D floorTexture, Texture2D environmentTexture)
//         {
//             var width = floorTexture.width;
//             var height = floorTexture.height;
//
//             var texture = new Texture2D(width, height, floorTexture.format, false);
//
//             for (var h = 0; h < height; ++h)
//             {
//                 for (var w = 0; w < width; ++w)
//                 {
//                     var floorColor = floorTexture.GetPixel(w, h);
//                     var environment = environmentTexture.GetPixel(w, h);
//
//                     // Just use floor texture for now
//
//                     texture.SetPixel(w, h, floorColor);
//                 }
//             }
//
//             return texture;
//         }
//
//         private static GameObject CreateGridMapBlobAssetAuthoringGameObject()
//         {
//             // TODO: This creates new game object on scene, which has to be removed later to no alert the scene
//             var createdInstance = new GameObject();
//             createdInstance.AddComponent<GameEnvironment.GridMapBlobAssetAuthoring>();
//             createdInstance.AddComponent<ConvertToEntity>();
//
//             return createdInstance;
//         }
//     }
// }
