namespace JoyBrick.Walkio.Build.CreatureDesignExport.EditorPart
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public partial class AssetCreator
    {
#if ASSET_LEVEL_DESIGN

        [MenuItem("Assets/Walkio/Export/Creature Asset Package")]
        public static void ExportPackage()
        {
            var startTimeStamp = DateTime.Now;
            Debug.Log($"Creature - AssetCreator - Export - start {startTimeStamp:mm:ss.fff}");

            var parentDirectory = Directory.GetParent(Application.dataPath);
            var packagePath = Path.Combine(parentDirectory.FullName, "Level Asset - Creature.unitypackage");

            var paths = new List<string>
            {
                "Assets/_/1 - Game - Level Design - Generated/Module - Environment - Creature"
            };
            
            AssetDatabase.ExportPackage(paths.ToArray(), packagePath, ExportPackageOptions.Recurse);

            var endTimeStamp = DateTime.Now;
            Debug.Log($"Creature - AssetCreator - Export - end {endTimeStamp:mm:ss.fff}");            
        }

#endif
    } 
} 
