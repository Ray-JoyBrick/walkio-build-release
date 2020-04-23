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
            SetupEcsDone
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
                    _logger.Debug($"Bootstrap - Start - Receive Command: {x}");
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
                    _notifySetupEcsDone.OnNext(1);
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
            var presentationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PresentationSystemGroup>();

            // Project-wide
            var loadingDoneCheckSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameGameFlowControl.LoadingDoneCheckSystem>();
            var settingDoneCheckSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameGameFlowControl.SettingDoneCheckSystem>();

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
            
            // App-wide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            loadAppHudSystem.CommandService = (GameCommand.ICommandService) this;
            loadAppHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            setupAppHudSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif
            
            // Preparation-wide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            loadPreparationHudSystem.RefBootstrap = this.gameObject;
            loadPreparationHudSystem.CommandService = (GameCommand.ICommandService) this;
            loadPreparationHudSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif

            // Stage-wide
#if COMPLETE_PROJECT || HUD_FLOW_PROJECT
            loadStageHudSystem.RefBootstrap = this.gameObject;
            loadStageHudSystem.CommandService = (GameCommand.ICommandService) this;
            loadStageHudSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif

#if COMPLETE_PROJECT || LEVEL_FLOW_PROJECT
            loadEnvironmentSystem.CommandService = (GameCommand.ICommandService) this;
            loadEnvironmentSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif
            
            //
            loadingDoneCheckSystem.Construct();
            settingDoneCheckSystem.Construct();

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
