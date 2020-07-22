namespace JoyBrick.Walkio.Build.LevelDesignExport.EditorPart
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public partial class AssetCreator
    {
        [MenuItem("Assets/Walkio/Remove/Level Asset - Level Addressable Group")]
        public static void RemoveAddressableGroup()
        {
            var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            
            // Create group
            RemoveGroup(assetSettings, "Assets - Level");
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
        
        [MenuItem("Assets/Walkio/Create/Level Asset - Level Addressable Group")]
        public static void CreateAddressableGroup()
        {
            var assetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            
            // Create group
            var levelGroup = CreateGroup(assetSettings, "Assets - Level");
            
            //
            PlaceAssetIntoGroup_Environment(assetSettings, levelGroup);
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
            PlaceAssetIntoGroup_EnvironmentLevel(
                "Level 004",
                assetSettings, assetGroup);
        }

        private static void PlaceAssetIntoGroup_EnvironmentLevel(
            string levelName,
            AddressableAssetSettings assetSettings,
            AddressableAssetGroup assetGroup)
        {
            var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game - Level Design - Generated");
            // var relativeCommonFolderPath = Path.Combine(relativeGameFolderPath, "Module - Environment - _Common");
            var relativeLevelFolderPath = Path.Combine(relativeGameFolderPath,
                "Module - Environment - Level", "Levels");
            
            var relativeLevel001Path = Path.Combine(relativeLevelFolderPath, levelName);
            var relativeLevelSettingPathPath = Path.Combine(relativeLevel001Path, "level-setting");
            // var relativeObstacleTexturePathPath = Path.Combine(relativeLevel001Path, "obstacle-texture");
            // var relativeWaypointPathPath = Path.Combine(relativeLevel001Path, "waypoint-path");
            var relativeAStarDataPathPath = Path.Combine(relativeLevel001Path, "astar-data");
            var relativeScenePath = Path.Combine(relativeLevel001Path, "scenes");

            var absoluteLevel001LevelSettingPath = Path.Combine(Application.dataPath, "_", "1 - Game - Level Design - Generated",
                "Module - Environment - Level",
                "Levels", levelName, "level-setting");

            var absoluteLevel001ObstacleTexturePath = Path.Combine(Application.dataPath, "_", "1 - Game - Level Design - Generated",
                "Module - Environment - Level",
                "Levels", levelName, "obstacle-texture");

            var absoluteLevel001WaypointDataPath = Path.Combine(Application.dataPath, "_", "1 - Game - Level Design - Generated",
                "Module - Environment - Level",
                "Levels", levelName, "waypoint-path");

            var absoluteLevel001AStarDataPath = Path.Combine(Application.dataPath, "_", "1 - Game - Level Design - Generated",
                "Module - Environment - Level",
                "Levels", levelName, "astar-data");

            var absoluteLevel001ScenePath = Path.Combine(Application.dataPath, "_", "1 - Game - Level Design - Generated",
                "Module - Environment - Level",
                "Levels", levelName, "scenes");

            var label = $"{levelName}";
            assetSettings.AddLabel(label);
            
            {
                DirectoryInfo di = new DirectoryInfo(absoluteLevel001LevelSettingPath);
                //
                var levelSettingFileInfos = di.EnumerateFiles().Where(fi => fi.Extension.CompareTo(".meta") != 0);
            
                foreach (var levelSettingFileInfo in levelSettingFileInfos)
                {
                    var fileName = levelSettingFileInfo.Name;
                    var strippedFileName = fileName.Replace(".asset", "");
                    strippedFileName = fileName.Replace(".prefab", "");
            
                    var levelSettingPath = Path.Combine(relativeLevelSettingPathPath, fileName);
                    // var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                
                    Debug.Log(levelSettingPath);
            
                    GameCommon.EditorPart.Utility.PlaceAssetInAddressble(assetSettings, assetGroup, levelSettingPath,
                        label, strippedFileName);
                }
            }            

            // {
            //     DirectoryInfo di = new DirectoryInfo(absoluteLevel001ObstacleTexturePath);
            //     //
            //     var obstacleTextureFileInfos = di.EnumerateFiles().Where(fi => fi.Extension.CompareTo(".meta") != 0);
            //
            //     foreach (var obstacleTextureFileInfo in obstacleTextureFileInfos)
            //     {
            //         var fileName = obstacleTextureFileInfo.Name;
            //         var strippedFileName = fileName.Replace(".png", "");
            //
            //         var obstacleTexturePath = Path.Combine(relativeObstacleTexturePathPath, fileName);
            //         // var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            //     
            //         Debug.Log(obstacleTexturePath);
            //
            //         LevelDesignExport.EditorPart.Utility.PlaceAssetInAddressble(assetSettings, assetGroup, obstacleTexturePath,
            //             label, strippedFileName);
            //     }
            // }

            // {
            //     DirectoryInfo di = new DirectoryInfo(absoluteLevel001WaypointDataPath);
            //     //
            //     var waypointDataFileInfos = di.EnumerateFiles().Where(fi => fi.Extension.CompareTo(".meta") != 0);
            //
            //     foreach (var waypointDataFileInfo in waypointDataFileInfos)
            //     {
            //         var fileName = waypointDataFileInfo.Name;
            //         var strippedFileName = fileName.Replace(".asset", "");
            //         strippedFileName = fileName.Replace(".prefab", "");
            //
            //         var waypointDataPath = Path.Combine(relativeWaypointPathPath, fileName);
            //         // var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            //     
            //         Debug.Log(waypointDataPath);
            //
            //         LevelDesignExport.EditorPart.Utility.PlaceAssetInAddressble(assetSettings, assetGroup, waypointDataPath,
            //             label, strippedFileName);
            //     }
            // }

            {
                DirectoryInfo di = new DirectoryInfo(absoluteLevel001AStarDataPath);
                //
                var astarDataFileInfos = di.EnumerateFiles().Where(fi => fi.Extension.CompareTo(".meta") != 0);
            
                foreach (var astarDataFileInfo in astarDataFileInfos)
                {
                    var fileName = astarDataFileInfo.Name;
                    var strippedFileName = fileName.Replace(".bytes", "");
            
                    var astarDataPath = Path.Combine(relativeAStarDataPathPath, fileName);
                    // var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                
                    Debug.Log(astarDataPath);
            
                    GameCommon.EditorPart.Utility.PlaceAssetInAddressble(assetSettings, assetGroup, astarDataPath,
                        label, strippedFileName);
                }
            }
            
            {
                DirectoryInfo di = new DirectoryInfo(absoluteLevel001ScenePath);
                //
                var sceneFiles = di.EnumerateFiles().Where(fi => fi.Extension.CompareTo(".meta") != 0);
            
                foreach (var sceneFile in sceneFiles)
                {
                    var fileName = sceneFile.Name;
                    var strippedFileName = fileName.Replace(".unity", "");
            
                    var scenePath = Path.Combine(relativeScenePath, fileName);
                    // var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                
                    Debug.Log(scenePath);
            
                    GameCommon.EditorPart.Utility.PlaceAssetInAddressble(assetSettings, assetGroup, scenePath,
                        label, strippedFileName);
                }
            }  
            
            // {
            //     DirectoryInfo di = new DirectoryInfo(absoluteLevel001WaypointDataPath);
            //     //
            //     var waypointDataFileInfos = di.EnumerateFiles().Where(fi => fi.Extension.CompareTo(".meta") != 0);
            //
            //     foreach (var waypointDataFileInfo in waypointDataFileInfos)
            //     {
            //         var fileName = waypointDataFileInfo.Name;
            //         var strippedFileName = fileName.Replace(".prefab", "");
            //
            //         var waypointDataPath = Path.Combine(relativeWaypointPathPath, fileName);
            //         // var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            //     
            //         Debug.Log(waypointDataPath);
            //
            //         PlaceAssetInAddressble(assetSettings, assetGroup, waypointDataPath,
            //             label, strippedFileName);
            //     }
            // }

            // var unitySceneFiles = Directory.EnumerateFiles(absoluteLevel001Path, "*.unity");
            // foreach (var unitySceneFile in unitySceneFiles)
            // {
            //     var fileName = unitySceneFile;
            //     
            //     Debug.Log(fileName);
            //     // PlaceAssetInAddressble
            // }
        }        

        // private static void PlaceAssetIntoGroup_Environment(
        //     AddressableAssetSettings assetSettings,
        //     AddressableAssetGroup assetGroup)
        // {
        //     var relativeGameFolderPath = Path.Combine("Assets", "_", "1 - Game - Level Design");
        //     // var relativeCommonFolderPath = Path.Combine(relativeGameFolderPath, "Module - Environment - _Common");
        //     var relativeLevelFolderPath = Path.Combine(relativeGameFolderPath, "Module - Environment - Level");
        //     
        //     var relativeLevel001Path = Path.Combine(relativeLevelFolderPath, "Level 001", "Scenes");
        //
        //     var absoluteLevel001Path = Path.Combine(Application.dataPath, "_", "1 - Game - Level Design",
        //         "Module - Environment - Level", "Level 001", "Scenes");
        //     
        //     var label = "level001";
        //     assetSettings.AddLabel(label);
        //     
        //     DirectoryInfo di = new DirectoryInfo(absoluteLevel001Path);
        //
        //     var unitySceneFileInfos = di.EnumerateFiles().Where(fi => fi.Extension.CompareTo(".meta") != 0);
        //
        //     foreach (var unitySceneFileInfo in unitySceneFileInfos)
        //     {
        //         var fileName = unitySceneFileInfo.Name;
        //         var strippedFileName = fileName.Replace(".unity", "");
        //
        //         var scenePath = Path.Combine(relativeLevel001Path, fileName);
        //         // var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        //         
        //         Debug.Log(scenePath);
        //
        //         PlaceAssetInAddressble(assetSettings, assetGroup, scenePath,
        //             "level001", strippedFileName);
        //     }
        //     
        //     // var unitySceneFiles = Directory.EnumerateFiles(absoluteLevel001Path, "*.unity");
        //     // foreach (var unitySceneFile in unitySceneFiles)
        //     // {
        //     //     var fileName = unitySceneFile;
        //     //     
        //     //     Debug.Log(fileName);
        //     //     // PlaceAssetInAddressble
        //     // }
        // }
    } 
} 
