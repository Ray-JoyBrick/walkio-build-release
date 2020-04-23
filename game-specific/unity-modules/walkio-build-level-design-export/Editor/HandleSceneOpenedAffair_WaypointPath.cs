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
        private static void CreateWaypointPathPart(Scene masterScene)
        {
            var waypointDataAsset = CreateWaypointData(masterScene);
            var gameObject = CreateWaypointPathBlobAssetAuthoringGameObject();
            var waypointPathBlobAssetAuthoring =
                gameObject.GetComponent<GameEnvironment.WaypointPathBlobAssetAuthoring>();

            if (waypointDataAsset != null
                && waypointPathBlobAssetAuthoring != null)
            {
                waypointPathBlobAssetAuthoring.waypointDataAsset = waypointDataAsset;

                var absoluteStartingPath = Application.dataPath;
                var relativeStartingPath = "Assets";
                var assetDirectoryPath = Path.Combine("_", "1 - Game - Level Design - Generated",
                    "Levels", "level001", "waypoint-path");

                //
                {
                    var assetName = "Waypoint Data.asset";

                    SaveAssetTo(absoluteStartingPath, relativeStartingPath, assetDirectoryPath, assetName, waypointDataAsset);
                }
                //
                {
                    var prefabName = "Waypoint Path BlobAsset Authoring.prefab";

                    SaveGameObjectAsPrefabTo(absoluteStartingPath, relativeStartingPath, assetDirectoryPath, prefabName, gameObject);
                }
            }
        }

        private static GameEnvironment.WaypointData CreateWaypointData(Scene masterScene)
        {
            var levelOperator = GetComponentAtScene<LevelOperator>(masterScene);

            if (levelOperator == null) return null;

            var curvyRoot = levelOperator.curvy;

            //
            var waypointData = ScriptableObject.CreateInstance<GameEnvironment.WaypointData>();

            waypointData.waypointPaths = new List<GameEnvironment.WaypointPath>();
            foreach (Transform curvy in curvyRoot.transform)
            {
                // curvy.GetComponent<CurvySpline>()

                var wp = new GameEnvironment.WaypointPath();
                wp.waypoints = new List<GameEnvironment.Waypoint>();
                waypointData.waypointPaths.Add(wp);
                foreach (Transform waypoint in curvy)
                {
                    wp.waypoints.Add(new GameEnvironment.Waypoint
                    {
                        location = waypoint.position
                    });
                    Debug.Log(waypoint);
                }
            }

            return waypointData;
        }

        private static GameObject CreateWaypointPathBlobAssetAuthoringGameObject()
        {
            var createdInstance = new GameObject();
            createdInstance.AddComponent<GameEnvironment.WaypointPathBlobAssetAuthoring>();
            createdInstance.AddComponent<ConvertToEntity>();
            
            return createdInstance;
        }
    }
}
