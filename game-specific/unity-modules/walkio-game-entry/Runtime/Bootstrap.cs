﻿namespace JoyBrick.Walkio.Game
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
    
    #if COMPLETE_PROJECT || HUD_FLOW_PROJECT
    
    using GameGameFlowControl = JoyBrick.Walkio.Game.GameFlowControl;
    using GameHudApp = JoyBrick.Walkio.Game.Hud.App;
    using GameHudPreparation = JoyBrick.Walkio.Game.Hud.Preparation;
    using GameHudStage = JoyBrick.Walkio.Game.Hud.Stage;
    
    #endif
    
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
                    // StartInitializingAppwideService();
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
            ReformCommandStream();

            //
            SetupAddressable();
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT            
            AssignFsmVariableValue();
#endif
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
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            var loadingDoneCheckSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameGameFlowControl.LoadingDoneCheckSystem>();
            var settingDoneCheckSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameGameFlowControl.SettingDoneCheckSystem>();
#endif

            // Appwide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
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
#endif
            
            // Preparationwide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
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
#endif

            // Stagewide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
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
#endif

            //
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            loadingDoneCheckSystem.FlowControl = (GameCommon.IFlowControl) this;
            settingDoneCheckSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif
            
            // Appwide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            // initializeAppwideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            loadAppHudSystem.CommandService = (GameCommand.ICommandService) this;
            loadAppHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            setupAppHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            // setupAppwideServiceSystem.CommandService = (GameCommand.ICommandService) this;
#endif
            
            // Preparationwide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            // initializePreparationwideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            loadPreparationHudSystem.RefBootstrap = this.gameObject;
            loadPreparationHudSystem.CommandService = (GameCommand.ICommandService) this;
            loadPreparationHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            // setupPreparationwideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            // cleanupPreparationwideServiceSystem.CommandService = (GameCommand.ICommandService) this;
#endif

            // Stagewide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            // initializeStagewideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            loadStageHudSystem.RefBootstrap = this.gameObject;
            loadStageHudSystem.CommandService = (GameCommand.ICommandService) this;
            loadStageHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            // loadStageEnvironmentSystem.CommandService = (GameCommand.ICommandService) this;
            // setupStagewideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            // cleanupStagewideServiceSystem.CommandService = (GameCommand.ICommandService) this;
#endif
            
            //
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            loadingDoneCheckSystem.Construct();
            settingDoneCheckSystem.Construct();
#endif

            // Appwide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            // initializeAppwideServiceSystem.Construct();
            loadAppHudSystem.Construct();
            setupAppHudSystem.Construct();
            // setupAppwideServiceSystem.Construct();
#endif
            
            // Preparationwide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            // initializePreparationwideServiceSystem.Construct();
            loadPreparationHudSystem.Construct();
            // setupPreparationwideServiceSystem.Construct();
            // cleanupPreparationwideServiceSystem.Construct();
#endif
            
            // Stagewide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            // initializeStagewideServiceSystem.Construct();
            loadStageHudSystem.Construct();
            // loadStageEnvironmentSystem.Construct();
            // setupStagewideServiceSystem.Construct();
            // cleanupStagewideServiceSystem.Construct();
#endif
            
            // InitializationSystemGroup
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadingDoneCheckSystem);
            initializationSystemGroup.AddSystemToUpdateList(settingDoneCheckSystem);
#endif

            // Appwide - InitializationSystemGroup
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            // initializationSystemGroup.AddSystemToUpdateList(initializeAppwideServiceSystem);
            initializationSystemGroup.AddSystemToUpdateList(loadAppHudSystem);
            initializationSystemGroup.AddSystemToUpdateList(setupAppHudSystem);
            // initializationSystemGroup.AddSystemToUpdateList(setupAppwideServiceSystem);
#endif
            
            // Preparationwide - InitializationSystemGroup
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            // initializationSystemGroup.AddSystemToUpdateList(initializePreparationwideServiceSystem);
            initializationSystemGroup.AddSystemToUpdateList(loadPreparationHudSystem);
            // initializationSystemGroup.AddSystemToUpdateList(setupPreparationwideServiceSystem);
            // initializationSystemGroup.AddSystemToUpdateList(cleanupPreparationwideServiceSystem);
#endif
            
            // Stagewide - InitializationSystemGroup
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            // initializationSystemGroup.AddSystemToUpdateList(initializeStagewideServiceSystem);
            initializationSystemGroup.AddSystemToUpdateList(loadStageHudSystem);
            // initializationSystemGroup.AddSystemToUpdateList(loadStageEnvironmentSystem);
            // initializationSystemGroup.AddSystemToUpdateList(setupStagewideServiceSystem);
            // initializationSystemGroup.AddSystemToUpdateList(cleanupStagewideServiceSystem);
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

#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
        private void AssignFsmVariableValue()
        {
            var pmfsms = FindObjectsOfType<PlayMakerFSM>();
            
            pmfsms.ToList().ForEach(x => SetReferenceToExtension(x.gameObject));
        }

        
        // TODO: Move hard reference to PlayMakerFSM to somewhere else
        // TODO: Assign reference to FSM may need a better approach
        private void SetReferenceToExtension(GameObject inGO)
        {
            var pmfsms = new List<PlayMakerFSM>();

            // Canvas itself
            var comps = inGO.GetComponents<PlayMakerFSM>();
            if (comps.Length > 0)
            {
                pmfsms.AddRange(comps);
            }
            
            // Views under Canvas
            foreach (Transform child in inGO.transform)
            {
                comps = child.GetComponents<PlayMakerFSM>();
                if (comps.Length > 0)
                {
                    pmfsms.AddRange(comps);
                }
            }

            pmfsms.ForEach(x => SetFsmVariableValue(x, "zz_Command Service", this.gameObject));
            pmfsms.Clear();
        }

        // TODO: Make this in some static class so that other class can access as well
        private static void SetFsmVariableValue(PlayMakerFSM pmfsm, string variableName, GameObject inValue)
        {
            var commandServiceVariables =
                pmfsm.FsmVariables.GameObjectVariables.Where(x => string.CompareOrdinal(x.Name, variableName) == 0);
                
            commandServiceVariables.ToList()
                .ForEach(x =>
                {
                    x.Value = inValue;
                });
        }
#endif
        
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
