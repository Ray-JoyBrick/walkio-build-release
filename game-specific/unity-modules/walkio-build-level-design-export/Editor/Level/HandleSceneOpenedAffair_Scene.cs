namespace JoyBrick.Walkio.Build.LevelDesignExport.EditorPart
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Unity.Entities;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;

    public static partial class HandleSceneOpenedAffair
    {
        private static List<string> CreateScenePart(string levelName, Scene masterScene)
        {
            var sceneNames = CreateScenes(levelName, masterScene);

            return sceneNames;
        }

        private static List<string> CreateScenes(string levelName, Scene masterScene)
        {
            var levelOperator = GameCommon.Utility.SceneHelper.GetComponentAtScene<LevelOperator>(masterScene);

            if (levelOperator == null) return null;

            var subScenes = levelOperator.subScenes;

            var relativePathToMasterScene = masterScene.path.Replace(Application.dataPath, "Assets");
            var absolutePathToMasterScene = relativePathToMasterScene.Replace("Assets", Application.dataPath);

            var absoluteSceneFolder = Path.GetDirectoryName(masterScene.path);
            var relativeSceneFolder = absoluteSceneFolder.Replace(Application.dataPath, "Assets");
            
            Debug.Log($"CreateScenes - relativePathToMasterScene: {relativePathToMasterScene}");
            Debug.Log($"CreateScenes - absolutePathToMasterScene: {absolutePathToMasterScene}");
            Debug.Log($"CreateScenes - absoluteSceneFolder: {absoluteSceneFolder}");
            Debug.Log($"CreateScenes - relativeSceneFolder: {relativeSceneFolder}");
            
            //
            var sceneNames = new List<string>();
            subScenes.ForEach(sceneAsset =>
            {
                // var sourcePath = Path.Combine(relativeSceneFolder, $"{sceneAsset.name}.unity");
                var sourcePath = Path.Combine("Assets", "_", "1 - Game - Level Design",
                    "Module - Environment - Level", levelName, "Scenes", $"{sceneAsset.name}.unity");
                var targetPath = Path.Combine("Assets", "_", "1 - Game - Level Design - Generated",
                    "Module - Environment - Level",
                    "Levels", levelName, "scenes", $"{sceneAsset.name}.unity");
                
                Debug.Log($"CreateScenes - sourcePath: {sourcePath} \n targetPath: {targetPath}");
    
                //
                AssetDatabase.CopyAsset(sourcePath, targetPath);
                
                sceneNames.Add(sceneAsset.name);
            });

            return sceneNames;
        }
    }
}
