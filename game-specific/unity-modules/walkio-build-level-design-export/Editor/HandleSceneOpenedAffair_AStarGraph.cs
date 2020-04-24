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
        private static void CreateAStarGraphPart(Scene masterScene)
        {
            MakeAStarPathfindingData(masterScene);
        }
        
        private static void MakeAStarPathfindingData(Scene masterScene)
        {
            Debug.Log($"MakeAStarPathfindingData");
            // var astarPath = GetPathfinderAtScene(masterScene);
            // if (astarPath == null) return;
            //
            // astarPath
            
            AstarPath.active.Scan();
        }
    }
}
