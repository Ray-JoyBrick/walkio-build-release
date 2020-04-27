// namespace JoyBrick.Walkio.Game
// {
//     using System;
//     using System.Collections;
//     using System.Collections.Generic;
//     using UniRx;
//     using Unity.Entities;
//     using UnityEngine;
//     using UnityEngine.AddressableAssets;
//     using UnityEngine.AddressableAssets.ResourceLocators;
//     using UnityEngine.ResourceManagement.AsyncOperations;
//     using UnityEngine.ResourceManagement.ResourceProviders;
//     using UnityEngine.SceneManagement;
//     using GameCommon = JoyBrick.Walkio.Game.Common;
//     using GameEnvironment = JoyBrick.Walkio.Game.Environment;
//     using GameTemplate = JoyBrick.Walkio.Game.Template;

//     public partial class Bootstrap :
//         GameCommon.IEnvironmentSetupRequester,
//         GameCommon.IWorldLoadingRequester
//     {
//         //
//         private GameTemplate.EnvironmentData _environmentData;

//         private bool _zoneSceneLoaded;
//         private Scene _zoneScene;
        
//         #region Related to IEnvironmentSetupRequester

//         public IObservable<int> InitializingEnvironment => _notifyInitializingEnvironment.AsObservable();
//         private readonly Subject<int> _notifyInitializingEnvironment = new Subject<int>();

//         public void SetEnvironmentData(ScriptableObject scriptableObject)
//         {
//             _environmentData = scriptableObject as GameTemplate.EnvironmentData;
//         }

//         #endregion
        
//         #region Related to IWorldLoadingRequester

//         public IObservable<int> LoadingWorld => _notifyLoadingWorld.AsObservable();
//         private readonly Subject<int> _notifyLoadingWorld = new Subject<int>();

//         public void SetZoneScene(Scene scene)
//         {
//             _zoneSceneLoaded = true;
//             _zoneScene = scene;
//         }

//         public void SetupPathfindingData(TextAsset textAsset)
//         {
//             Debug.Log($"Bootstrap - SetupPathfindingData - graph data size: {textAsset.bytes.Length}");
            
//             AstarPath.active.data.DeserializeGraphs(textAsset.bytes);
//             Observable.FromCoroutine(PathScan)
//                 .Subscribe(x =>
//                 {
//                     Debug.Log($"Bootstrap - SetupPathfindingData - A star path data scan is finished");
//                 })
//                 .AddTo(_compositeDisposable);
//         }

//         // A star pathfinding uses coroutine, so define one here
//         IEnumerator PathScan()
//         {
//             foreach (var progress in AstarPath.active.ScanAsync())
//             {
//                 yield return null;
//             }
//         }

//         #endregion
//     }
// }
