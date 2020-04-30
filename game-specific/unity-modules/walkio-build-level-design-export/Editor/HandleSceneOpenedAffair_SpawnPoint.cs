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

    // TODO: Rename, this class name brings only confusion
    public static partial class HandleSceneOpenedAffair
    {
        private static List<Transform> CreateSpawnPart(Scene masterScene)
        {
            var spawnPointList = CreateSpawnPointList(masterScene);

            return spawnPointList;
        }

        private static List<Transform> CreateSpawnPointList(Scene masterScene)
        {
            var levelOperator = GetComponentAtScene<LevelOperator>(masterScene);

            if (levelOperator == null) return null;

            var spawnPointContainer = levelOperator.spawnPointContainer;

            var sps = new List<Transform>();
            
            //
            foreach (Transform spawnPoint in spawnPointContainer.transform)
            {
                sps.Add(spawnPoint);
            }

            return sps;
        }
    }
}
