namespace JoyBrick.Walkio.Build.FlowExport.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEngine;

    public partial class AssetCreator
    {
        [MenuItem("Assets/Walkio/Remove/Flow Asset Addressable Group")]
        public static void RemoveAddressableGroup()
        {
            var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            
            // Create group
            RemoveGroup(assetSettings, "Assets - Flow");
            RemoveGroup(assetSettings, "Assets - Stage Flow");
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
        
        [MenuItem("Assets/Walkio/Create/Flow Asset Addressable Group")]
        public static void CreateAddressableGroup()
        {
            var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            
            // Create group
            var gameFlowGroup = CreateGroup(assetSettings, "Assets - Game Flow");
            var stageFlowGroup = CreateGroup(assetSettings, "Assets - Stage Flow");
            
            //
            PlaceAssetIntoGroup_GameFlow(assetSettings, gameFlowGroup);
            PlaceAssetIntoGroup_StageFlow(assetSettings, stageFlowGroup);
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
        
        //
        private static void PlaceAssetIntoGroup_GameFlow(
            AddressableAssetSettings assetSettings,
            AddressableAssetGroup assetGroup)
        {
            var label = "Game Flow";
            assetSettings.AddLabel(label);
            
            var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game");
            // var relativeCommonFolderPath = Path.Combine(relativeGameFolderPath, "Module - Game Flow Control");

            var relativeExtensionPlayer =
                Path.Combine(relativeGameFolderPath, "Module - Game Flow Control", "Prefabs");
            
            // var relativeFontAtlas =
            //     Path.Combine(relativeCommonFolderPath, "Data Assets", "Font Atlas");

            var activateCanvasGroupAssetPath = Path.Combine(relativeExtensionPlayer, "Flow.prefab");
            // var deactivateCanvasGroupAssetPath = Path.Combine(relativeExtensionPlayer, "Deactivate Canvas Group.asset");

            PlaceAssetInAddressble(assetSettings, assetGroup,
                activateCanvasGroupAssetPath,
                label,
                "Game Flow");
        }

        //
        private static void PlaceAssetIntoGroup_StageFlow(
            AddressableAssetSettings assetSettings,
            AddressableAssetGroup assetGroup)
        {
            var label = "Stage Flow";
            assetSettings.AddLabel(label);
            
            var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game");
            // var relativeCommonFolderPath = Path.Combine(relativeGameFolderPath, "Module - Game Flow Control");

            var relativeExtensionPlayer =
                Path.Combine(relativeGameFolderPath, "Module - Stage Flow Control", "Prefabs");
            
            // var relativeFontAtlas =
            //     Path.Combine(relativeCommonFolderPath, "Data Assets", "Font Atlas");

            var createNeutralForceAssetPath = Path.Combine(relativeExtensionPlayer, "Create Neutral Force Unit.prefab");
            var createTeamForceAssetPath = Path.Combine(relativeExtensionPlayer, "Create Team Force Unit.prefab");
            // var deactivateCanvasGroupAssetPath = Path.Combine(relativeExtensionPlayer, "Deactivate Canvas Group.asset");

            PlaceAssetInAddressble(assetSettings, assetGroup,
                createNeutralForceAssetPath,
                label,
                "Create Neutral Force Unit");
            PlaceAssetInAddressble(assetSettings, assetGroup,
                createTeamForceAssetPath,
                label,
                "Create Team Force Unit");
        }

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
