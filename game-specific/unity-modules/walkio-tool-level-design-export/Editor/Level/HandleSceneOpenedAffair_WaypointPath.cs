// namespace JoyBrick.Walkio.Build.LevelDesignExport.EditorPart
// {
//     using System;
//     using System.Collections.Generic;
//     using System.IO;
//     using System.Linq;
//     using Unity.Entities;
//     using UnityEditor;
//     using UnityEditor.SceneManagement;
//     using UnityEngine;
//     using UnityEngine.SceneManagement;
//
//     //
//     using GameCommon = JoyBrick.Walkio.Game.Common;
//     using GameCommonEditor = JoyBrick.Walkio.Game.Common.EditorPart;
//     using GameEnvironment = JoyBrick.Walkio.Game.Environment;
//
//     // TODO: Rename, this class name brings only confusion
//     public static partial class HandleSceneOpenedAffair
//     {
//         private static List<GameEnvironment.WaypointPath> CreateWaypointPathPart(Scene masterScene)
//         {
//             var waypointPaths = CreateWaypointData(masterScene);
//
//             return waypointPaths;
//         }
//
//         // private static void CreateWaypointPathPart(Scene masterScene)
//         // {
//         //     //
//         //     var crossProjectData = AssetDatabase.LoadAssetAtPath<CrossProject.CrossProjectData>(
//         //         "Packages/walkio.cross-project/Data Assets/Cross Project Data.asset");
//         //
//         //     //
//         //     var relativeAssetFolderName = "Assets";
//         //     var projectBaseFolderName = crossProjectData.commonProjectData.projectBaseFolderName;
//         //     var baseFolderName = crossProjectData.assetLevelDesignProjectData.baseFolderName;
//         //     var levelModuleFolderName = crossProjectData.assetLevelDesignProjectData.levelModuleFolderName;
//         //     var generationBaseFolderName = crossProjectData.assetLevelDesignProjectData.generationBaseFolderName;
//         //
//         //     // var waypointDataAsset = CreateWaypointData(masterScene);
//         //     var waypointPaths = CreateWaypointData(masterScene);
//         //     var gameObject = CreateWaypointPathBlobAssetAuthoringGameObject();
//         //     var waypointPathBlobAssetAuthoring =
//         //         gameObject.GetComponent<GameEnvironment.WaypointPathBlobAssetAuthoring>();
//         //
//         //     if (waypointPaths.Any()
//         //         && waypointPathBlobAssetAuthoring != null)
//         //     {
//         //         waypointPathBlobAssetAuthoring.waypointPaths = waypointPaths;
//         //
//         //         var absoluteStartingPath = Application.dataPath;
//         //         var relativeStartingPath = "Assets";
//         //         var assetDirectoryPath = Path.Combine(projectBaseFolderName, generationBaseFolderName,
//         //             levelModuleFolderName, "level001", "waypoint-path");
//         //
//         //         // //
//         //         // {
//         //         //     var assetName = "Waypoint Data.asset";
//         //         //
//         //         //     CommonEditorPart.Utility.SaveAssetTo(absoluteStartingPath, relativeStartingPath, assetDirectoryPath, assetName, waypointDataAsset);
//         //         // }
//         //         //
//         //         {
//         //             var prefabName = "Waypoint Path BlobAsset Authoring.prefab";
//         //
//         //             CommonEditorPart.Utility.SaveGameObjectAsPrefabTo(absoluteStartingPath, relativeStartingPath, assetDirectoryPath, prefabName, gameObject);
//         //             // Remove game object from scene after saving
//         //             GameObject.DestroyImmediate(gameObject);
//         //             EditorSceneManager.SaveScene(currentMasterScene);
//         //         }
//         //     }
//         // }
//
//         private static List<GameEnvironment.WaypointPath> CreateWaypointData(Scene masterScene)
//         {
//             var levelOperator = GameCommon.Utility.SceneHelper.GetComponentAtScene<LevelOperator>(masterScene);
//
//             if (levelOperator == null) return null;
//
//             var curvyPathContainer = levelOperator.curvyPathContainer;
//
//             //
//             var waypointPaths = new List<GameEnvironment.WaypointPath>();
//             foreach (Transform curvy in curvyPathContainer.transform)
//             {
//                 // curvy.GetComponent<CurvySpline>()
//
//                 var wp = new GameEnvironment.WaypointPath();
//                 wp.waypoints = new List<GameEnvironment.Waypoint>();
//                 waypointPaths.Add(wp);
//                 foreach (Transform waypoint in curvy)
//                 {
//                     // TODO: Check to see if the position is in world and affected by parent position
//                     wp.waypoints.Add(new GameEnvironment.Waypoint
//                     {
//                         location = waypoint.position
//                     });
//                     // Debug.Log(waypoint);
//                 }
//             }
//
//             return waypointPaths;
//         }
//
//         private static GameObject CreateWaypointPathBlobAssetAuthoringGameObject()
//         {
//             // TODO: This creates new game object on scene, which has to be removed later to no alert the scene
//             var createdInstance = new GameObject();
//             createdInstance.AddComponent<GameEnvironment.WaypointPathBlobAssetAuthoring>();
//             createdInstance.AddComponent<ConvertToEntity>();
//
//             return createdInstance;
//         }
//     }
// }
