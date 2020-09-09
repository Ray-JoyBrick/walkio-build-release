namespace JoyBrick.Walkio.Tool.LevelDesign.EditorPart
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
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameLevel = JoyBrick.Walkio.Game.Level;

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
                Debug.Log($"Game - Assist - HandleSceneOpenedAffair - Setup - Opened Scene: {scene.name}");

                if (scene.name.Contains("Level 005 - Main"))
                {
                    currentMasterScene = scene;
                    loadedSubSceneCount = 0;
                    HandleMasterSceneOpened(scene, mode);
                }
                else
                {
                    var levelOperator = GameCommon.Utility.SceneHelper.GetComponentAtScene<GameLevel.LevelOperator>(currentMasterScene);
                    if (levelOperator != null)
                    {
                        Debug.Log($"Game - Assist - HandleSceneOpenedAffair - s.name: {scene.name} scene name: {scene.name}");
                        var existed = levelOperator.subScenes.Exists(s => String.CompareOrdinal(s.name, scene.name) == 0);
                        if (existed)
                        {
                            loadedSubSceneCount += 1;
                        }

                        if (loadedSubSceneCount == levelOperator.subScenes.Count)
                        {
                            // All sub scenes are loaded
                            Debug.Log($"Game - Assist - HandleSceneOpenedAffair - All sub scenes of {currentMasterScene.name} are loaded");
                        }
                    }
                }
            };
        }

        private static void HandleMasterSceneOpened(Scene scene, OpenSceneMode mode)
        {
            var rootGameObjects = scene.GetRootGameObjects();
            // var levelOperator = GameCommon.Utility.SceneHelper.GetComponentAtScene<GameLevel.LevelOperator>(scene);
            var levelOperator = GameCommon.Utility.SceneHelper.GetComponentAtScene<LevelOperator>(scene);
            if (levelOperator == null) return;

            LoadSubScenes(levelOperator.subScenes);
        }

        private static void LoadSubScenes(List<SceneAsset> subScenes)
        {
            subScenes.ForEach(x =>
            {
                //
                var assetPath = AssetDatabase.GetAssetPath(x);
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(assetPath, OpenSceneMode.Additive);
            });
        }
    }
}
