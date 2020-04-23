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
    public static partial class HandleSceneOpenedAffair
    {
        private static T GetComponentAtScene<T>(Scene scene) where T : UnityEngine.Component 
        {
            var foundGO =
                scene.GetRootGameObjects()
                    .Where(x => x.GetComponent<T>() != null)
                    .First();

            var comp  = foundGO.GetComponent<T>();

            return comp;
        }
        
        private static void CreateDirectoryIfNotExisted(string directoryPath)
        {
            var existed = Directory.Exists(directoryPath);
            if (!existed)
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
        
        private static void SaveAssetTo<T>(
            string absoluteStartingPath,
            string relativeStartingPath,
            string assetDirectoryPath,
            string assetName,
            T asset)
            where T : UnityEngine.Object
        {
            var absoluteAssetPath = Path.Combine(absoluteStartingPath, assetDirectoryPath);
            var relativeAssetPath = Path.Combine(relativeStartingPath, assetDirectoryPath);

            CreateDirectoryIfNotExisted(absoluteAssetPath);
            var completeAssetPath = Path.Combine(relativeAssetPath, assetName);
            
            AssetDatabase.CreateAsset(asset, completeAssetPath);
            AssetDatabase.SaveAssets();
        }
        
        private static void SaveGameObjectAsPrefabTo(
            string absoluteStartingPath,
            string relativeStartingPath,
            string assetDirectoryPath,
            string gameObjecttName,
            GameObject inGO)
        {
            var absoluteAssetPath = Path.Combine(absoluteStartingPath, assetDirectoryPath);
            var relativeAssetPath = Path.Combine(relativeStartingPath, assetDirectoryPath);

            CreateDirectoryIfNotExisted(absoluteAssetPath);
            var completeGameObjectPath = Path.Combine(relativeAssetPath, gameObjecttName);
            
            completeGameObjectPath = AssetDatabase.GenerateUniqueAssetPath(completeGameObjectPath);
            PrefabUtility.SaveAsPrefabAssetAndConnect(inGO, completeGameObjectPath, InteractionMode.AutomatedAction);
        }
    }
}
