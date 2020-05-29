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
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;

    public static partial class HandleSceneOpenedAffair
    {
        private static string CreateAStarGraphPart(string inLevelName, string outLevelName, Scene masterScene)
        {
            var path =
                MakeAStarPathfindingData(inLevelName, outLevelName, masterScene);

            return path;
        }
        
        private static string MakeAStarPathfindingData(string inLevelName, string outLevelName, Scene masterScene)
        {
            Debug.Log($"MakeAStarPathfindingData - inLevelName: {inLevelName} outLevelName: {outLevelName}");
            // var astarPath = GetPathfinderAtScene(masterScene);
            // if (astarPath == null) return;
            //
            // astarPath
            
            // AstarPath.active.Scan();
            
            var generatedDirectoryPath = Path.Combine(Application.dataPath, "_", "1 - Game - Level Design - Generated", "Module - Environment - Level");
            var levelDirectoryPath = Path.Combine(generatedDirectoryPath, "Levels");
            GameCommon.Utility.CreateDirectoryIfNotExisted(generatedDirectoryPath);
            GameCommon.Utility.CreateDirectoryIfNotExisted(levelDirectoryPath);
            
            var specificLevelDirectoryPath = Path.Combine(levelDirectoryPath, outLevelName);
            GameCommon.Utility.CreateDirectoryIfNotExisted(specificLevelDirectoryPath);

            var obstacleTextureDirectoryPath = Path.Combine(specificLevelDirectoryPath, "astar-data");
            GameCommon.Utility.CreateDirectoryIfNotExisted(obstacleTextureDirectoryPath);
            
            //

            var aStarGraphDataAssetPath = Path.Combine("Assets", "_", "1 - Game - Level Design",
                "Module - Environment - Level", inLevelName);
            var relativeAStarGraphDataAssetPath = Path.Combine(aStarGraphDataAssetPath, "Data Assets", $"graph.bytes");

            // var originalGraphData = AssetDatabase.LoadAssetAtPath<TextAsset>(relativeAStarGraphDataAssetPath);

            var aStarGraphDataAssetGeneratedPath = Path.Combine("Assets", "_", "1 - Game - Level Design - Generated",
                "Module - Environment - Level",
                "Levels", outLevelName, "astar-data");
            var relativeAStarGraphDataAssetGeneratedPath = Path.Combine(aStarGraphDataAssetGeneratedPath, $"graph.bytes");

            AssetDatabase.CopyAsset(relativeAStarGraphDataAssetPath, relativeAStarGraphDataAssetGeneratedPath);

            return relativeAStarGraphDataAssetGeneratedPath;
        }
    }
}
