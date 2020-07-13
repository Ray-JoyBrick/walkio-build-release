// namespace JoyBrick.Walkio.Game.Pathfinding
// {
//     using System;
//     using System.Collections.Generic;
//     using UniRx;
//     using Unity.Entities;
//     using UnityEngine;
//
//     using GameCommon = JoyBrick.Walkio.Game.Common;
//
//     [DisableAutoCreation]
//     public class LoadAStarNavMeshSystem :
//         SystemBase
//     {
//         //
//         public Common.IAssetLoadingService assetLoadingService;        
//         public GameCommon.IWorldLoading worldLoading;
//         
//         private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
//         
//         protected override void OnCreate()
//         {
//             base.OnCreate();
//
//             // LoadAssetStream()
//             worldLoading.LoadingWorld
//                 .Subscribe(x =>
//                 {
//                     //
//                     assetLoadingService.LoadAsset<TextAsset>("", ta =>
//                     {
//                         //
//                     });
//                 })
//                 .AddTo(_compositeDisposable);
//         }
//
//         protected override void OnUpdate()
//         {
//         }
//
//         protected override void OnDestroy()
//         {
//             base.OnDestroy();
//             
//             _compositeDisposable?.Dispose();
//         }
//     }
// }
