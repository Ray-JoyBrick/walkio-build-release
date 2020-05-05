namespace JoyBrick.Walkio.Build.LevelDesignExport.Editor
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

    using GameEnvironment = JoyBrick.Walkio.Game.Environment;

    public static partial class HandleSceneOpenedAffair
    {
        private static void CreateLevelSettingPart(
            Scene masterScene,
            List<Transform> spawnPointList,
            IEnumerable<string> texturePaths,
            string aStarGraphDataPath)
        {
            var levelSettingAsset = CreateLevelSetting(masterScene, spawnPointList, texturePaths, aStarGraphDataPath);
            var gameObject = CreateLevelSettingBlobAssetAuthoringGameObject();
            var levelSettingBlobAssetAuthoring =
                gameObject.GetComponent<GameEnvironment.LevelSettingBlobAssetAuthoring>();

            if (levelSettingAsset != null
                && levelSettingBlobAssetAuthoring != null)
            {
                levelSettingBlobAssetAuthoring.levelSettingAsset = levelSettingAsset;
                
                var absoluteStartingPath = Application.dataPath;
                var relativeStartingPath = "Assets";
                var assetDirectoryPath = Path.Combine("_", "1 - Game - Level Design - Generated",
                    "Levels", "level001", "level-setting");
                
                //
                {
                    var assetName = "Level Setting.asset";
                
                    SaveAssetTo(absoluteStartingPath, relativeStartingPath, assetDirectoryPath, assetName, levelSettingAsset);
                }
                //
                {
                    var prefabName = "Level Setting BlobAsset Authoring.prefab";
                
                    SaveGameObjectAsPrefabTo(absoluteStartingPath, relativeStartingPath, assetDirectoryPath, prefabName, gameObject);
                    // Remove game object from scene after saving
                    GameObject.DestroyImmediate(gameObject);
                    EditorSceneManager.SaveScene(currentMasterScene);
                }
            }
        }

        private static GameEnvironment.LevelSetting CreateLevelSetting(
            Scene masterScene,
            List<Transform> spawnPointList,
            IEnumerable<string> texturePaths,
            string aStarGraphDataPath)
        {
            var levelOperator = GetComponentAtScene<LevelOperator>(masterScene);

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
