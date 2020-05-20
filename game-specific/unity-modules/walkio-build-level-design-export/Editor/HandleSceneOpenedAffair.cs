namespace JoyBrick.Walkio.Build.LevelDesignExport.EditorPart
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    //
    using Common = JoyBrick.Walkio.Common;
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
                Debug.Log($"Build - LevelDesignExport - HandleSceneOpenedAffair - Setup - Opened Scene: {scene.name}");
                
                // TODO: Apply this to all master scenes rather than Level 001
                // if (String.CompareOrdinal(scene.name, "Level 001 - Master") == 0)
                if (scene.name.Contains("Master"))
                {
                    currentMasterScene = scene;
                    loadedSubSceneCount = 0;
                    HandleMasterSceneOpened(scene, mode);
                }
                // if (scene.isSubScene)
                else
                {
                    var levelOperator = Common.Utility.GetComponentAtScene<LevelOperator>(currentMasterScene);
                    if (levelOperator != null)
                    {
                        Debug.Log($"Build - LevelDesignExport - HandleSceneOpenedAffair - s.name: {scene.name} scene name: {scene.name}");
                        var existed = levelOperator.subScenes.Exists(s => String.CompareOrdinal(s.name, scene.name) == 0);
                        if (existed)
                        {
                            loadedSubSceneCount += 1;
                        }

                        if (loadedSubSceneCount == levelOperator.subScenes.Count)
                        {
                            // All sub scenes are loaded
                            Debug.Log($"Build - LevelDesignExport - HandleSceneOpenedAffair - All sub scenes of {currentMasterScene.name} are loaded");

                            var doGeneration = false;
                            var levelOverallAffairAssetPath = Path.Combine("Assets", "_", "1 - Game - Level Design",
                                "Module - Environment - Level", "Level Overall Affair.asset");
                            var levelOverallAffair = AssetDatabase.LoadAssetAtPath<LevelOverallAffair>(levelOverallAffairAssetPath);
                            if (levelOverallAffair != null)
                            {
                                doGeneration = levelOverallAffair.doGeneration;
                            }

                            if (doGeneration)
                            {
                                var index =
                                    levelOverallAffair.masterSceneNames.FindIndex(x =>
                                        String.Compare(x, currentMasterScene.name, StringComparison.Ordinal) == 0);

                                var levelName = string.Empty;
                                if (index >= 0)
                                {
                                    levelName = levelOverallAffair.masterSceneToLevelNames[index];
                                }
                                
                                //
                                ProcessEachPart(levelName);
                            }
                            
                            if (levelOverallAffair != null)
                            {
                                levelOverallAffair.doGeneration = false;
                            }
                        }
                    }
                }
            };
        }

        private static void ProcessEachPart(string levelName)
        {
            //
            CreateWaypointPathPart(currentMasterScene);
            var texturePaths =
                CreateObstacleGridPart(levelName, currentMasterScene);
            var aStarGraphDataPath =
                CreateAStarGraphPart("Level 001", levelName, currentMasterScene);

            var spawnPointList = CreateSpawnPointList(currentMasterScene);

            // Should actually gather everything from the previous
            CreateLevelSettingPart(
                currentMasterScene,
                spawnPointList,
                texturePaths,
                aStarGraphDataPath);
        }

        private static void HandleMasterSceneOpened(Scene scene, OpenSceneMode mode)
        {
            var rootGameObjects = scene.GetRootGameObjects();
            // var levelOperator = GetLevelOperatorAtScene(scene);
            var levelOperator = Common.Utility.GetComponentAtScene<LevelOperator>(scene);
            if (levelOperator == null) return;
            
            LoadSubScenes(levelOperator.subScenes);
        }

        private static void LoadSubScenes(List<SceneAsset> subScenes)
        {
            subScenes.ForEach(x =>
            {
                //
                var assetPath = AssetDatabase.GetAssetPath(x);
                // Debug.Log(assetPath);
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(assetPath, OpenSceneMode.Additive);
            });
        }
    }
}
