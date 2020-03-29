namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.AddressableAssets.ResourceLocators;
    using UnityEngine.ResourceManagement.AsyncOperations;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;

    public class Bootstrap : MonoBehaviour
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        void Start()
        {
            var addressableInitializeAsync = Addressables.InitializeAsync();
            
            var addressableInitializeAsyncObservable =
                Observable
                    .FromEvent<AsyncOperationHandle<IResourceLocator>>(
                        h => addressableInitializeAsync.Completed += h,
                        h => addressableInitializeAsync.Completed -= h);
            addressableInitializeAsyncObservable                
                .Subscribe(x =>
                {
                    //
                    Debug.Log($"Bootstrap - addressableInitializeAsyncObservable done");
                    HandleAddressableInitializeAsyncCompleted();
                })
                .AddTo(_compositeDisposable);
        }

        private void HandleAddressableInitializeAsyncCompleted()
        {
            // Debug.Log($"Bootstrap - OnAddressableInitializeAsyncCompleted");
            Debug.Log($"Bootstrap - HandleAddressableInitializeAsyncCompleted");

            // Addressables.LoadAssetsAsync(new List<string> {"App - Hud"}, (UnityEngine.Object o) => { Debug.Log(o); });
            var group = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();
            // var system01 = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<Appwide.AssetLoadingSystem>();
            
            var loadWorldSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<GameEnvironment.LoadWorldSystem>();
            var loadMapGridSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<GameEnvironment.LoadMapGridSystem>();

            loadMapGridSystem.WorldLoading = (GameCommon.IWorldLoading) loadWorldSystem;
            
            group.AddSystemToUpdateList(loadWorldSystem); 
            group.AddSystemToUpdateList(loadMapGridSystem); 
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
