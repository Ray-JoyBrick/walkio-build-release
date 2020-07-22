// namespace JoyBrick.Walkio.Game.Hud.App
// {
//     using System;
//     using System.Threading.Tasks;
//     using UniRx;
//     using Unity.Entities;
//     using UnityEngine;
//     using UnityEngine.AddressableAssets;
//     using UnityEngine.ResourceManagement.ResourceProviders;
//     using UnityEngine.SceneManagement;
//     
//     using GameCommon = JoyBrick.Walkio.Game.Common;
//     using GameCommand = JoyBrick.Walkio.Game.Command;
//     using GameExtension = JoyBrick.Walkio.Game.Extension;
//
//     [DisableAutoCreation]
//     public class CleanupAppHudSystem : SystemBase
//     {
//         //
//         private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
//
//         //
//         private GameObject _canvasPrefab;
//         private GameObject _viewLoadingPrefab;
//         private ScriptableObject _timelineAsset;
//         private ScriptableObject _i2Asset;
//
//         //
//         private GameObject _canvas;
//
//         // private View _loadView;
//
//         //
//         public GameCommand.ICommandService CommandService { get; set; }
//         public GameCommon.IFlowControl FlowControl { get; set; }
//
//         //
//         public void Construct()
//         {
//             base.OnCreate();
//             
//             //
//             FlowControl.CleaningAsset
//                 .Where(x => x.Name.Contains("App"))
//                 .Subscribe(x =>
//                 {
//                     CleaningAsset();
//                 })
//                 .AddTo(_compositeDisposable);            
//         }
//
//         private void CleaningAsset()
//         {
//             // LoadAppHudSystem removing assets
//         }
//
//         protected override void OnUpdate() {}
//         
//         protected override void OnDestroy()
//         {
//             base.OnDestroy();
//
//             //
//             Addressables.ReleaseInstance(_canvasPrefab);
//             Addressables.ReleaseInstance(_viewLoadingPrefab);
//             Addressables.Release(_timelineAsset);
//             Addressables.Release(_i2Asset);
//             
//             //
//             GameObject.Destroy(_canvas);
//             
//             _compositeDisposable?.Dispose();
//         }
//     }
// }
