namespace JoyBrick.Walkio.Build.FlowExport.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public partial class AssetCreator
    {
        // #if ASSET_HUD_DESIGN

        [MenuItem("Assets/Walkio/Export/Flow Asset Package")]
        public static void ExportPackage()
        {
            var startTimeStamp = DateTime.Now;
            Debug.Log($"AssetCreator - Export - start {startTimeStamp:mm:ss.fff}");

            var parentDirectory = Directory.GetParent(Application.dataPath);
            var packagePath = Path.Combine(parentDirectory.FullName, "Flow Asset.unitypackage");

            var paths = new List<string>
            {
                // // Assets generated
                // "Assets/_/_Generated",
                // // Localization sheet gathering here
                // "Assets/Resources/I2Languages.asset"
                "Assets/_/1 - Game/Module - Game Flow Control"
            };
            
            AssetDatabase.ExportPackage(paths.ToArray(), packagePath, ExportPackageOptions.Recurse);

            var endTimeStamp = DateTime.Now;
            Debug.Log($"AssetCreator - Export - end {endTimeStamp:mm:ss.fff}");
        }

        // #endif
    } 
} 
