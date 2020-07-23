namespace JoyBrick.Walkio.Build.FlowControlExport.EditorPart
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
        private const string groupName0 = "Flow Control";

        private const string groupName1 = "Flow Control - App";
        private const string groupName2 = "Flow Control - Stage";

        [MenuItem("Assets/Walkio/Addressable/Remove/Flow Control")]
        public static void RemoveAddressableGroup()
        {
            var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;

            GameCommon.EditorPart.Utility.AddressableHelper.RemoveGroup(assetSettings, groupName0);

            // GameCommon.EditorPart.Utility.AddressableHelper.RemoveGroup(assetSettings, groupName1);
            // GameCommon.EditorPart.Utility.AddressableHelper.RemoveGroup(assetSettings, groupName2);
        }

        [MenuItem("Assets/Walkio/Addressable/Create/Flow Control")]
        public static void CreateAddressableGroup()
        {
            var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;

            // Create group
            GameCommon.EditorPart.Utility.AddressableHelper.CreateGroup(assetSettings, groupName0);
            
            // GameCommon.EditorPart.Utility.AddressableHelper.CreateGroup(assetSettings, groupName1);
            // GameCommon.EditorPart.Utility.AddressableHelper.CreateGroup(assetSettings, groupName2);
        }
    }
}
