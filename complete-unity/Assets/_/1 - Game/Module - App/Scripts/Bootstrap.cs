namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using Unity.XR.OpenVR;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.AddressableAssets.ResourceLocators;
    using UnityEngine.LowLevel;
    using UnityEngine.ResourceManagement.AsyncOperations;
    
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;

    public class Bootstrap :
        MonoBehaviour
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private void Start()
        {
            //
            SetupAddressable();
        }
        
        private void SetupAddressable()
        {
            var addressableInitializeAsync = Addressables.InitializeAsync();
            
            // This might cause Exception: Attempting to use an invalid operation handle
            // Workaround is to not unregister the event
            var addressableInitializeAsyncObservable =
                Observable
                    .FromEvent<AsyncOperationHandle<IResourceLocator>>(
                        h => addressableInitializeAsync.Completed += h,
                        h => { });
            addressableInitializeAsyncObservable                
                .Subscribe(x =>
                {
                    //
                    Debug.Log($"Bootstrap - addressableInitializeAsync is received");

                    HandleAddressableInitializeAsyncCompleted();
                })
                .AddTo(_compositeDisposable);            
        }

        private void HandleAddressableInitializeAsyncCompleted()
        {
            var initializationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();

            {
                var loadAssetSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<Hud.App.LoadAssetSystem>();
            
                initializationSystemGroup.AddSystemToUpdateList(loadAssetSystem);
            }
        }

        private void CreateWorld()
        {
            var world = new World("Render");
            var systemTypes = new List<System.Type>();
            
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systemTypes);
            
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
        
            ScriptBehaviourUpdateOrder.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());

            foreach (var w in World.All)
            {
                ScriptBehaviourUpdateOrder.UpdatePlayerLoop(w, ScriptBehaviourUpdateOrder.CurrentPlayerLoop);
            }
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }

    // public class Bootstrap :
    //     MonoBehaviour
    // {
    //     public bool turnOnDiagnostic;
    //
    //     public GameObject tileAuthoringPrefab01;
    //     public GameObject tileAuthoringPrefab02;
    //
    //     public Environment.Bridge.TileDetailAsset tileDetailAsset;
    //     
    //     private Entity _entity;
    //     
    //     private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
    //
    //     void Start()
    //     {
    //         var archetype = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(
    //             typeof(GameEnvironment.TheEnvironment));
    //         
    //         _entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(archetype);
    //         var archetype01 = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(
    //             typeof(Hud.App.LoadHudApp));
    //         var archetype02 = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(
    //             typeof(Hud.Stage.LoadHudStage));
    //
    //         World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(archetype01);
    //         World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(archetype02);
    //         // World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(_entity, tileDataAsset);
    //         
    //         // Observable
    //         //     .FromEvent<GenerationStatusDelegate, (DungeonGenerator, GenerationStatus)>(
    //         //         h => (generator, status) => h.Invoke((generator, status)),
    //         //         h => dungeonGenerator.OnGenerationStatusChanged += h,
    //         //         h => dungeonGenerator.OnGenerationStatusChanged -= h)
    //         //     .Subscribe(x =>
    //         //     {
    //         //     })
    //         //     .AddTo(_compositeDisposable);
    //
    //         var addressableInitializeAsync = Addressables.InitializeAsync();
    //         
    //         var addressableInitializeAsyncObservable =
    //             Observable
    //                 .FromEvent<AsyncOperationHandle<IResourceLocator>>(
    //                     h => addressableInitializeAsync.Completed += h,
    //                     h => addressableInitializeAsync.Completed -= h);
    //         addressableInitializeAsyncObservable                
    //             .Subscribe(x =>
    //             {
    //                 //
    //                 Debug.Log($"Bootstrap - addressableInitializeAsyncObservable done");
    //                 HandleAddressableInitializeAsyncCompleted();
    //             })
    //             .AddTo(_compositeDisposable);
    //         
    //         // Addressables.InitializeAsync().Completed += OnAddressableInitializeAsyncCompleted;
    //     }
    //
    //     // private void OnAddressableInitializeAsyncCompleted(AsyncOperationHandle<IResourceLocator> handle)
    //     private void HandleAddressableInitializeAsyncCompleted()
    //     {
    //         // Debug.Log($"Bootstrap - OnAddressableInitializeAsyncCompleted");
    //         Debug.Log($"Bootstrap - HandleAddressableInitializeAsyncCompleted");
    //
    //         Addressables.LoadAssetsAsync(new List<string> {"App - Hud"}, (UnityEngine.Object o) =>
    //         {
    //             Debug.Log(o);
    //         });
    //
    //         // StartCoroutine(SpawnPrefab(tileAuthoringPrefab01, 1.0f));
    //         // StartCoroutine(SpawnPrefab(tileAuthoringPrefab02, 4.0f));
    //         // StartCoroutine(SpawnPrefab(tileAuthoringPrefab01, 6.0f));
    //         // StartCoroutine(SpawnPrefab(tileAuthoringPrefab02, 9.0f));
    //         //
    //         // //
    //         // Utility.Bootstrap.AddInitializationSystem<Environment.RemoveConvertedSystem>();
    //         // Utility.Bootstrap.AddGameObjectBeforeConversionSystem<Environment.LoadSettingsSystem>();
    //         // Utility.Bootstrap.AddInitializationSystem<Environment.LoadTiledWorldMapSystem>();
    //         // Utility.Bootstrap.AddInitializationSystem<Environment.GenerateWorldMapSystem>();
    //         // if (turnOnDiagnostic)
    //         // {
    //         //     Utility.Bootstrap.AddInitializationSystem<Environment.GenerateDiagnosticWorldMapSystem>();
    //         // }
    //         // else
    //         // {
    //         //     Utility.Bootstrap.AddInitializationSystem<Environment.GenerateDiagnosticWorldMapSystem_Empty>();
    //         // }
    //         //
    //         // //
    //         // Utility.Bootstrap.AddSimulationSystem<Battle.UnitSpawnSystem>();
    //
    //         // var h = Addressables.LoadAssetsAsync<ScriptableObject>("ES", operation => {});
    //         // await h.Task;
    //         // var so = h.Result;
    //         // var count = so.Count;
    //         // Debug.Log($"count: {count}");
    //         // var desc = so.Aggregate("", (acc, next) => $"{acc}\n{next.name}");
    //         // Debug.Log(desc);
    //         
    //         // Environment Settings 2
    //
    //         // var h = Addressables.LoadAssetAsync<ScriptableObject>("Environment Settings 1");
    //         // await h.Task;
    //         // var so = h.Result;
    //         // Debug.Log($"{so.name}");
    //
    //         var group = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();
    //         var system01 = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<Appwide.AssetLoadingSystem>();
    //         // var system02 = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<Environment.SetupWorldMapSystem>();
    //         var system02 = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<Hud.App.LoadHudSystem>();
    //         var system03 = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<Hud.Stage.LoadHudSystem>();
    //         var system03a = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GameEnvironment.SetupWorldMapSystem>();
    //         var system03a1 = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GameEnvironment.GenerateWorldMapSystem>();
    //         var system03b = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<Hud.Stage.HandleMessageSystem>();
    //         var system03c = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GameEnvironment.RemoveConvertedSystem>();
    //
    //         GameEnvironment.GenerateVisualWorldMapSystem_Base system03a2 = null;
    //         if (turnOnDiagnostic)
    //         {
    //             system03a2 = World.DefaultGameObjectInjectionWorld
    //                 .GetOrCreateSystem<GameEnvironment.GenerateDiagnosticWorldMapSystem>();
    //         }
    //         else
    //         {
    //             system03a2 = World.DefaultGameObjectInjectionWorld
    //                 .GetOrCreateSystem<GameEnvironment.GenerateDiagnosticWorldMapSystem_Empty>();
    //         }
    //         
    //         system02.SetAssetLoadingService((Common.IAssetLoadingService)system01);
    //         system03.SetAssetLoadingService((Common.IAssetLoadingService)system01);
    //         system03a.SetAssetLoadingService((Common.IAssetLoadingService)system01);
    //         
    //         group.AddSystemToUpdateList(system01);            
    //         group.AddSystemToUpdateList(system02);            
    //         group.AddSystemToUpdateList(system03);            
    //         group.AddSystemToUpdateList(system03a);
    //         group.AddSystemToUpdateList(system03a1);
    //         group.AddSystemToUpdateList(system03a2);
    //         group.AddSystemToUpdateList(system03b);            
    //         group.AddSystemToUpdateList(system03c);            
    //     }
    //
    //
    //     IEnumerator SpawnPrefab(GameObject prefab, float afterTime)
    //     {
    //         yield return new WaitForSeconds(afterTime);
    //
    //         GameObject.Instantiate(prefab);
    //     }
    //
    //     IEnumerator Spawn01()
    //     {
    //         yield return new WaitForSeconds(3.0f);
    //
    //         GameObject.Instantiate(tileAuthoringPrefab01);
    //     }
    //
    //     IEnumerator Spawn02()
    //     {
    //         yield return new WaitForSeconds(30.0f);
    //
    //         GameObject.Instantiate(tileAuthoringPrefab02);
    //     }
    //
    //     private void OnDestroy()
    //     {
    //         _compositeDisposable?.Dispose();
    //     }
    // }
}
