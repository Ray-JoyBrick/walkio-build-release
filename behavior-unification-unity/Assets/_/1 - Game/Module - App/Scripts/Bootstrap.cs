// namespace JoyBrick.Walkio.Game
// {
//     using System;
//     using System.Collections;
//     using System.Collections.Generic;
//     using HellTap.PoolKit;
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
//     using GameHudApp = JoyBrick.Walkio.Game.Hud.App;
//     using GameHudPreparation = JoyBrick.Walkio.Game.Hud.Preparation;
//     using GameTemplate = JoyBrick.Walkio.Game.Template;

//     public partial class Bootstrap :
//         MonoBehaviour,
//         GameCommon.IServiceManagement,
//         GameCommon.ICommandHandler
//     {
//         //
//         private EntityManager _entityManager;

//         // private EntityArchetype _loadZoneRequestEventArchetype;

//         //
//         private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

//         private IObservable<int> SetupECSDone => _notifySetupECSDone.AsObservable();
//         private readonly Subject<int> _notifySetupECSDone = new Subject<int>();
        
//         //
//         void Start()
//         {
//             SetupECSDone
//                 .Subscribe(x =>
//                 {
//                     _notifyLoadAppHud.OnNext(1);
//                 })
//                 .AddTo(_compositeDisposable);
            
//             //
//             SetupECSWorld();
            
//             //
//             SetupAddressable();
            
//             // Can actually group by using visual scripting tool
//             // For now, just simulate the flow by sending timed event
//             Observable.Timer(System.TimeSpan.FromMilliseconds(500))
//                 .Subscribe(_ =>
//                 {
//                     //
//                     _notifyInitializingEnvironment.OnNext(0);
//                 })
//                 .AddTo(_compositeDisposable);
            
//             // Observable.Timer(System.TimeSpan.FromMilliseconds(3000))
//             //     .Subscribe(_ =>
//             //     {
//             //         //
//             //         // _entityManager.CreateEntity(_loadZoneRequestEventArchetype);
//             //
//             //         
//             //         //
//             //         if (_zoneSceneLoaded)
//             //         {
//             //             // Should just removing additive scene before calling loading world
//             //             var asyncOp = SceneManager.UnloadSceneAsync(_zoneScene);
//             //
//             //             asyncOp.AsObservable()
//             //                 .Subscribe(x =>
//             //                 {
//             //                     _notifyLoadingWorld.OnNext(0);
//             //                 })
//             //                 .AddTo(_compositeDisposable);
//             //         }
//             //         else
//             //         {
//             //             _notifyLoadingWorld.OnNext(0);
//             //         }
//             //     })
//             //     .AddTo(_compositeDisposable);


//         }

//         private void SetupECSWorld()
//         {
//             //
//             _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

//             // _loadZoneRequestEventArchetype = _entityManager.CreateArchetype(
//             //     typeof(GameCommon.LoadZoneRequest));

//             var environmentArchetype =
//                 _entityManager.CreateArchetype(typeof(GameEnvironment.TheEnvironment));

//             var zoneArchetype =
//                 _entityManager.CreateArchetype(
//                     typeof(GameEnvironment.Zone));

//             _entityManager.CreateEntity(environmentArchetype);
//             _entityManager.CreateEntity(zoneArchetype);
//         }

//         private void SetupAddressable()
//         {
//             var addressableInitializeAsync = Addressables.InitializeAsync();
            
//             // This might cause Exception: Attempting to use an invalid operation handle
//             // Workaround is to not unregister the event
//             var addressableInitializeAsyncObservable =
//                 Observable
//                     .FromEvent<AsyncOperationHandle<IResourceLocator>>(
//                         h => addressableInitializeAsync.Completed += h,
//                         h => { });
//             addressableInitializeAsyncObservable                
//                 .Subscribe(x =>
//                 {
//                     //
//                     Debug.Log($"Bootstrap - addressableInitializeAsync is received");

//                     HandleAddressableInitializeAsyncCompleted();
//                     _notifySetupECSDone.OnNext(1);
//                 })
//                 .AddTo(_compositeDisposable);            
//         }

//         private void HandleAddressableInitializeAsyncCompleted()
//         {
//             Debug.Log($"Bootstrap - HandleAddressableInitializeAsyncCompleted");

//             //
//             var group = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();
            
