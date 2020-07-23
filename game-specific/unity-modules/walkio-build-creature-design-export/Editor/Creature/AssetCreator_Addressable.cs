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

    public partial class AssetCreator
    {
        private const string groupName1000 = "Creatue Leader Player";

        private const string groupName1001 = "Creatue Leader Player 001";
        private const string groupName1002 = "Creatue Leader Player 002";

        private const string groupName2000 = "Creatue Leader Npc";

        private const string groupName2001 = "Creatue Leader Npc 001";
        private const string groupName2002 = "Creatue Leader Npc 002";

        private const string groupName3000 = "Creatue Minion";

        private const string groupName3001 = "Creatue Minion 001";
        private const string groupName3002 = "Creatue Minion 002";

        [MenuItem("Assets/Walkio/Addressable/Remove/Creature")]
        public static void RemoveAddressableGroup()
        {
            var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;

            GameCommon.EditorPart.Utility.AddressableHelper.RemoveGroup(assetSettings, groupName1000);
            GameCommon.EditorPart.Utility.AddressableHelper.RemoveGroup(assetSettings, groupName2000);
            GameCommon.EditorPart.Utility.AddressableHelper.RemoveGroup(assetSettings, groupName3000);

            // GameCommon.EditorPart.Utility.AddressableHelper.RemoveGroup(assetSettings, groupName1001);
            // GameCommon.EditorPart.Utility.AddressableHelper.RemoveGroup(assetSettings, groupName1002);
            //
            // GameCommon.EditorPart.Utility.AddressableHelper.RemoveGroup(assetSettings, groupName2001);
            // GameCommon.EditorPart.Utility.AddressableHelper.RemoveGroup(assetSettings, groupName2002);
            //
            // GameCommon.EditorPart.Utility.AddressableHelper.RemoveGroup(assetSettings, groupName3001);
            // GameCommon.EditorPart.Utility.AddressableHelper.RemoveGroup(assetSettings, groupName3002);
        }

        [MenuItem("Assets/Walkio/Addressable/Create/Creature")]
        public static void CreateAddressableGroup()
        {
            var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;

            // Create group
            GameCommon.EditorPart.Utility.AddressableHelper.CreateGroup(assetSettings, groupName1000);
            GameCommon.EditorPart.Utility.AddressableHelper.CreateGroup(assetSettings, groupName2000);
            
            GameCommon.EditorPart.Utility.AddressableHelper.CreateGroup(assetSettings, groupName3000);

            // GameCommon.EditorPart.Utility.AddressableHelper.CreateGroup(assetSettings, groupName1001);
            // GameCommon.EditorPart.Utility.AddressableHelper.CreateGroup(assetSettings, groupName1002);
            //
            // GameCommon.EditorPart.Utility.AddressableHelper.CreateGroup(assetSettings, groupName2001);
            // GameCommon.EditorPart.Utility.AddressableHelper.CreateGroup(assetSettings, groupName2002);
            //
            // GameCommon.EditorPart.Utility.AddressableHelper.CreateGroup(assetSettings, groupName3001);
            // GameCommon.EditorPart.Utility.AddressableHelper.CreateGroup(assetSettings, groupName3002);
        }
    }
}

// namespace JoyBrick.Walkio.Build.CreatureDesignExport.EditorPart
// {
//     using System;
//     using System.Collections.Generic;
//     using System.IO;
//     using System.Linq;
//     using UnityEditor;
//     using UnityEditor.AddressableAssets.Settings;
//     using UnityEngine;
//
//     //
//     using GameCommon = JoyBrick.Walkio.Game.Common;
//
//     //
//     public partial class AssetCreator
//     {
//         [MenuItem("Assets/Walkio/Remove/Level Asset - Creature Addressable Group")]
//         public static void RemoveAddressableGroup()
//         {
//             var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
//
//             // Create group
//             RemoveGroup(assetSettings, "Assets - Creature");
//         }
//
//         private static void RemoveGroup(AddressableAssetSettings assetSettings, string groupName)
//         {
//             // Clean up what is already in
//             var addressableAssetGroup = assetSettings.FindGroup(groupName);
//             if (addressableAssetGroup == null)
//             {
//                 addressableAssetGroup = assetSettings.CreateGroup(groupName, false, false, false, assetSettings.DefaultGroup.Schemas);
//             }
//
//             if (addressableAssetGroup != null)
//             {
//                 assetSettings.RemoveGroup(addressableAssetGroup);
//             }
//         }
//
//         [MenuItem("Assets/Walkio/Create/Level Asset - Creature Addressable Group")]
//         public static void CreateAddressableGroup()
//         {
//             var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
//
//             // Create group
//             var creatureGroup = CreateGroup(assetSettings, "Assets - Creature");
//
//             //
//             PlaceAssetIntoGroup_Creature(assetSettings, creatureGroup);
//         }
//
//         private static AddressableAssetGroup CreateGroup(AddressableAssetSettings assetSettings, string groupName)
//         {
//             // Clean up what is already in
//             var addressableAssetGroup = assetSettings.FindGroup(groupName);
//             if (addressableAssetGroup == null)
//             {
//                 addressableAssetGroup = assetSettings.CreateGroup(groupName, false, false, false, assetSettings.DefaultGroup.Schemas);
//             }
//
//             if (addressableAssetGroup != null)
//             {
// //                aasdoSettings.RemoveGroup(addressableAssetGroup);
//                 foreach (var entry in addressableAssetGroup.entries)
//                 {
//                     assetSettings.RemoveAssetEntry(entry.guid);
//                 }
//             }
//
//             return addressableAssetGroup;
//         }
//
//         private static void PlaceAssetIntoGroup_Creature(
//             AddressableAssetSettings assetSettings,
//             AddressableAssetGroup assetGroup)
//         {
//             //
//             var crossProjectData = AssetDatabase.LoadAssetAtPath<GameCommon.CrossProjectData>(
//                 "Packages/walkio.game.common/Data Assets/Cross Project Data.asset");
//
//             //
//             var relativeAssetFolderName = "Assets";
//             var projectBaseFolderName = crossProjectData.commonProjectData.projectBaseFolderName;
//             var baseFolderName = crossProjectData.assetLevelDesignProjectData.baseFolderName;
//             var creatureModuleFolderName = crossProjectData.assetLevelDesignProjectData.creatureModuleFolderName;
//             var generationBaseFolderName = crossProjectData.assetLevelDesignProjectData.generationBaseFolderName;
//
//             var relativeGameFolderPath = Path.Combine(relativeAssetFolderName, projectBaseFolderName, generationBaseFolderName);
//
//             var creatureDataAssetPath = Path.Combine(relativeGameFolderPath, creatureModuleFolderName, "Data Assets", "Creature Data.asset");
//
//             var label = "Creature";
//             assetSettings.AddLabel(label);
//
//             GameCommon.EditorPart.Utility.PlaceAssetInAddressble(assetSettings, assetGroup, creatureDataAssetPath,
//                 label, "Creature Data");
//         }
//     }
// }
