namespace JoyBrick.Walkio.Game
{
    using UniRx;
    using UnityEngine.AddressableAssets;
    using UnityEngine.AddressableAssets.ResourceLocators;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public partial class Bootstrap
    {
        private void HandleAddressableInitializeAsyncCompleted()
        {
            _logger.Debug($"Bootstrap - HandleAddressableInitializeAsyncCompleted");

            _notifySetupBeforeEcs.OnNext(1);

            //
            SetupEcsWorldContext();
            SetupEcsWorldSystem();

            SetupFlowFieldPart();

            _notifySetupAfterEcs.OnNext(1);
            //
            // //
            // _assistants.ForEach(x =>
            // {
            // x.HandleAfterEcsSetup();
            // });
            //
            // _notifySetupEcsDone.OnNext(1);
            
            // _notifyAssetLoadingStarted.OnNext();
            StartLoadingAsset_App();
            
            SetupLevelPart();
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
                    _logger.Debug($"Bootstrap - SetupAddressable - addressableInitializeAsync is received");

                    HandleAddressableInitializeAsyncCompleted();
                })
                .AddTo(_compositeDisposable);
        }
    }
}
