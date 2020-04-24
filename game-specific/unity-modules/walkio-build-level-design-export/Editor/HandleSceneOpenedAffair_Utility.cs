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
        private static T GetComponentAtScene<T>(Scene? scene) where T : UnityEngine.Component
        {
            T comp = default;

            var foundGOs =
                scene?.GetRootGameObjects()
                    .Where(x => x.GetComponent<T>() != null)
                    .ToList();

            if (foundGOs != null && foundGOs.Any())
            {
                var foundGO = foundGOs.First();
                comp = foundGO.GetComponent<T>();
            }

            return comp;
        }
        
        // private static GameObject AttachComponentToNewGameObject<T>(T[] comps)
        //     where T : UnityEngine.Component
        // {
        //     var createdInstance = new GameObject();
        //     comps.ToList().ForEach(c => createdInstance.AddComponent(typeof(T)));
        //     // createdInstance.AddComponent<GameEnvironment.WaypointPathBlobAssetAuthoring>();
        //     // createdInstance.AddComponent<ConvertToEntity>();
        //     
        //     return createdInstance;
        // }
        
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

            Utility.CreateDirectoryIfNotExisted(absoluteAssetPath);
            var completeAssetPath = Path.Combine(relativeAssetPath, assetName);
            
            AssetDatabase.CreateAsset(asset, completeAssetPath);
            AssetDatabase.SaveAssets();
        }
        
        private static void SaveGameObjectAsPrefabTo(
            string absoluteStartingPath,
            string relativeStartingPath,
            string assetDirectoryPath,
            string gameObjectName,
            GameObject inGO)
        {
            var absoluteAssetPath = Path.Combine(absoluteStartingPath, assetDirectoryPath);
            var relativeAssetPath = Path.Combine(relativeStartingPath, assetDirectoryPath);

            Utility.CreateDirectoryIfNotExisted(absoluteAssetPath);
            var completeGameObjectPath = Path.Combine(relativeAssetPath, gameObjectName);
            
            completeGameObjectPath = AssetDatabase.GenerateUniqueAssetPath(completeGameObjectPath);
            PrefabUtility.SaveAsPrefabAssetAndConnect(inGO, completeGameObjectPath, InteractionMode.AutomatedAction);
        }
    }
}
