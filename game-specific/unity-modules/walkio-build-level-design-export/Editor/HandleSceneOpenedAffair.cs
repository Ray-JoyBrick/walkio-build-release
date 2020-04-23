namespace JoyBrick.Walkio.Build.LevelDesignExport.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;

    // TODO: Rename, this class name brings only confusion
    [InitializeOnLoad]
    public static partial class HandleSceneOpenedAffair
    {
        public static Scene currentMasterScene;
        public static int loadedSubSceneCount;
        
        [InitializeOnLoadMethod]
        public static void Setup()
        {
            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                Debug.Log($"HandleSceneOpenedAffair - Setup - Opened Scene: {scene.name}");
                
                if (String.CompareOrdinal(scene.name, "Level 001 - Master") == 0)
                {
                    currentMasterScene = scene;
                    loadedSubSceneCount = 0;
                    HandleMasterSceneOpened(scene, mode);
                }
                // if (scene.isSubScene)
                else
                {
                    var levelOperator = GetComponentAtScene<LevelOperator>(currentMasterScene);
                    if (levelOperator != null)
                    {
                        Debug.Log($"s.name: {scene.name} scene name: {scene.name}");
                        var existed = levelOperator.subScenes.Exists(s => String.CompareOrdinal(s.name, scene.name) == 0);
                        if (existed)
                        {
                            loadedSubSceneCount += 1;
                        }

                        if (loadedSubSceneCount == levelOperator.subScenes.Count)
                        {
                            // All sub scenes are loaded
                            Debug.Log($"All sub scenes of {currentMasterScene.name} are loaded");
                            
                            //
                            CreateWaypointPathPart(currentMasterScene);

                            CreateObstacleTexture(currentMasterScene);
                            MakeAStarPathfindingData(currentMasterScene);
                        }
                    }
                }
            };
        }

        private static void HandleMasterSceneOpened(Scene scene, OpenSceneMode mode)
        {
            var rootGameObjects = scene.GetRootGameObjects();
            // var levelOperator = GetLevelOperatorAtScene(scene);
            var levelOperator = GetComponentAtScene<LevelOperator>(scene);
            if (levelOperator == null) return;
            
            LoadSubScenes(levelOperator.subScenes);
        }

        private static void LoadSubScenes(List<SceneAsset> subScenes)
        {
            subScenes.ForEach(x =>
            {
                //
                var assetPath = AssetDatabase.GetAssetPath(x);
                Debug.Log(assetPath);
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(assetPath, OpenSceneMode.Additive);
            });
        }

        private static void MakeAStarPathfindingData(Scene masterScene)
        {
            Debug.Log($"MakeAStarPathfindingData");
            // var astarPath = GetPathfinderAtScene(masterScene);
            // if (astarPath == null) return;
            //
            // astarPath
            
            AstarPath.active.Scan();
        }

        private static void CreateObstacleTexture(Scene masterScene)
        {
            var levelOperator = GetComponentAtScene<LevelOperator>(masterScene);
            if (levelOperator == null) return;

            for (var i = 0; i < levelOperator.subScenes.Count; ++i)
            {
                var sceneAsset = levelOperator.subScenes[i];
                var subScene = EditorSceneManager.GetSceneByName(sceneAsset.name);
                
                // levelOperator.zSubSceneCount
                // levelOperator.xSubSceneCount
                
                var basePosition = new Vector3(0, 0, 0);
                HandleObstacleForSubScene(i, subScene, basePosition);
            }
        }

        private static void HandleObstacleForSubScene(int index, Scene subScene, Vector3 basePosition)
        {
            Debug.Log($"HandleObstacleForSubScene - {subScene}");
            
             var environmentRoot = GetComponentAtScene<EnvironmentRoot>(subScene);
             if (environmentRoot == null) return;

             var floorRoot = GetComponentAtScene<FloorRoot>(subScene);
             if (floorRoot == null) return;
             
             // Store the original state
             var floorRootActive = floorRoot.isActiveAndEnabled;
             var environmentRootActive = environmentRoot.isActiveAndEnabled;
             
             // Test floor first
             floorRoot.gameObject.SetActive(true);
             environmentRoot.gameObject.SetActive(false);

             var floorTexture = MakeTextureBasedOnFloor(basePosition);
             
             // Test Environment second
             environmentRoot.gameObject.SetActive(true);
             floorRoot.gameObject.SetActive(false);
             
             var environmentTexture = MakeTextureBasedOnEnvironment(basePosition);

             // floorTexture.EncodeToPNG();
             
             // Restore the state
             floorRoot.gameObject.SetActive(floorRootActive);
             environmentRoot.gameObject.SetActive(environmentRootActive);

             var combinedTexture = CombineObstacleTexture(floorTexture, environmentTexture);
             
             // Save the texture
             var generatedDirectoryPath = Path.Combine(Application.dataPath, "_", "1 - Game - Level Design - Generated");
             var levelDirectoryPath = Path.Combine(generatedDirectoryPath, "Levels");
             CreateDirectoryIfNotExisted(generatedDirectoryPath);
             CreateDirectoryIfNotExisted(levelDirectoryPath);
            
             var level001DirectoryPath = Path.Combine(levelDirectoryPath, "level001");
             CreateDirectoryIfNotExisted(level001DirectoryPath);

             var obstacleTextureDirectoryPath = Path.Combine(level001DirectoryPath, "obstacle-texture");
             CreateDirectoryIfNotExisted(obstacleTextureDirectoryPath);
             
             var obstacleTextureAssetPath = Path.Combine(obstacleTextureDirectoryPath, $"obstacle{index:0000}.png");

             var bytes = combinedTexture.EncodeToPNG();
             File.WriteAllBytes(obstacleTextureAssetPath, bytes);
             AssetDatabase.Refresh();
             
             var obstacleTextureAssetLevelPath = Path.Combine("Assets", "_", "1 - Game - Level Design - Generated",
                 "Levels", "level001", "obstacle-texture");
             var relativeObstacleTextureAssetPath = Path.Combine(obstacleTextureAssetLevelPath, $"obstacle{index:0000}.png");
             
             var textureImporter = AssetImporter.GetAtPath(relativeObstacleTextureAssetPath) as TextureImporter;

             textureImporter.filterMode = FilterMode.Point;
             textureImporter.maxTextureSize = 64;

             textureImporter.mipmapEnabled = false;
             
             EditorUtility.SetDirty(textureImporter);
             textureImporter.SaveAndReimport();

             //
             // AssetDatabase.CreateAsset(floorTexture, obstacleTextureAssetPath);
             // AssetDatabase.SaveAssets();              
        }

        private static Texture2D MakeTextureBasedOnFloor(Vector3 basePosition)
        {
            var texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);

            // RaycastHit[] An array of RaycastHit objects. Note that the order of the results is undefined. 
            // In such case, the layer has to be turned on or off and merge the result
            for (var z = 0; z < 64; ++z)
            {
                for (var x = 0; x < 64; ++x)
                {
                    var adjustedX = basePosition.x + x + 0.5f;
                    var adjustedZ = basePosition.z + z + 0.5f;
                     
                    var raycastHits =
                        Physics.RaycastAll(
                            // new Ray(new Vector3(0.5f, 100.0f, 0.5f), Vector3.down),
                            new Ray(new Vector3(adjustedX, 100.0f, adjustedZ), Vector3.down),
                            150.0f);

                    var castedRaycastHits = raycastHits.ToList();
                    if (castedRaycastHits.Count > 0)
                    {
                        // Use the first one as there should be one
                        var floorObject = castedRaycastHits[0];
                        // floorObject.collider
                        var color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        texture.SetPixel(x, z, color);
                    }
             
                    // raycastHits.ToList().ForEach(rh =>
                    // {
                    //     //
                    //     Debug.Log($"hit: {rh.collider.name}");
                    // });
                }
            }

            return texture;
        }

        private static Texture2D MakeTextureBasedOnEnvironment(Vector3 basePosition)
        {
            var texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);

            // RaycastHit[] An array of RaycastHit objects. Note that the order of the results is undefined. 
            // In such case, the layer has to be turned on or off and merge the result
            for (var z = 0; z < 64; ++z)
            {
                for (var x = 0; x < 64; ++x)
                {
                    var adjustedX = basePosition.x + x + 0.5f;
                    var adjustedZ = basePosition.z + z + 0.5f;
                     
                    var raycastHits =
                        Physics.RaycastAll(
                            // new Ray(new Vector3(0.5f, 100.0f, 0.5f), Vector3.down),
                            new Ray(new Vector3(adjustedX, 100.0f, adjustedZ), Vector3.down),
                            150.0f);

                    var castedRaycastHits = raycastHits.ToList();
                    if (castedRaycastHits.Count > 0)
                    {
                        // Use the first one as there should be one
                        var floorObject = castedRaycastHits[0];
                        // floorObject.collider
                        var color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                        texture.SetPixel(x, z, color);
                    }
                    else
                    {
                        var color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        texture.SetPixel(x, z, color);
                    }
                }
            }

            return texture;
        }

        private static Texture2D CombineObstacleTexture(Texture2D floorTexture, Texture2D environmentTexture)
        {
            var width = floorTexture.width;
            var height = floorTexture.height;
            
            var texture = new Texture2D(width, height, floorTexture.format, false);

            for (var h = 0; h < height; ++h)
            {
                for (var w = 0; w < width; ++w)
                {
                    var floorColor = floorTexture.GetPixel(w, h);
                    var environment = environmentTexture.GetPixel(w, h);
                    
                    // Just use floor texture for now

                    texture.SetPixel(w, h, floorColor);
                }
            }

            return texture;
        }
    }
}
