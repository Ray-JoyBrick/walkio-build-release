namespace JoyBrick.Walkio.Game
{
    using UnityEngine.AddressableAssets;
    using UnityEngine.AddressableAssets.ResourceLocators;
    using UnityEngine.ResourceManagement.AsyncOperations;

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

        //
        public void Dispose()
        {
        }
    }
}
