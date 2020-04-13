namespace JoyBrick.Walkio.Build.LevelDesignExport.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEngine;

    public partial class AssetCreator
    {
        [MenuItem("Assets/Walkio/Remove/Level Asset Addressable Group")]
        public static void RemoveAddressableGroup()
        {
            var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            
            // Create group
            RemoveGroup(assetSettings, "Assets - Environment");
            // RemoveGroup(assetSettings, "Assets - Hud - App");
            // RemoveGroup(assetSettings, "Assets - Hud - Preparation");
            // RemoveGroup(assetSettings, "Assets - Hud - Stage");            
        }

        private static void RemoveGroup(AddressableAssetSettings assetSettings, string groupName)
        {
            // Clean up what is already in
            var addressableAssetGroup = assetSettings.FindGroup(groupName);
            if (addressableAssetGroup == null)
            {
                addressableAssetGroup = assetSettings.CreateGroup(groupName, false, false, false, assetSettings.DefaultGroup.Schemas);
            }

            if (addressableAssetGroup != null)
            {
                assetSettings.RemoveGroup(addressableAssetGroup);
            }
        }
        
        [MenuItem("Assets/Walkio/Create/Level Asset Addressable Group")]
        public static void CreateAddressableGroup()
        {
            var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            
            // Create group
            // var commonGroup = CreateGroup(assetSettings, "Assets - Hud - Common");
            var environmentGroup = CreateGroup(assetSettings, "Assets - Environment");
            // var preparationGroup = CreateGroup(assetSettings, "Assets - Hud - Preparation");
            // var stageGroup = CreateGroup(assetSettings, "Assets - Hud - Stage");
            
            // //
            // PlaceAssetIntoGroup_Common(assetSettings, commonGroup);
            PlaceAssetIntoGroup_Environment(assetSettings, environmentGroup);
            // PlaceAssetIntoGroup_Preparation(assetSettings, preparationGroup);
            // PlaceAssetIntoGroup_Stage(assetSettings, stageGroup);
        }

        private static AddressableAssetGroup CreateGroup(AddressableAssetSettings assetSettings, string groupName)
        {
            // Clean up what is already in
            var addressableAssetGroup = assetSettings.FindGroup(groupName);
            if (addressableAssetGroup == null)
            {
                addressableAssetGroup = assetSettings.CreateGroup(groupName, false, false, false, assetSettings.DefaultGroup.Schemas);
            }

            if (addressableAssetGroup != null)
            {
//                aasdoSettings.RemoveGroup(addressableAssetGroup);
                foreach (var entry in addressableAssetGroup.entries)
                {
                    assetSettings.RemoveAssetEntry(entry.guid);
                }
            }

            return addressableAssetGroup;
        }

        private static void PlaceAssetIntoGroup_Environment(
            AddressableAssetSettings assetSettings,
            AddressableAssetGroup assetGroup)
        {
            var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game - Level Design");
            // var relativeCommonFolderPath = Path.Combine(relativeGameFolderPath, "Module - Environment - _Common");
            var relativeLevelFolderPath = Path.Combine(relativeGameFolderPath, "Module - Environment - Level");
            
            var relativeLevel001Path = Path.Combine(relativeLevelFolderPath, "Level 001", "Scenes");

            var absoluteLevel001Path = Path.Combine(Application.dataPath, "_", "1 - Game - Level Design",
                "Module - Environment - Level", "Level 001", "Scenes");
            
            var label = "level001";
            assetSettings.AddLabel(label);
            
            DirectoryInfo di = new DirectoryInfo(absoluteLevel001Path);

            var unitySceneFileInfos = di.EnumerateFiles().Where(fi => fi.Extension.CompareTo(".meta") != 0);

            foreach (var unitySceneFileInfo in unitySceneFileInfos)
            {
                var fileName = unitySceneFileInfo.Name;
                var strippedFileName = fileName.Replace(".unity", "");

                var scenePath = Path.Combine(relativeLevel001Path, fileName);
                // var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                
                Debug.Log(scenePath);

                PlaceAssetInAddressble(assetSettings, assetGroup, scenePath,
                    "level001", strippedFileName);
            }
            
            // var unitySceneFiles = Directory.EnumerateFiles(absoluteLevel001Path, "*.unity");
            // foreach (var unitySceneFile in unitySceneFiles)
            // {
            //     var fileName = unitySceneFile;
            //     
            //     Debug.Log(fileName);
            //     // PlaceAssetInAddressble
            // }
        }
        
        // //
        // private static void PlaceAssetIntoGroup_Common(
        //     AddressableAssetSettings assetSettings,
        //     AddressableAssetGroup assetGroup)
        // {
        //     var label = "Hud - Common";
        //     assetSettings.AddLabel(label);
            
        //     var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game - Hud Design");
        //     var relativeCommonFolderPath = Path.Combine(relativeGameFolderPath, "Module - Hud - _Common");

        //     var relativeExtensionPlayer =
        //         Path.Combine(relativeCommonFolderPath, "Data Assets", "Extension - PlayMaker");

        //     var relativeFontAtlas =
        //         Path.Combine(relativeCommonFolderPath, "Data Assets", "Font Atlas");

        //     var activateCanvasGroupAssetPath = Path.Combine(relativeExtensionPlayer, "Activate Canvas Group.asset");
        //     var deactivateCanvasGroupAssetPath = Path.Combine(relativeExtensionPlayer, "Deactivate Canvas Group.asset");

        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         activateCanvasGroupAssetPath,
        //         label,
        //         "Hud - Activate Canvas Group");

        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         deactivateCanvasGroupAssetPath,
        //         label,
        //         "Hud - Deactivate Canvas Group");
            
        //     //
        //     var chineseFontAtlasAssetPath =
        //         Path.Combine(relativeFontAtlas, "Chinese", "TaipeiSansTCBeta-Regular SDF.asset");
        //     var englishFontAtlasAssetPath =
        //         Path.Combine(relativeFontAtlas, "English", "Edition-BxnV SDF.asset");
        //     var japaneseFontAtlasAssetPath =
        //         Path.Combine(relativeFontAtlas, "Japanese", "Senobi-Gothic-Regular SDF.asset");
            
        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         chineseFontAtlasAssetPath,
        //         label,
        //         "Hud - Chinese Font Atlas");            
        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         englishFontAtlasAssetPath,
        //         label,
        //         "Hud - English Font Atlas");            
        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         japaneseFontAtlasAssetPath,
        //         label,
        //         "Hud - Japanese Font Atlas");            
        // }

        // private static void PlaceAssetIntoGroup_App(
        //     AddressableAssetSettings assetSettings,
        //     AddressableAssetGroup assetGroup)
        // {
        //     var label = "Hud - App";
        //     assetSettings.AddLabel(label);
            
        //     var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game - Hud Design");
        //     var relativeAppFolderPath = Path.Combine(relativeGameFolderPath, "Module - Hud - App");
            
        //     var canvasAssetPath = Path.Combine(relativeAppFolderPath, "Canvas.prefab");
            
        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         canvasAssetPath,
        //         label,
        //         "Hud - Canvas - App");
            
        //     var relativeI2Asset =
        //         Path.Combine(relativeAppFolderPath, "Extension - I2", "I2Languages.asset");
            
        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         relativeI2Asset,
        //         label,
        //         "Hud - App - I2");            
            
        //     var relativeViewLoadingPrefab =
        //         Path.Combine(relativeAppFolderPath, "View - Loading", "View - Loading.prefab");
        //     var relativeViewLoadingTimelineAsset =
        //         Path.Combine(relativeAppFolderPath, "View - Loading", "View - Loading.playable");

        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         relativeViewLoadingPrefab,
        //         label,
        //         "Hud - App - View - Loading Prefab");
        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         relativeViewLoadingTimelineAsset,
        //         label,
        //         "Hud - App - View - Loading Timeline");    
        // }

        // private static void PlaceAssetIntoGroup_Preparation(
        //     AddressableAssetSettings assetSettings,
        //     AddressableAssetGroup assetGroup)
        // {
        //     var label = "Hud - Preparation";
        //     assetSettings.AddLabel(label);
            
        //     var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game - Hud Design");
        //     var relativeAppFolderPath = Path.Combine(relativeGameFolderPath, "Module - Hud - Preparation");
            
        //     var canvasAssetPath = Path.Combine(relativeAppFolderPath, "Canvas.prefab");
            
        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         canvasAssetPath,
        //         label,
        //         "Hud - Canvas - Preparation");
            
        //     var relativeI2Asset =
        //         Path.Combine(relativeAppFolderPath, "Extension - I2", "I2Languages.asset");
            
        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         relativeI2Asset,
        //         label,
        //         "Hud - Preparation - I2");            
            
        //     var relativeViewBasePrefab =
        //         Path.Combine(relativeAppFolderPath, "View - Base", "View - Base.prefab");
        //     var relativeViewBaseTimelineAsset =
        //         Path.Combine(relativeAppFolderPath, "View - Base", "View - Base.playable");

        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         relativeViewBasePrefab,
        //         label,
        //         "Hud - Preparation - View - Base Prefab");
        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         relativeViewBaseTimelineAsset,
        //         label,
        //         "Hud - Preparation - View - Base Timeline");    
        // }
        
        // private static void PlaceAssetIntoGroup_Stage(
        //     AddressableAssetSettings assetSettings,
        //     AddressableAssetGroup assetGroup)
        // {
        //     var label = "Hud - Stage";
        //     assetSettings.AddLabel(label);
            
        //     var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game - Hud Design");
        //     var relativeAppFolderPath = Path.Combine(relativeGameFolderPath, "Module - Hud - Stage");
            
        //     var canvasAssetPath = Path.Combine(relativeAppFolderPath, "Canvas.prefab");
            
        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         canvasAssetPath,
        //         label,
        //         "Hud - Canvas - Stage");
            
        //     var relativeI2Asset =
        //         Path.Combine(relativeAppFolderPath, "Extension - I2", "I2Languages.asset");
            
        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         relativeI2Asset,
        //         label,
        //         "Hud - Stage - I2");            
            
        //     var relativeViewBasePrefab =
        //         Path.Combine(relativeAppFolderPath, "View - Base", "View - Base.prefab");
        //     var relativeViewBaseTimelineAsset =
        //         Path.Combine(relativeAppFolderPath, "View - Base", "View - Base.playable");

        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         relativeViewBasePrefab,
        //         label,
        //         "Hud - Stage - View - Base Prefab");
        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         relativeViewBaseTimelineAsset,
        //         label,
        //         "Hud - Stage - View - Base Timeline");    

        //     var relativeViewOptionPrefab =
        //         Path.Combine(relativeAppFolderPath, "View - Option", "View - Option.prefab");
        //     var relativeViewOptionTimelineAsset =
        //         Path.Combine(relativeAppFolderPath, "View - Option", "View - Option.playable");

        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         relativeViewOptionPrefab,
        //         label,
        //         "Hud - Stage - View - Option Prefab");
        //     PlaceAssetInAddressble(assetSettings, assetGroup,
        //         relativeViewOptionTimelineAsset,
        //         label,
        //         "Hud - Stage - View - Option Timeline");    
        // }

        private static void PlaceAssetInAddressble(
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
