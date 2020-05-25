namespace JoyBrick.Walkio.Game.Common.EditorPart
{
    using System.IO;
    using UnityEditor;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

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

            GameCommon.Utility.CreateDirectoryIfNotExisted(absoluteAssetPath);
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

            GameCommon.Utility.CreateDirectoryIfNotExisted(absoluteAssetPath);
            var completeGameObjectPath = Path.Combine(relativeAssetPath, gameObjectName);
            
            completeGameObjectPath = AssetDatabase.GenerateUniqueAssetPath(completeGameObjectPath);
            PrefabUtility.SaveAsPrefabAssetAndConnect(inGO, completeGameObjectPath, InteractionMode.AutomatedAction);
        }

        public static void PlaceAssetInAddressble(
            AddressableAssetSettings assetSettings,
            AddressableAssetGroup assetGroup,
            string assetPath,
            string labelName,
            string assetAddressableName)
        {
            //
            var entryId = AssetDatabase.AssetPathToGUID(assetPath);
            var assetEntry = assetSettings.CreateOrMoveEntry(entryId, assetGroup);
            if (!string.IsNullOrEmpty(labelName))
            {
                assetEntry.SetLabel($"{labelName}", true);
            }
            assetEntry.SetAddress($"{assetAddressableName}");
        }  
    }
}
