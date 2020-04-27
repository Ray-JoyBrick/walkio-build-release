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

    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;
    
    using GameGameFlowControl = JoyBrick.Walkio.Game.GameFlowControl;

#if COMPLETE_PROJECT || HUD_FLOW_PROJECT

    using GameHudApp = JoyBrick.Walkio.Game.Hud.App;
    using GameHudPreparation = JoyBrick.Walkio.Game.Hud.Preparation;
    using GameHudStage = JoyBrick.Walkio.Game.Hud.Stage;
    
#endif
    
#if COMPLETE_PROJECT || LEVEL_FLOW_PROJECT
    
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;
    
#endif
    
    public partial class Bootstrap :
        MonoBehaviour
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(Bootstrap));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private IObservable<int> SetupEcsDone => _notifySetupEcsDone.AsObservable();
        private readonly Subject<int> _notifySetupEcsDone = new Subject<int>();

        //
        void Awake()
        {
            //
            SetupAppCenterCrashes();

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
        
        private void SetupUniRxLogger()
        {
            ObservableLogger.Listener
                .SubscribeOn(Scheduler.ThreadPool)
                .LogToUnityDebug()
                .AddTo(_compositeDisposable);
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
                    _logger.Debug($"Bootstrap - addressableInitializeAsync is received");

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

        private void SetupEcsSystem()
        {
            _logger.Debug($"Bootstrap - SetupEcsSystem");

            //
            var initializationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();
            var simulationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>();
            var presentationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PresentationSystemGroup>();

            // Project-wide
            var loadingDoneCheckSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameGameFlowControl.LoadingDoneCheckSystem>();
            var settingDoneCheckSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameGameFlowControl.SettingDoneCheckSystem>();
            
            var loadGameFlowSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameGameFlowControl.LoadGameFlowSystem>();
            

            // App-wide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            var loadAppHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudApp.LoadAppHudSystem>();
            var setupAppHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudApp.SetupAppHudSystem>();
#endif
            
            // Preparation-wide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            var loadPreparationHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudPreparation.LoadPreparationHudSystem>();
#endif

            // Stage-wide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            var loadStageHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudStage.LoadStageHudSystem>();
#endif
            
#if COMPLETE_PROJECT || LEVEL_FLOW_PROJECT
            var loadEnvironmentSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameEnvironment.LoadEnvironmentSystem>();
#endif

            //
            loadingDoneCheckSystem.FlowControl = (GameCommon.IFlowControl) this;
            settingDoneCheckSystem.FlowControl = (GameCommon.IFlowControl) this;
            loadGameFlowSystem.RefBootstrap = this.gameObject;
            loadGameFlowSystem.FlowControl = (GameCommon.IFlowControl) this;
            
            // App-wide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            loadAppHudSystem.RefBootstrap = this.gameObject;
            loadAppHudSystem.CommandService = (GameCommand.ICommandService) this;
            // loadAppHudSystem.InfoPresenter = (GameCommand.IInfoPresenter) this;
            loadAppHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            setupAppHudSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif
            
            // Preparation-wide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            loadPreparationHudSystem.RefBootstrap = this.gameObject;
            loadPreparationHudSystem.CommandService = (GameCommand.ICommandService) this;
            // loadPreparationHudSystem.InfoPresenter = (GameCommand.IInfoPresenter) this;
            loadPreparationHudSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif

            // Stage-wide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            loadStageHudSystem.RefBootstrap = this.gameObject;
            loadStageHudSystem.CommandService = (GameCommand.ICommandService) this;
            // loadStageHudSystem.InfoPresenter = (GameCommand.IInfoPresenter) this;
            loadStageHudSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif

#if COMPLETE_PROJECT || LEVEL_FLOW_PROJECT
            loadEnvironmentSystem.CommandService = (GameCommand.ICommandService) this;
            loadEnvironmentSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif
            
            //
            loadingDoneCheckSystem.Construct();
            settingDoneCheckSystem.Construct();
            loadGameFlowSystem.Construct();

            // App-wide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            loadAppHudSystem.Construct();
            setupAppHudSystem.Construct();
#endif
            
            // Preparation-wide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            loadPreparationHudSystem.Construct();
#endif
            
            // Stage-wide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            loadStageHudSystem.Construct();
#endif
            
#if COMPLETE_PROJECT || LEVEL_FLOW_PROJECT
            loadEnvironmentSystem.Construct();
#endif            
            
            // InitializationSystemGroup
            initializationSystemGroup.AddSystemToUpdateList(loadingDoneCheckSystem);
            initializationSystemGroup.AddSystemToUpdateList(settingDoneCheckSystem);
            initializationSystemGroup.AddSystemToUpdateList(loadGameFlowSystem);

            // App-wide - InitializationSystemGroup
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadAppHudSystem);
            initializationSystemGroup.AddSystemToUpdateList(setupAppHudSystem);
#endif
            
            // Preparation-wide - InitializationSystemGroup
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadPreparationHudSystem);
#endif
            
            // Stage-wide - InitializationSystemGroup
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadStageHudSystem);
#endif
            
#if COMPLETE_PROJECT || LEVEL_FLOW_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadEnvironmentSystem);
#endif
        }

        private void SetupFoundationFlow()
        {
            _logger.Debug($"Bootstrap - SetupFoundationFlow");
            _notifyLoadingAsset.OnNext(new GameCommon.FlowControlContext
            {
                Name = "App"
            });
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
