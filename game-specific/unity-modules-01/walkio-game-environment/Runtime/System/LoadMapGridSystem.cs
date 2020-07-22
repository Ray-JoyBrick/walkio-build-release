// namespace JoyBrick.Walkio.Game.Environment
// {
//     using System.Threading.Tasks;
//     using UniRx;
//     using Unity.Entities;
//     using UnityEngine;
//     using UnityEngine.AddressableAssets;
//     using GameCommon = JoyBrick.Walkio.Game.Common;
//     
//     [DisableAutoCreation]
//     public class LoadMapGridSystem :
//         SystemBase
//     {
//         public GameCommon.IWorldLoading WorldLoading { get; set; }
//         
//         private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
//         private EntityQuery _eventQuery;
//
//         class Context
//         {
//             public int index;
//             public ScriptableObject scriptableObject;
//             public GameObject prefab;
//         }
//         
//         protected override void OnCreate()
//         {
//             base.OnCreate();
//             
//             //
//             _eventQuery = GetEntityQuery(new EntityQueryDesc
//             {
//                 All = new ComponentType[]
//                 {
//                     ComponentType.ReadOnly<Common.LoadWorldMapRequest>() 
//                 }
//             });
//
//             WorldLoading.LoadingWorld
//                 .Subscribe(x =>
//                 {
//                     //
//                     Load().ToObservable()
//                         .ObserveOnMainThread()
//                         .SubscribeOnMainThread()
//                         .Subscribe(result =>
//                         {
//                             //
//                             var entity = _eventQuery.GetSingletonEntity();
//
//                             var (scriptableObject, prefab) = result;
//                             var castedSO = scriptableObject as Bridge.TileDetailAsset;
//                             
//                             // EntityManager.SetComponentData(entity);
//                         })
//                         .AddTo(_compositeDisposable);
//                 })
//                 .AddTo(_compositeDisposable);
//
//             // var ob1 = WorldLoading.LoadingWorld.Select(x => new Context { index = x});
//             // var ob2 = Load().ToObservable().Select(x => new Context {  });
//             //
//             // ob1.Concat(ob2)
//             //     .ObserveOnMainThread()
//             //     .SubscribeOnMainThread()
//             //     .Buffer(2)
//             //     .Subscribe(x =>
//             //     {
//             //         //
//             //         var index = x[0].index;
//             //         
//             //     })
//             //     .AddTo(_compositeDisposable);
//         }
//
//         private async Task<(ScriptableObject, GameObject)> Load()
//         {
//             var handle1 = Addressables.LoadAssetAsync<ScriptableObject>("");
//             var handle2 = Addressables.LoadAssetAsync<GameObject>("");
//             // var result = await handle.Task;
//
//             var r1 = await handle1.Task;
//             var r2 = await handle2.Task;
//
//             return (r1, r2);
//         }
//
//         protected override void OnUpdate()
//         {
//             throw new System.NotImplementedException();
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
