namespace JoyBrick.Walkio.Build.HudDesignExport.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public partial class AssetCreator
    {
        #if ASSET_HUD_DESIGN

        [MenuItem("Assets/Walkio/Export/Hud Asset Package")]
        public static void ExportPackage()
        {
            var startTimeStamp = DateTime.Now;
            Debug.Log($"AssetCreator - Export - start {startTimeStamp:mm:ss.fff}");

            var parentDirectory = Directory.GetParent(Application.dataPath);
            var packagePath = Path.Combine(parentDirectory.FullName, "Hud Asset.unitypackage");

            var paths = new List<string>
            {
                "Assets/_/1 - Game - Hud Design Generated"
            };
            
            AssetDatabase.ExportPackage(paths.ToArray(), packagePath, ExportPackageOptions.Recurse);

            var endTimeStamp = DateTime.Now;
            Debug.Log($"AssetCreator - Export - end {endTimeStamp:mm:ss.fff}");            
        }

        #endif
    } 
} 