//             // Hud - App
//             var appHudLoadSceneSystem =
//                 World.DefaultGameObjectInjectionWorld
//                     .GetOrCreateSystem<GameHudApp.HudLoadSceneSystem>();
//             var processDoozyMessageSystem =
//                 World.DefaultGameObjectInjectionWorld
//                     .GetOrCreateSystem<GameHudApp.ProcessDoozyMessageSystem>();
            
//             // Hud - Preparation
//             var preparationHudLoadSceneSystem =
//                 World.DefaultGameObjectInjectionWorld
//                     .GetOrCreateSystem<GameHudPreparation.HudLoadSceneSystem>();
//             var preparationHudUnloadSceneSystem =
//                 World.DefaultGameObjectInjectionWorld
//                     .GetOrCreateSystem<GameHudPreparation.HudUnloadSceneSystem>();

//             // Environment
//             var loadEnvironmentTemplateSystem = World.DefaultGameObjectInjectionWorld
//                 .GetOrCreateSystem<GameEnvironment.LoadEnvironmentTemplateSystem>();
//             var loadZoneTemplateSystem = World.DefaultGameObjectInjectionWorld
//                 .GetOrCreateSystem<GameEnvironment.LoadZoneTemplateSystem>();
//             var generateZoneSystem = World.DefaultGameObjectInjectionWorld
//                 .GetOrCreateSystem<GameEnvironment.GenerateZoneSystem>();

//             var generatePathfindSystem = World.DefaultGameObjectInjectionWorld
//                 .GetOrCreateSystem<GameEnvironment.GeneratePathfindSystem>();

//             var removeConvertedSystem =
//                 World.DefaultGameObjectInjectionWorld
//                     .GetOrCreateSystem<GameEnvironment.RemoveConvertedSystem>();

//             // Explicitly assign the dependencies manually
//             appHudLoadSceneSystem.ServiceManagement = (GameCommon.IServiceManagement) this;
//             processDoozyMessageSystem.ServiceManagement = (GameCommon.IServiceManagement) this;
            
//             preparationHudLoadSceneSystem.ServiceManagement = (GameCommon.IServiceManagement) this;
//             preparationHudLoadSceneSystem.CommandHandler = (GameCommon.ICommandHandler) this;
//             preparationHudUnloadSceneSystem.ServiceManagement = (GameCommon.IServiceManagement) this;
            
//             loadEnvironmentTemplateSystem.EnvironmentSetupRequester = (GameCommon.IEnvironmentSetupRequester) this;
//             loadZoneTemplateSystem.WorldLoadingRequester = (GameCommon.IWorldLoadingRequester) this;
            
//             //
//             group.AddSystemToUpdateList(appHudLoadSceneSystem);
//             group.AddSystemToUpdateList(processDoozyMessageSystem);

//             group.AddSystemToUpdateList(preparationHudLoadSceneSystem);
//             group.AddSystemToUpdateList(preparationHudUnloadSceneSystem);
            
//             group.AddSystemToUpdateList(loadEnvironmentTemplateSystem);
//             group.AddSystemToUpdateList(loadZoneTemplateSystem);
//             group.AddSystemToUpdateList(generateZoneSystem);
//             group.AddSystemToUpdateList(generatePathfindSystem);
            
//             group.AddSystemToUpdateList(removeConvertedSystem);
            
//             //
//             appHudLoadSceneSystem.Setup();
//             processDoozyMessageSystem.Setup();

//             preparationHudLoadSceneSystem.Setup();
//             preparationHudUnloadSceneSystem.Setup();
            
//             loadEnvironmentTemplateSystem.Setup();
//             loadZoneTemplateSystem.Setup();
//         }

//         private void OnDestroy()
//         {
//             _compositeDisposable?.Dispose();
//         }

//         #region
        
//         public void LoadZone(int index)
//         {
//             _notifyLoadingWorld.OnNext(0);
//         }
        
//         #endregion

//         #region

//         public IObservable<int> LoadAppHud => _notifyLoadAppHud.AsObservable();
//         private readonly Subject<int> _notifyLoadAppHud = new Subject<int>();

//         public void LoadAppHudDone()
//         {
//             _notifyLoadPreparationHud.OnNext(1);
//         }

//         public IObservable<int> LoadPreparationHud => _notifyLoadPreparationHud.AsObservable();
//         private readonly Subject<int> _notifyLoadPreparationHud = new Subject<int>();

//         public IObservable<int> UnloadPreparationHud => _notifyUnloadPreparationHud.AsObservable();
//         private readonly Subject<int> _notifyUnloadPreparationHud = new Subject<int>();

//         #endregion
//     }
// }
