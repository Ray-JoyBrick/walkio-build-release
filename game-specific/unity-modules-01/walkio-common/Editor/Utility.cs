namespace JoyBrick.Walkio.Common.EditorPart
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    //
    using Common = JoyBrick.Walkio.Common;

    public static partial class Utility
    {
        public static void SaveAssetTo<T>(
            string absoluteStartingPath,
            string relativeStartingPath,
            string assetDirectoryPath,
            string assetName,
            T asset)
            where T : UnityEngine.Object
        {
            var absoluteAssetPath = Path.Combine(absoluteStartingPath, assetDirectoryPath);
            var relativeAssetPath = Path.Combine(relativeStartingPath, assetDirectoryPath);

            Common.Utility.CreateDirectoryIfNotExisted(absoluteAssetPath);
            var completeAssetPath = Path.Combine(relativeAssetPath, assetName);
            
            AssetDatabase.CreateAsset(asset, completeAssetPath);
            AssetDatabase.SaveAssets();
        }

        public static void SaveGameObjectAsPrefabTo(
            string absoluteStartingPath,
            string relativeStartingPath,
            string assetDirectoryPath,
            string gameObjectName,
            GameObject inGO)
        {
            var absoluteAssetPath = Path.Combine(absoluteStartingPath, assetDirectoryPath);
            var relativeAssetPath = Path.Combine(relativeStartingPath, assetDirectoryPath);

            Common.Utility.CreateDirectoryIfNotExisted(absoluteAssetPath);
            var completeGameObjectPath = Path.Combine(relativeAssetPath, gameObjectName);
            
            completeGameObjectPath = AssetDatabase.GenerateUniqueAssetPath(completeGameObjectPath);
            PrefabUtility.SaveAsPrefabAssetAndConnect(inGO, completeGameObjectPath, InteractionMode.AutomatedAction);
        }
    }
}
