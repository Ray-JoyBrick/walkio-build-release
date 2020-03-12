namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.AddressableAssets.ResourceLocators;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public interface IAssetLoadingService
    {
        void LoadAssetAsync<T>(string addressableName, System.Action<T> handler);
        void Release<T>(T o) where T : UnityEngine.Object;
    }

    // Should make this static instead of creating instance as there should actually be one singleton system in
    // the project. This is used to be simple in the help of Extenject. Will see if the use of Extenject still makes
    // sense at year 2020.
    // Can just use ECS system for this?
    
    [DisableAutoCreation]
    public class AssetLoadingSystem :
        SystemBase,
        IAssetLoadingService
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            
            Addressables.InitializeAsync().Completed += OnCompleted;
        }

        private void OnCompleted(AsyncOperationHandle<IResourceLocator> handle)
        {
            
        }

        public void LoadAssetAsync<T>(string addressableName, System.Action<T> handler)
        {
            var handle = Addressables.LoadAssetAsync<T>(addressableName);

            handle.Completed += operationHandle => { handler(operationHandle.Result); };
        }

        public void Release<T>(T o) where T : UnityEngine.Object
        {
            Addressables.Release(o);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Addressables.InitializeAsync().Completed -= OnCompleted;
        }

        protected override void OnUpdate()
        {
        }
    }
    //     System.IDisposable,
    //     IAssetLoadingService
    // {
    //     // Use a single event for simplicity at this moment
    //     public event System.Action InitializationFinished;
    //
    //     public AssetLoadingSystem()
    //     {
    //     }
    //
    //     public void Initialize()
    //     {
    //         Addressables.InitializeAsync().Completed += OnCompleted;
    //     }
    //
    //     private void OnCompleted(AsyncOperationHandle<IResourceLocator> handle)
    //     {
    //         // At this point, addressable itself should be initialized completely
    //         InitializationFinished?.Invoke();
    //     }
    //
    //     public async Task<T> LoadFromAddressable<T>(string addressName) where T : UnityEngine.Object
    //     {
    //         var asyncOperationHandle = Addressables.LoadAssetAsync<T>(addressName);
    //
    //         var singleAsset = await asyncOperationHandle.Task;
    //
    //         return singleAsset;
    //     }
    //     
    //     // public AsyncOperationHandle<List<AsyncOperationHandle<T>>> LoadAssetsByLabelAsync<T>(string label) where T : UnityEngine.Object
    //     // {
    //     //     var handle = Addressables.ResourceManager.StartOperation(
    //     //         new LoadAssetsByLabelOperation(_loadedAssets, _loadingAssets, label, AssetLoadedCallback), default);
    //     //     return handle;
    //     // }
    //
    //     //
    //     public void Dispose()
    //     {
    //     }
    // }
}
