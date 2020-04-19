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
    using GameHudApp = JoyBrick.Walkio.Game.Hud.App;
    using GameHudPreparation = JoyBrick.Walkio.Game.Hud.Preparation;
    using GameHudStage = JoyBrick.Walkio.Game.Hud.Stage;
    
    public partial class Bootstrap :
        MonoBehaviour
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(Bootstrap));

        void Awake()
        {
            // This will be used for AppCenter, as AppCenter does not have pre-processor define,
            // Use custom COMPLETE_PROJECT to tell if AppCenter functionality should be turned on.
            #if COMPLETE_PROJECT
            
            #endif
        }
        
        void Start()
        {
            SetupUniRxLogger();
            
            //
            SetupECSDone
                .Subscribe(x =>
                {
                    // _notifyLoadAppHud.OnNext(1);
                    StartInitializingAppwideService();
                })
                .AddTo(_compositeDisposable);

            CommandStream
                .Subscribe(x =>
                {
                    //
                    _logger.Debug($"command: {x}");
                })
                .AddTo(_compositeDisposable);
            
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
                    _notifySetupECSDone.OnNext(1);
                })
                .AddTo(_compositeDisposable);            
        }

        private void HandleAddressableInitializeAsyncCompleted()
        {
            _logger.Debug($"Bootstrap - HandleAddressableInitializeAsyncCompleted");

            SetupEcsSystem();
            SetupFoundationFlow();
        }

        private void SetupEcsSystem()
        {
            _logger.Debug($"Bootstrap - SetupEcsSystem");

            //
            var initializationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();
            var simulationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>();

            //
            var loadingDoneCheckSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameGameFlowControl.LoadingDoneCheckSystem>();
            var settingDoneCheckSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameGameFlowControl.SettingDoneCheckSystem>();

            // Appwide
            // var initializeAppwideServiceSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameSceneApp.InitializeAppwideServiceSystem>();
            var loadAppHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudApp.LoadAppHudSystem>();
            var setupAppHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudApp.SetupAppHudSystem>();
            // var setupAppwideServiceSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameSceneApp.SetupAppwideServiceSystem>();
            
            // Preparationwide
            // var initializePreparationwideServiceSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameScenePreparation.InitializePreparationwideServiceSystem>();
            var loadPreparationHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudPreparation.LoadPreparationHudSystem>();
            // var setupPreparationwideServiceSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameScenePreparation.SetupPreparationwideServiceSystem>();
            // var cleanupPreparationwideServiceSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameScenePreparation.CleanupPreparationwideServiceSystem>();

            // Stagewide
            // var initializeStagewideServiceSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameSceneStage.InitializeStagewideServiceSystem>();
            var loadStageHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudStage.LoadStageHudSystem>();
            // var loadStageEnvironmentSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameSceneStage.LoadStageEnvironmentSystem>();
            // var setupStagewideServiceSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameSceneStage.SetupStagewideServiceSystem>();
            // var cleanupStagewideServiceSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameSceneStage.CleanupStagewideServiceSystem>();

            //
            loadingDoneCheckSystem.FlowControl = (GameCommon.IFlowControl) this;
            settingDoneCheckSystem.FlowControl = (GameCommon.IFlowControl) this;
            
            // Appwide
            // initializeAppwideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            loadAppHudSystem.CommandService = (GameCommand.ICommandService) this;
            loadAppHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            setupAppHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            // setupAppwideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            
            // Preparationwide
            // initializePreparationwideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            loadPreparationHudSystem.CommandService = (GameCommand.ICommandService) this;
            loadPreparationHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            // setupPreparationwideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            // cleanupPreparationwideServiceSystem.CommandService = (GameCommand.ICommandService) this;

            // Stagewide
            // initializeStagewideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            loadStageHudSystem.CommandService = (GameCommand.ICommandService) this;
            loadStageHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            // loadStageEnvironmentSystem.CommandService = (GameCommand.ICommandService) this;
            // setupStagewideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            // cleanupStagewideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            
            //
            loadingDoneCheckSystem.Construct();
            settingDoneCheckSystem.Construct();

            // Appwide
            // initializeAppwideServiceSystem.Construct();
            loadAppHudSystem.Construct();
            setupAppHudSystem.Construct();
            // setupAppwideServiceSystem.Construct();
            
            // Preparationwide
            // initializePreparationwideServiceSystem.Construct();
            loadPreparationHudSystem.Construct();
            // setupPreparationwideServiceSystem.Construct();
            // cleanupPreparationwideServiceSystem.Construct();
            
            // Stagewide
            // initializeStagewideServiceSystem.Construct();
            loadStageHudSystem.Construct();
            // loadStageEnvironmentSystem.Construct();
            // setupStagewideServiceSystem.Construct();
            // cleanupStagewideServiceSystem.Construct();
            
            // InitializationSystemGroup
            initializationSystemGroup.AddSystemToUpdateList(loadingDoneCheckSystem);
            initializationSystemGroup.AddSystemToUpdateList(settingDoneCheckSystem);

            // Appwide - InitializationSystemGroup
            // initializationSystemGroup.AddSystemToUpdateList(initializeAppwideServiceSystem);
            initializationSystemGroup.AddSystemToUpdateList(loadAppHudSystem);
            initializationSystemGroup.AddSystemToUpdateList(setupAppHudSystem);
            // initializationSystemGroup.AddSystemToUpdateList(setupAppwideServiceSystem);
            
            // Preparationwide - InitializationSystemGroup
            // initializationSystemGroup.AddSystemToUpdateList(initializePreparationwideServiceSystem);
            initializationSystemGroup.AddSystemToUpdateList(loadPreparationHudSystem);
            // initializationSystemGroup.AddSystemToUpdateList(setupPreparationwideServiceSystem);
            // initializationSystemGroup.AddSystemToUpdateList(cleanupPreparationwideServiceSystem);
            
            // Stagewide - InitializationSystemGroup
            // initializationSystemGroup.AddSystemToUpdateList(initializeStagewideServiceSystem);
            initializationSystemGroup.AddSystemToUpdateList(loadStageHudSystem);
            // initializationSystemGroup.AddSystemToUpdateList(loadStageEnvironmentSystem);
            // initializationSystemGroup.AddSystemToUpdateList(setupStagewideServiceSystem);
            // initializationSystemGroup.AddSystemToUpdateList(cleanupStagewideServiceSystem);            
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
        
        // private void CreateWorld()
        // {
        //     var world = new World("Render");
        //     var systemTypes = new List<System.Type>();
        //     
        //     DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systemTypes);
        //     
        //     ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
        //
        //     ScriptBehaviourUpdateOrder.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());
        //
        //     foreach (var w in World.All)
        //     {
        //         ScriptBehaviourUpdateOrder.UpdatePlayerLoop(w, ScriptBehaviourUpdateOrder.CurrentPlayerLoop);
        //     }
        // }
    }
}
