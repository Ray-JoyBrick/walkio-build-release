namespace JoyBrick.Walkio.Build.HudDesignExport.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEngine;

    public partial class AssetCreator
    {
        [MenuItem("Assets/Walkio/Remove/Hud Asset Addressable Group")]
        public static void RemoveAddressableGroup()
        {
            var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            
            // Create group
            RemoveGroup(assetSettings, "Assets - Hud - Common");
            RemoveGroup(assetSettings, "Assets - Hud - App");
            RemoveGroup(assetSettings, "Assets - Hud - App - Assist");
            RemoveGroup(assetSettings, "Assets - Hud - Preparation");
            RemoveGroup(assetSettings, "Assets - Hud - Stage");
            RemoveGroup(assetSettings, "Assets - Hud - Stage - Assist");
        }

        // TODO: Extract to utility class
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
        
        [MenuItem("Assets/Walkio/Create/Hud Asset Addressable Group")]
        public static void CreateAddressableGroup()
        {
            var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            
            // Create group
            var commonGroup = CreateGroup(assetSettings, "Assets - Hud - Common");
            var appGroup = CreateGroup(assetSettings, "Assets - Hud - App");
            var appAssistGroup = CreateGroup(assetSettings, "Assets - Hud - App - Assist");
            var preparationGroup = CreateGroup(assetSettings, "Assets - Hud - Preparation");
            var stageGroup = CreateGroup(assetSettings, "Assets - Hud - Stage");
            var stageAssistGroup = CreateGroup(assetSettings, "Assets - Hud - Stage - Assist");
            
            //
            PlaceAssetIntoGroup_Common(assetSettings, commonGroup);
            PlaceAssetIntoGroup_App(assetSettings, appGroup);
            PlaceAssetIntoGroup_AppAssist(assetSettings, appAssistGroup);
            PlaceAssetIntoGroup_Preparation(assetSettings, preparationGroup);
            PlaceAssetIntoGroup_Stage(assetSettings, stageGroup);
            PlaceAssetIntoGroup_StageAssist(assetSettings, stageAssistGroup);
        }

        // TODO: Extract to utility class
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
        
        //
        private static void PlaceAssetIntoGroup_Common(
            AddressableAssetSettings assetSettings,
            AddressableAssetGroup assetGroup)
        {
            var label = "Hud - Common";
            assetSettings.AddLabel(label);
            
            var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game - Hud Design");
            var relativeCommonFolderPath = Path.Combine(relativeGameFolderPath, "Module - Hud - _Common");

            var relativeExtensionPlayer =
                Path.Combine(relativeCommonFolderPath, "Data Assets", "Extension - PlayMaker");

            var relativeFontAtlas =
                Path.Combine(relativeCommonFolderPath, "Data Assets", "Font Atlas");

            var activateCanvasGroupAssetPath = Path.Combine(relativeExtensionPlayer, "Activate Canvas Group.asset");
            var deactivateCanvasGroupAssetPath = Path.Combine(relativeExtensionPlayer, "Deactivate Canvas Group.asset");

            PlaceAssetInAddressble(assetSettings, assetGroup,
                activateCanvasGroupAssetPath,
                label,
                "Hud - Activate Canvas Group");

            PlaceAssetInAddressble(assetSettings, assetGroup,
                deactivateCanvasGroupAssetPath,
                label,
                "Hud - Deactivate Canvas Group");
            
            //
            var chineseFontAtlasAssetPath =
                Path.Combine(relativeFontAtlas, "Chinese", "TaipeiSansTCBeta-Regular SDF.asset");
            var englishFontAtlasAssetPath =
                Path.Combine(relativeFontAtlas, "English", "Edition-BxnV SDF.asset");
            var japaneseFontAtlasAssetPath =
                Path.Combine(relativeFontAtlas, "Japanese", "Senobi-Gothic-Regular SDF.asset");
            
            PlaceAssetInAddressble(assetSettings, assetGroup,
                chineseFontAtlasAssetPath,
                label,
                "Hud - Chinese Font Atlas");            
            PlaceAssetInAddressble(assetSettings, assetGroup,
                englishFontAtlasAssetPath,
                label,
                "Hud - English Font Atlas");            
            PlaceAssetInAddressble(assetSettings, assetGroup,
                japaneseFontAtlasAssetPath,
                label,
                "Hud - Japanese Font Atlas");            
        }

        private static void PlaceAssetIntoGroup_App(
            AddressableAssetSettings assetSettings,
            AddressableAssetGroup assetGroup)
        {
            var label = "Hud - App";
            assetSettings.AddLabel(label);
            
            var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game - Hud Design");
            var relativeAppFolderPath = Path.Combine(relativeGameFolderPath, "Module - Hud - App");
            
            var canvasAssetPath = Path.Combine(relativeAppFolderPath, "Canvas.prefab");
            
            PlaceAssetInAddressble(assetSettings, assetGroup,
                canvasAssetPath,
                label,
                "Hud - Canvas - App");
            
            var relativeI2Asset =
                Path.Combine(relativeAppFolderPath, "Extension - I2", "I2Languages.asset");
            
            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeI2Asset,
                label,
                "Hud - App - I2");            
            
            var relativeViewLoadingPrefab =
                Path.Combine(relativeAppFolderPath, "View - Loading", "View - Loading.prefab");
            var relativeViewLoadingTimelineAsset =
                Path.Combine(relativeAppFolderPath, "View - Loading", "View - Loading.playable");

            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewLoadingPrefab,
                label,
                "Hud - App - View - Loading Prefab");
            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewLoadingTimelineAsset,
                label,
                "Hud - App - View - Loading Timeline");    

            var relativeViewHelpPrefab =
                Path.Combine(relativeAppFolderPath, "View - Help", "View - Help.prefab");
            var relativeViewHelpTimelineAsset =
                Path.Combine(relativeAppFolderPath, "View - Help", "View - Help.playable");

            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewHelpPrefab,
                label,
                "Hud - App - View - Help Prefab");
            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewHelpTimelineAsset,
                label,
                "Hud - App - View - Help Timeline");    
        }

        private static void PlaceAssetIntoGroup_AppAssist(
            AddressableAssetSettings assetSettings,
            AddressableAssetGroup assetGroup)
        {
            var label = "Hud - App - Assist";
            assetSettings.AddLabel(label);
            
            var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game - Hud Design");
            var relativeAppFolderPath = Path.Combine(relativeGameFolderPath, "Module - Hud - App - Assist");
            
            var canvasAssetPath = Path.Combine(relativeAppFolderPath, "Canvas.prefab");
            
            PlaceAssetInAddressble(assetSettings, assetGroup,
                canvasAssetPath,
                label,
                "Hud - Canvas - App - Assist");
            
            var relativeI2Asset =
                Path.Combine(relativeAppFolderPath, "Extension - I2", "I2Languages.asset");
            
            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeI2Asset,
                label,
                "Hud - App - Assist - I2");
            
            var relativeViewLoadingPrefab =
                Path.Combine(relativeAppFolderPath, "View - Base", "View - Base.prefab");
            var relativeViewLoadingTimelineAsset =
                Path.Combine(relativeAppFolderPath, "View - Base", "View - Base.playable");

            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewLoadingPrefab,
                label,
                "Hud - App - Assist - View - Base Prefab");
            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewLoadingTimelineAsset,
                label,
                "Hud - App - Assist - View - Base Timeline");

            //
            var relativeViewInitialPrefab =
                Path.Combine(relativeAppFolderPath, "View - Initial", "View - Initial.prefab");
            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewInitialPrefab,
                label,
                "Hud - App - Assist - View - Initial Prefab");
        }

        private static void PlaceAssetIntoGroup_Preparation(
            AddressableAssetSettings assetSettings,
            AddressableAssetGroup assetGroup)
        {
            var label = "Hud - Preparation";
            assetSettings.AddLabel(label);
            
            var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game - Hud Design");
            var relativeAppFolderPath = Path.Combine(relativeGameFolderPath, "Module - Hud - Preparation");
            
            var canvasAssetPath = Path.Combine(relativeAppFolderPath, "Canvas.prefab");
            
            PlaceAssetInAddressble(assetSettings, assetGroup,
                canvasAssetPath,
                label,
                "Hud - Canvas - Preparation");
            
            var relativeI2Asset =
                Path.Combine(relativeAppFolderPath, "Extension - I2", "I2Languages.asset");
            
            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeI2Asset,
                label,
                "Hud - Preparation - I2");            
            
            var relativeViewBasePrefab =
                Path.Combine(relativeAppFolderPath, "View - Base", "View - Base.prefab");
            var relativeViewBaseTimelineAsset =
                Path.Combine(relativeAppFolderPath, "View - Base", "View - Base.playable");

            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewBasePrefab,
                label,
                "Hud - Preparation - View - Base Prefab");
            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewBaseTimelineAsset,
                label,
                "Hud - Preparation - View - Base Timeline");    
        }
        
        private static void PlaceAssetIntoGroup_Stage(
            AddressableAssetSettings assetSettings,
            AddressableAssetGroup assetGroup)
        {
            var label = "Hud - Stage";
            assetSettings.AddLabel(label);
            
            var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game - Hud Design");
            var relativeAppFolderPath = Path.Combine(relativeGameFolderPath, "Module - Hud - Stage");
            
            var canvasAssetPath = Path.Combine(relativeAppFolderPath, "Canvas.prefab");
            
            PlaceAssetInAddressble(assetSettings, assetGroup,
                canvasAssetPath,
                label,
                "Hud - Canvas - Stage");
            
            var relativeI2Asset =
                Path.Combine(relativeAppFolderPath, "Extension - I2", "I2Languages.asset");
            
            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeI2Asset,
                label,
                "Hud - Stage - I2");            
            
            var relativeViewBasePrefab =
                Path.Combine(relativeAppFolderPath, "View - Base", "View - Base.prefab");
            var relativeViewBaseTimelineAsset =
                Path.Combine(relativeAppFolderPath, "View - Base", "View - Base.playable");

            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewBasePrefab,
                label,
                "Hud - Stage - View - Base Prefab");
            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewBaseTimelineAsset,
                label,
                "Hud - Stage - View - Base Timeline");    

            var relativeViewOptionPrefab =
                Path.Combine(relativeAppFolderPath, "View - Option", "View - Option.prefab");
            var relativeViewOptionTimelineAsset =
                Path.Combine(relativeAppFolderPath, "View - Option", "View - Option.playable");

            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewOptionPrefab,
                label,
                "Hud - Stage - View - Option Prefab");
            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewOptionTimelineAsset,
                label,
                "Hud - Stage - View - Option Timeline");    
        }
        
        private static void PlaceAssetIntoGroup_StageAssist(
            AddressableAssetSettings assetSettings,
            AddressableAssetGroup assetGroup)
        {
            var label = "Hud - Stage - Assist";
            assetSettings.AddLabel(label);
            
            var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game - Hud Design");
            var relativeAppFolderPath = Path.Combine(relativeGameFolderPath, "Module - Hud - Stage - Assist");
            
            var canvasAssetPath = Path.Combine(relativeAppFolderPath, "Canvas.prefab");
            
            PlaceAssetInAddressble(assetSettings, assetGroup,
                canvasAssetPath,
                label,
                "Hud - Canvas - Stage - Assist");
            
            var relativeI2Asset =
                Path.Combine(relativeAppFolderPath, "Extension - I2", "I2Languages.asset");
            
            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeI2Asset,
                label,
                "Hud - Stage - Assist - I2");
            
            var relativeViewBasePrefab =
                Path.Combine(relativeAppFolderPath, "View - Base", "View - Base.prefab");
            var relativeViewBaseTimelineAsset =
                Path.Combine(relativeAppFolderPath, "View - Base", "View - Base.playable");

            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewBasePrefab,
                label,
                "Hud - Stage - Assist - View - Base Prefab");
            PlaceAssetInAddressble(assetSettings, assetGroup,
                relativeViewBaseTimelineAsset,
                label,
                "Hud - Stage - Assist - View - Base Timeline");    
        }        

        // TODO: Extract to utility class
        // This puts asset into addressable for according group, label
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
