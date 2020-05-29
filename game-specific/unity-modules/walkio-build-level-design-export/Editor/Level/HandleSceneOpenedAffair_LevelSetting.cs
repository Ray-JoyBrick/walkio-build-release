namespace JoyBrick.Walkio.Build.LevelDesignExport.EditorPart
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Unity.Entities;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommonEditor = JoyBrick.Walkio.Game.Common.EditorPart;
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;

    public static partial class HandleSceneOpenedAffair
    {
        private static GameObject CreatePrefabToReference(
            string sourcePath, string targetPath)
        {
            var copied = AssetDatabase.CopyAsset(sourcePath, targetPath);
            if (!copied) return null;
            
            AssetDatabase.Refresh();

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(targetPath);

            return prefab;
        }
        
        private static void CreateLevelSettingPart(
            Scene masterScene,
            string levelName,
            IEnumerable<GameEnvironment.WaypointPath> waypointPaths,
            List<Transform> spawnPoints,
            IEnumerable<string> texturePaths,
            string aStarGraphDataPath)
        {
            //
            var waypointPathBlobAssetAuthoringPrefabPath = Path.Combine(
                "Assets", "_", "1 - Game - Level Design", "Module - Environment - Level",
                "Common", "Prefabs", "Waypoint Path Blob Asset Authoring.prefab");
            
            var generatedWaypointPathBlobAssetAuthoringPrefabPath = Path.Combine(
                "Assets", "_", "1 - Game - Level Design - Generated", "Module - Environment - Level",
                "Levels", levelName, "level-setting", "Waypoint Path Blob Asset Authoring.prefab");

            var waypointPathBlobAssetAuthoringPrefab = CreatePrefabToReference(waypointPathBlobAssetAuthoringPrefabPath,
                generatedWaypointPathBlobAssetAuthoringPrefabPath);

            //
            var levelSettingBlobAssetAuthoringPrefabPath = Path.Combine(
                "Assets", "_", "1 - Game - Level Design", "Module - Environment - Level",
                "Common", "Prefabs", "Level Setting Blob Asset Authoring.prefab");
            
            var generatedLevelSettingBlobAssetAuthoringPrefabPath = Path.Combine(
                "Assets", "_", "1 - Game - Level Design - Generated", "Module - Environment - Level",
                "Levels", levelName, "level-setting", "Level Setting Blob Asset Authoring.prefab");

            var levelSettingBlobAssetAuthoringPrefab = CreatePrefabToReference(levelSettingBlobAssetAuthoringPrefabPath,
                generatedLevelSettingBlobAssetAuthoringPrefabPath);

            // var waypointPathBlobAssetAuthoringPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(waypointPathBlobAssetAuthoringPrefabPath);
            var waypointPathBlobAssetAuthoring = waypointPathBlobAssetAuthoringPrefab
                .GetComponent<GameEnvironment.WaypointPathBlobAssetAuthoring>();

            waypointPathBlobAssetAuthoring.waypointPaths = new List<GameEnvironment.WaypointPath>();
            waypointPathBlobAssetAuthoring.waypointPaths.AddRange(waypointPaths);

            // var aStarGraphDataPath = 
            //     var waypointPathBlobAssetAuthoringPrefabPath = Path.Combine(
            //     "Assets", "_", "1 - Game - Level Design", "Module - Environment - Level",
            //     "Common", "Prefabs", "Waypoint Path Blob Asset Authoring.prefab");
            
            //
            var levelSettingAsset = CreateLevelSetting(levelName, masterScene,
                waypointPaths, spawnPoints, texturePaths,
                waypointPathBlobAssetAuthoringPrefab,
                levelSettingBlobAssetAuthoringPrefab,
                aStarGraphDataPath);

            // var levelSettingBlobAssetAuthoringPrefabPath = Path.Combine(
            //     "Assets", "_", "1 - Game - Level Design", "Module - Environment - Level",
            //     "Common", "Prefabs", "Level Setting Blob Asset Authoring.prefab");

            // var levelSettingBlobAssetAuthoringPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(levelSettingBlobAssetAuthoringPrefabPath);
            var levelSettingBlobAssetAuthoring =
                levelSettingBlobAssetAuthoringPrefab.GetComponent<GameEnvironment.LevelSettingBlobAssetAuthoring>();
            
            // levelSettingBlobAssetAuthoring.levelSettingAsset = levelSettingAsset;
                
            var absoluteStartingPath = Application.dataPath;
            var relativeStartingPath = "Assets";
            var assetDirectoryPath = Path.Combine("_", "1 - Game - Level Design - Generated", "Module - Environment - Level",
                "Levels", levelName, "level-setting");
            
            {
                var assetName = "Level Setting.asset";
                
                GameCommonEditor.Utility.SaveAssetTo(absoluteStartingPath, relativeStartingPath, assetDirectoryPath, assetName, levelSettingAsset);
            }            
            // var assetDirectoryWaypointPath = Path.Combine("_", "1 - Game - Level Design - Generated",
            //     "Levels", "level004", "waypoint-path");
            //     
            //
            // {
            //     var prefabName = "Level Setting BlobAsset Authoring.prefab";
            //     
            //     // CommonEditorPart.Utility.SaveGameObjectAsPrefabTo(absoluteStartingPath, relativeStartingPath, assetDirectoryPath, prefabName, gameObject);
            //     // // Remove game object from scene after saving
            //     // // GameObject.DestroyImmediate(gameObject);
            //     // EditorSceneManager.SaveScene(currentMasterScene);
            //
            //     var prefabPath = Path.Combine(relativeStartingPath, assetDirectoryPath, prefabName);
            //     var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            //
            //     levelSettingAsset.levelSettingAuthoringPrefab = prefab;
            // }
        }
        
        private static GameEnvironment.LevelSetting CreateLevelSetting(
            string levelName,
            Scene masterScene,
            IEnumerable<GameEnvironment.WaypointPath> waypointPaths,
            List<Transform> spawnPoints,
            IEnumerable<string> texturePaths,
            GameObject waypointPathBlobAssetAuthoringPrefab,
            GameObject levelSettingBlobAssetAuthoringPrefab,
            string aStarGraphDataPath)
        {
            var levelOperator = GameCommon.Utility.GetComponentAtScene<LevelOperator>(masterScene);

            if (levelOperator == null) return null;

            var levelSetting = ScriptableObject.CreateInstance<GameEnvironment.LevelSetting>();

            // Way point
            levelSetting.waypointPaths = new List<GameEnvironment.WaypointPath>();
            levelSetting.waypointPaths.AddRange(waypointPaths);

            levelSetting.waypointPathAuthoringPrefab = waypointPathBlobAssetAuthoringPrefab;

            //
            levelSetting.levelSettingAuthoringPrefab = levelSettingBlobAssetAuthoringPrefab;

            levelSetting.hGridCount = levelOperator.xSubSceneCount;
            levelSetting.vGridCount = levelOperator.zSubSceneCount;
            levelSetting.gridCellCount = levelOperator.gridCount;
            
            levelSetting.aiControlCount = levelOperator.aiControlCount;
            
            levelSetting.spawnPoints = new List<Vector3>();
            levelSetting.spawnPoints.AddRange(spawnPoints.Select(x => x.position));

            //
            levelSetting.gridTextures = new List<Texture2D>();
            var obstacleTextures = texturePaths.Select(tp => AssetDatabase.LoadAssetAtPath<Texture2D>(tp));
            levelSetting.gridTextures.AddRange(obstacleTextures);
            
            var aStarGraphData = AssetDatabase.LoadAssetAtPath<TextAsset>(aStarGraphDataPath);
            levelSetting.astarGraphDatas = new List<TextAsset>();
            levelSetting.astarGraphDatas.Add(aStarGraphData);
            
            //
            levelSetting.sceneNames = new List<string>();
            levelSetting.sceneNames.Add(levelName);
            levelSetting.sceneNames.AddRange(levelOperator.subScenes.Select(sceneAsset => sceneAsset.name));

            return levelSetting;
        }
        
        
        private static void CreateLevelSettingPartEx(
            Scene masterScene,
            List<Transform> spawnPointList,
            IEnumerable<string> texturePaths,
            string aStarGraphDataPath)
        {
            var levelSettingAsset = CreateLevelSettingEx(masterScene, spawnPointList, texturePaths, aStarGraphDataPath);
            var gameObject = CreateLevelSettingBlobAssetAuthoringGameObject();
            var levelSettingBlobAssetAuthoring =
                gameObject.GetComponent<GameEnvironment.LevelSettingBlobAssetAuthoring>();

            var gridMapGameObject = CreateGridMapBlobAssetAuthoringGameObject();
            var gridMapBlobAssetAuthoring =
                gridMapGameObject.GetComponent<GameEnvironment.GridMapBlobAssetAuthoring>();

            if (levelSettingAsset != null
                && levelSettingBlobAssetAuthoring != null
                && gridMapBlobAssetAuthoring != null)
            {
                levelSettingBlobAssetAuthoring.levelSettingAsset = levelSettingAsset;
                
                var absoluteStartingPath = Application.dataPath;
                var relativeStartingPath = "Assets";
                var assetDirectoryPath = Path.Combine("_", "1 - Game - Level Design - Generated",
                    "Levels", "level001", "level-setting");
                
                var assetDirectoryWaypointPath = Path.Combine("_", "1 - Game - Level Design - Generated",
                    "Levels", "level001", "waypoint-path");
                
                //
                {
                    var prefabName = "Level Setting BlobAsset Authoring.prefab";
                
                    GameCommonEditor.Utility.SaveGameObjectAsPrefabTo(absoluteStartingPath, relativeStartingPath, assetDirectoryPath, prefabName, gameObject);
                    // Remove game object from scene after saving
                    GameObject.DestroyImmediate(gameObject);
                    EditorSceneManager.SaveScene(currentMasterScene);

                    var prefabPath = Path.Combine(relativeStartingPath, assetDirectoryPath, prefabName);
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                    levelSettingAsset.levelSettingAuthoringPrefab = prefab;
                }
                //
                {
                    var prefabName = "Waypoint Path BlobAsset Authoring.prefab";
                
                    var prefabPath = Path.Combine(relativeStartingPath, assetDirectoryWaypointPath, prefabName);
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                    levelSettingAsset.waypointPathAuthoringPrefab = prefab;
                }
                //
                {
                    var prefabName = "Grid Map BlobAsset Authoring.prefab";
                
                    GameCommonEditor.Utility.SaveGameObjectAsPrefabTo(absoluteStartingPath, relativeStartingPath, assetDirectoryPath, prefabName, gridMapGameObject);
                    // Remove game object from scene after saving
                    GameObject.DestroyImmediate(gridMapGameObject);
                    EditorSceneManager.SaveScene(currentMasterScene);

                    var prefabPath = Path.Combine(relativeStartingPath, assetDirectoryPath, prefabName);
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                    levelSettingAsset.gridMapAuthoringPrefab = prefab;
                }
                //
                {
                    var assetName = "Level Setting.asset";
                
                    GameCommonEditor.Utility.SaveAssetTo(absoluteStartingPath, relativeStartingPath, assetDirectoryPath, assetName, levelSettingAsset);
                }
            }
        }

        private static GameEnvironment.LevelSetting CreateLevelSettingEx(
            Scene masterScene,
            List<Transform> spawnPointList,
            IEnumerable<string> texturePaths,
            string aStarGraphDataPath)
        {
            var levelOperator = GameCommon.Utility.GetComponentAtScene<LevelOperator>(masterScene);

            if (levelOperator == null) return null;

            var levelSetting = ScriptableObject.CreateInstance<GameEnvironment.LevelSetting>();

            levelSetting.hGridCount = levelOperator.xSubSceneCount;
            levelSetting.vGridCount = levelOperator.zSubSceneCount;
            levelSetting.gridCellCount = levelOperator.gridCount;

            levelSetting.aiControlCount = levelOperator.aiControlCount;

            levelSetting.spawnPoints = new List<Vector3>();
            levelSetting.spawnPoints.AddRange(spawnPointList.Select(x => x.position));
            
            levelSetting.gridTextures = new List<Texture2D>();
            var obstacleTextures = texturePaths.Select(tp => AssetDatabase.LoadAssetAtPath<Texture2D>(tp));
            levelSetting.gridTextures.AddRange(obstacleTextures);

            var aStarGraphData = AssetDatabase.LoadAssetAtPath<TextAsset>(aStarGraphDataPath);
            levelSetting.astarGraphDatas = new List<TextAsset>();
            levelSetting.astarGraphDatas.Add(aStarGraphData);

            return levelSetting;
        }
        
        private static GameObject CreateLevelSettingBlobAssetAuthoringGameObject()
        {
            var createdInstance = new GameObject();
            createdInstance.AddComponent<GameEnvironment.LevelSettingBlobAssetAuthoring>();
            createdInstance.AddComponent<ConvertToEntity>();
            
            return createdInstance;
        }
    }
}
