// namespace JoyBrick.Walkio.Build.LevelDesignExport
// {
//     using System.Collections.Generic;
// #if ODIN_INSPECTOR
//     using Sirenix.OdinInspector;
// #endif
// #if UNITY_EDITOR
//     using UnityEditor;
// #endif
//     using UnityEngine;
//
//     // The entire stuffs should just move to Editor scope so that when build, can just get rid
//     // of this editor only functionality without any issue.
//     public partial class LevelOperator : MonoBehaviour
//     {
// #if UNITY_EDITOR
//         public List<SceneAsset> subScenes;
// #endif
//         
//         [HideInInspector]
//         public string buttonCreateInBoundaryLayer = "Create In Boundary Layer";
//         [HideInInspector]
//         public string buttonCreateOutBoundaryLayer = "Create Out Boundary Layer";
//         [HideInInspector]
//         public string buttonCreateObstacleLayer = "Create Obstacle Layer";
//         [HideInInspector]
//         public string buttonCreateAreaLayer = "Create Area Layer";
//       
// #if ODIN_INSPECTOR   
//         [Button("$buttonCreateInBoundaryLayer")]
// #endif
//         private void HandleButtonCreateInBoundaryLayerInteracted()
//         {
// #if UNITY_EDITOR
//             var sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
//             for (var i = 0; i < sceneCount; ++i)
//             {
//                 var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
//                 var isMasterScene = scene.name.Contains("Master");
//                 if (!isMasterScene)
//                 {
//                     // scene.GetRootGameObjects()
//                 }
//             }
// #endif
//         }
//
// #if ODIN_INSPECTOR   
//         [Button("$buttonCreateOutBoundaryLayer")]
// #endif
//         private void HandleButtonCreateOutBoundaryLayerInteracted()
//         {
// #if UNITY_EDITOR
//             
// #endif
//         }
//
// #if ODIN_INSPECTOR   
//         [Button("$buttonCreateObstacleLayer")]
// #endif
//         private void HandleButtonCreateObstacleLayerInteracted()
//         {
// #if UNITY_EDITOR
//             
// #endif
//         }
//
// #if ODIN_INSPECTOR   
//         [Button("$buttonCreateAreaLayer")]
// #endif
//         private void HandleButtonCreateAreaLayerInteracted()
//         {
// #if UNITY_EDITOR
//             
// #endif
//         }
//     }
// }
