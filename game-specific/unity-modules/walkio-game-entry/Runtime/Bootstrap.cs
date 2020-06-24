namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using UniRx.Diagnostics;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.AddressableAssets.ResourceLocators;
    using UnityEngine.ResourceManagement.AsyncOperations;

    //
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT

    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;
    
    using GameGameFlowControl = JoyBrick.Walkio.Game.GameFlowControl;

#endif

#if COMPLETE_PROJECT || BEHAVIOR_PROJECT

    using GameHudApp = JoyBrick.Walkio.Game.Hud.App;
    using GameHudPreparation = JoyBrick.Walkio.Game.Hud.Preparation;
    using GameHudStage = JoyBrick.Walkio.Game.Hud.Stage;
    
#endif
    
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT || LEVEL_FLOW_PROJECT
    
    using GameBattle = JoyBrick.Walkio.Game.Battle;
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;
    using GameStageFlowControl = JoyBrick.Walkio.Game.StageFlowControl;
    
#endif
    
    public partial class Bootstrap :
        MonoBehaviour
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(Bootstrap));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private IObservable<int> SetupEcsDone => _notifySetupEcsDone.AsObservable();
        private readonly Subject<int> _notifySetupEcsDone = new Subject<int>();

        // //
        // private EntityManager _entityManager;
        
        //
        void Awake()
        {
            //
            SetupAppCenterCrashes();
            
            //
            SetupRemoteConfiguration();
            
            //
            SetupAuth();

            //
            SetupUniRxLogger();
        }

        void Start()
        {
            //
            SetupAppCenterAnalytics();

            //
            SetupEcsDone
                .Subscribe(x =>
                {
                    // If assist is presented, has to wait till assist part is done.
                    SetupFoundationFlow();
                })
                .AddTo(_compositeDisposable);

            CommandStream
                .Subscribe(x =>
                {
                    //
                    _logger.Debug($"Bootstrap - Start - Receive Command: {x}");
                })
                .AddTo(_compositeDisposable);

            //
            ReformCommandStream();

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
                    _logger.Debug($"Bootstrap - SetupAddressable - addressableInitializeAsync is received");

                    HandleAddressableInitializeAsyncCompleted();
                    _notifySetupEcsDone.OnNext(1);
                })
                .AddTo(_compositeDisposable);
        }

        private void HandleAddressableInitializeAsyncCompleted()
        {
            _logger.Debug($"Bootstrap - HandleAddressableInitializeAsyncCompleted");

            _notifyCanStartInitialSetup.OnNext(1);
            SetupEcsSystem();
        }

        private void SetupFoundationFlow()
        {
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            
            _logger.Debug($"Bootstrap - SetupFoundationFlow");
            _notifyLoadingAsset.OnNext(new GameCommon.FlowControlContext
            {
                Name = "App"
            });

#endif
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
