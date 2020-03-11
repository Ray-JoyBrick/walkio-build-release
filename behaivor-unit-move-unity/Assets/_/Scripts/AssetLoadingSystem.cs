namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine.AddressableAssets;
    using UnityEngine.AddressableAssets.ResourceLocators;
    using UnityEngine.ResourceManagement.AsyncOperations;

    // Should make this static instead of creating instance as there should actually be one singleton system in
    // the project. This is used to be simple in the help of Extenject. Will see if the use of Extenject still makes
    // sense at year 2020.
    public class AssetLoadingSystem :
        System.IDisposable
    {
        // Use a single event for simplicity at this moment
        public event System.Action InitializationFinished;

        public AssetLoadingSystem()
        {
        }

        public void Initialize()
        {
            Addressables.InitializeAsync().Completed += OnCompleted;
        }

        private void OnCompleted(AsyncOperationHandle<IResourceLocator> handle)
        {
            // At this point, addressable itself should be initialized completely
            InitializationFinished?.Invoke();
        }

        public async Task<T> LoadFromAddressable<T>(string addressName) where T : UnityEngine.Object
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<T>(addressName);

            var singleAsset = await asyncOperationHandle.Task;

            return singleAsset;
        }
        
        // public AsyncOperationHandle<List<AsyncOperationHandle<T>>> LoadAssetsByLabelAsync<T>(string label) where T : UnityEngine.Object
        // {
        //     var handle = Addressables.ResourceManager.StartOperation(
        //         new LoadAssetsByLabelOperation(_loadedAssets, _loadingAssets, label, AssetLoadedCallback), default);
        //     return handle;
        // }

        //
        public void Dispose()
        {
        }
    }
}
