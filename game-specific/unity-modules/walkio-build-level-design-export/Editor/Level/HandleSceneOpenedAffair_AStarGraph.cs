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
    using Common = JoyBrick.Walkio.Common;
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
            Common.Utility.CreateDirectoryIfNotExisted(generatedDirectoryPath);
            Common.Utility.CreateDirectoryIfNotExisted(levelDirectoryPath);
            
            var specificLevelDirectoryPath = Path.Combine(levelDirectoryPath, outLevelName);
            Common.Utility.CreateDirectoryIfNotExisted(specificLevelDirectoryPath);

            var obstacleTextureDirectoryPath = Path.Combine(specificLevelDirectoryPath, "astar-data");
            Common.Utility.CreateDirectoryIfNotExisted(obstacleTextureDirectoryPath);
            
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
