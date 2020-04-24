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
                
                // TODO: Apply this to all master scenes rather than Level 001
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
                            CreateLevelSettingPart(currentMasterScene);
                            CreateWaypointPathPart(currentMasterScene);
                            CreateObstacleGridPart(currentMasterScene);
                            CreateAStarGraphPart(currentMasterScene);
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
    }
}
