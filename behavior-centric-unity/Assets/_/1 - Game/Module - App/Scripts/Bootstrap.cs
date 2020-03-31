namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using HellTap.PoolKit;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.AddressableAssets.ResourceLocators;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;
    using GameTemplate = JoyBrick.Walkio.Game.Template;

    public partial class Bootstrap :
        MonoBehaviour
    {
        //
        private EntityManager _entityManager;

        // private EntityArchetype _loadZoneRequestEventArchetype;

        private Pool _teamForcePool;
        private Pool _freeUnitPool;
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        void Start()
        {
            //
            SetupECSWorld();
            
            //
            SetupAddressable();
            
            //
            SetupTeamForcePool();
            SetupFreeUnitPool();

            // Can actually group by using visual scripting tool
            // For now, just simulate the flow by sending timed event
            Observable.Timer(System.TimeSpan.FromMilliseconds(500))
                .Subscribe(_ =>
                {
                    //
                    _notifyInitializingEnvironment.OnNext(0);
                })
                .AddTo(_compositeDisposable);
            
            Observable.Timer(System.TimeSpan.FromMilliseconds(3000))
                .Subscribe(_ =>
                {
                    //
                    // _entityManager.CreateEntity(_loadZoneRequestEventArchetype);

                    
                    //
                    if (_zoneSceneLoaded)
                    {
                        // Should just removing additive scene before calling loading world
                        var asyncOp = SceneManager.UnloadSceneAsync(_zoneScene);

                        asyncOp.AsObservable()
                            .Subscribe(x =>
                            {
                                _notifyLoadingWorld.OnNext(0);
                            })
                            .AddTo(_compositeDisposable);
                    }
                    else
                    {
                        _notifyLoadingWorld.OnNext(0);
                    }
                })
                .AddTo(_compositeDisposable);

            Observable.Timer(System.TimeSpan.FromMilliseconds(5500))
                .Subscribe(_ =>
                {
                    //
                    var teamForce = _teamForcePool.Spawn("Team Force");
                })
                .AddTo(_compositeDisposable);
        }

        private void SetupECSWorld()
        {
            //
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            // _loadZoneRequestEventArchetype = _entityManager.CreateArchetype(
            //     typeof(GameCommon.LoadZoneRequest));

            var environmentArchetype =
                _entityManager.CreateArchetype(typeof(GameEnvironment.TheEnvironment));

            var zoneArchetype =
                _entityManager.CreateArchetype(
                    typeof(GameEnvironment.Zone));

            _entityManager.CreateEntity(environmentArchetype);
            _entityManager.CreateEntity(zoneArchetype);
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

        private void SetupTeamForcePool()
        {
            _teamForcePool = PoolKit.Find("Team Force Pool");
            
            var onPoolSpawnObservable =
                Observable
                    .FromEvent<Pool.OnPoolSpawnDelegate, (Transform, Pool)>(
                        h => (t, p) => h.Invoke((t, p)),
                        h => _teamForcePool.onPoolSpawn += h,
                        h => _teamForcePool.onPoolSpawn -= h);

            var onPoolDespawnObservable =
                Observable
                    .FromEvent<Pool.OnPoolDespawnDelegate, (Transform, Pool)>(
                        h => (t, p) => h.Invoke((t, p)),
                        h => _teamForcePool.onPoolDespawn += h,
                        h => _teamForcePool.onPoolDespawn -= h);

            var combined = onPoolSpawnObservable.Merge(onPoolDespawnObservable);
            combined
                .Subscribe(x =>
                {
                    //
                    Debug.Log($"Team Force Pool: spawn or despawn");
                })
                .AddTo(_compositeDisposable);
        }
        
        private void SetupFreeUnitPool()
        {
            _freeUnitPool = PoolKit.Find("Free Unit Pool");
            
            var onPoolSpawnObservable =
                Observable
                    .FromEvent<Pool.OnPoolSpawnDelegate, (Transform, Pool)>(
                        h => (t, p) => h.Invoke((t, p)),
                        h => _freeUnitPool.onPoolSpawn += h,
                        h => _freeUnitPool.onPoolSpawn -= h);

            var onPoolDespawnObservable =
                Observable
                    .FromEvent<Pool.OnPoolDespawnDelegate, (Transform, Pool)>(
                        h => (t, p) => h.Invoke((t, p)),
                        h => _freeUnitPool.onPoolDespawn += h,
                        h => _freeUnitPool.onPoolDespawn -= h);

            var combined = onPoolSpawnObservable.Merge(onPoolDespawnObservable);
            combined
                .Subscribe(x =>
                {
                    //
                    Debug.Log($"Free Unit Pool: spawn or despawn");
                })
                .AddTo(_compositeDisposable);
        }

        private void HandleAddressableInitializeAsyncCompleted()
        {
            Debug.Log($"Bootstrap - HandleAddressableInitializeAsyncCompleted");

            //
            var group = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();

            //
            var loadEnvironmentTemplateSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<GameEnvironment.LoadEnvironmentTemplateSystem>();
            var loadZoneTemplateSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<GameEnvironment.LoadZoneTemplateSystem>();
            var generateZoneSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<GameEnvironment.GenerateZoneSystem>();

            var generatePathfindSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<GameEnvironment.GeneratePathfindSystem>();

            var removeConvertedSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameEnvironment.RemoveConvertedSystem>();

            // Explicitly assign the dependencies manually
            loadEnvironmentTemplateSystem.EnvironmentSetupRequester = (GameCommon.IEnvironmentSetupRequester) this;
            loadZoneTemplateSystem.WorldLoadingRequester = (GameCommon.IWorldLoadingRequester) this;
            
            //
            group.AddSystemToUpdateList(loadEnvironmentTemplateSystem);
            group.AddSystemToUpdateList(loadZoneTemplateSystem);
            group.AddSystemToUpdateList(generateZoneSystem);
            group.AddSystemToUpdateList(generatePathfindSystem);
            
            group.AddSystemToUpdateList(removeConvertedSystem);
            
            //
            loadEnvironmentTemplateSystem.Setup();
            loadZoneTemplateSystem.Setup();
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
