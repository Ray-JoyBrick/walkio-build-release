namespace JoyBrick.Walkio.Build.LevelDesignExport.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public partial class AssetCreator
    {
        #if ASSET_LEVEL_DESIGN

        [MenuItem("Assets/Walkio/Export/Level Asset Package")]
        public static void ExportPackage()
        {
            var startTimeStamp = DateTime.Now;
            Debug.Log($"AssetCreator - Export - start {startTimeStamp:mm:ss.fff}");

            var parentDirectory = Directory.GetParent(Application.dataPath);
            var packagePath = Path.Combine(parentDirectory.FullName, "Level Asset.unitypackage");

            var paths = new List<string>
            {
                // // Assets generated
                // "Assets/_/_Generated",
                // // Localization sheet gathering here
                // "Assets/Resources/I2Languages.asset"
                "Assets/_/1 - Game - Level Design - Generated"
            };
            
            AssetDatabase.ExportPackage(paths.ToArray(), packagePath, ExportPackageOptions.Recurse);

            var endTimeStamp = DateTime.Now;
            Debug.Log($"AssetCreator - Export - end {endTimeStamp:mm:ss.fff}");            
        }

        #endif
    } 
} 
