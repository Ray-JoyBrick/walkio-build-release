namespace JoyBrick.Walkio.Build.CreatureDesignExport.EditorPart
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    
    //
    public partial class AssetCreator
    {
        [MenuItem("Assets/Walkio/Remove/Level Asset - Creature Addressable Group")]
        public static void RemoveAddressableGroup()
        {
            var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            
            // Create group
            RemoveGroup(assetSettings, "Assets - Creature");
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
        
        [MenuItem("Assets/Walkio/Create/Level Asset - Creature Addressable Group")]
        public static void CreateAddressableGroup()
        {
            var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            
            // Create group
            var creatureGroup = CreateGroup(assetSettings, "Assets - Creature");
            
            //
            PlaceAssetIntoGroup_Creature(assetSettings, creatureGroup);
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
        
        private static void PlaceAssetIntoGroup_Creature(
            AddressableAssetSettings assetSettings,
            AddressableAssetGroup assetGroup)
        {
            //
            var crossProjectData = AssetDatabase.LoadAssetAtPath<GameCommon.CrossProjectData>(
                "Packages/walkio.game.common/Data Assets/Cross Project Data.asset");

            //
            var relativeAssetFolderName = "Assets";
            var projectBaseFolderName = crossProjectData.commonProjectData.projectBaseFolderName;
            var baseFolderName = crossProjectData.assetLevelDesignProjectData.baseFolderName;
            var creatureModuleFolderName = crossProjectData.assetLevelDesignProjectData.creatureModuleFolderName;
            var generationBaseFolderName = crossProjectData.assetLevelDesignProjectData.generationBaseFolderName;
            
            var relativeGameFolderPath = Path.Combine(relativeAssetFolderName, projectBaseFolderName, generationBaseFolderName);

            var creatureDataAssetPath = Path.Combine(relativeGameFolderPath, creatureModuleFolderName, "Data Assets", "Creature Data.asset");
            
            var label = "Creature";
            assetSettings.AddLabel(label);

            GameCommon.EditorPart.Utility.PlaceAssetInAddressble(assetSettings, assetGroup, creatureDataAssetPath,
                label, "Creature Data");
        }        
    } 
} 
