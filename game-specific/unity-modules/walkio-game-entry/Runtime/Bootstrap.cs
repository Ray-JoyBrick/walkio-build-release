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
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;
    
    using GameGameFlowControl = JoyBrick.Walkio.Game.GameFlowControl;

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
            
            // //
            // _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
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
            var cleanupSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameGameFlowControl.CleanupSystem>();
            var cleanupStageUseEntitySystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameGameFlowControl.CleanupStageUseEntitySystem>();
            
            var loadGameFlowSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameGameFlowControl.LoadGameFlowSystem>();

            // App-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            var loadAppHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudApp.LoadAppHudSystem>();
            var setupAppHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudApp.SetupAppHudSystem>();
#endif
            
            // Preparation-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            var loadPreparationHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudPreparation.LoadPreparationHudSystem>();
            var setupPreparationHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudPreparation.SetupPreparationHudSystem>();
#endif

            // Stage-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            var loadStageHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudStage.LoadStageHudSystem>();
            var setupStageHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudStage.SetupStageHudSystem>();
#endif
            
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            var loadStageFlowSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameStageFlowControl.LoadStageFlowSystem>();
            var loadBattleSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameBattle.LoadBattleSystem>();
            var loadEnvironmentSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameEnvironment.LoadEnvironmentSystem>();
            
            var setupStageFlowSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameStageFlowControl.SetupStageFlowSystem>();
            var setupBattleSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameBattle.SetupBattleSystem>();
            var setupEnvironmentSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameEnvironment.SetupEnvironmentSystem>();

            var manageUnitSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameBattle.ManageUnitSystem>();

            //
            var createTeamForceUnitSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameBattle.CreateTeamForceUnitSystem>();

            var teamUnitToPathSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameBattle.TeamUnitToPathSystem>();

            //
            var adjustMoveToTargetFlowFieldSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameBattle.AdjustMoveToTargetFlowFieldSystem>();
            var cleanUpFlowFieldSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameBattle.CleanUpFlowFieldSystem>();

            //
            var moveOnPathSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameBattle.MoveOnPathSystem>();

            var assignFlowFieldTileToTeamUnitSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameBattle.AssignFlowFieldTileToTeamUnitSystem>();

            var moveOnFlowFieldTileSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameBattle.MoveOnFlowFieldTileSystem>();

            var neutralForceUnitHitCheckSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameBattle.NeutralForceUnitHitCheckSystem>();

            // var pickupHitCheckSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameBattle.PickupHitCheckSystem>();
            
#endif

            //
            loadingDoneCheckSystem.FlowControl = (GameCommon.IFlowControl) this;
            settingDoneCheckSystem.FlowControl = (GameCommon.IFlowControl) this;
            cleanupSystem.FlowControl = (GameCommon.IFlowControl) this;
            loadGameFlowSystem.RefBootstrap = this.gameObject;
            loadGameFlowSystem.FlowControl = (GameCommon.IFlowControl) this;
            
            // App-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadAppHudSystem.RefBootstrap = this.gameObject;
            loadAppHudSystem.CommandService = (GameCommand.ICommandService) this;
            // loadAppHudSystem.InfoPresenter = (GameCommand.IInfoPresenter) this;
            loadAppHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            setupAppHudSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif
            
            // Preparation-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadPreparationHudSystem.RefBootstrap = this.gameObject;
            loadPreparationHudSystem.CommandService = (GameCommand.ICommandService) this;
            // loadPreparationHudSystem.InfoPresenter = (GameCommand.IInfoPresenter) this;
            loadPreparationHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            setupPreparationHudSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif

            // Stage-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadStageHudSystem.RefBootstrap = this.gameObject;
            loadStageHudSystem.CommandService = (GameCommand.ICommandService) this;
            // loadStageHudSystem.InfoPresenter = (GameCommand.IInfoPresenter) this;
            loadStageHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            setupStageHudSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif

#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadStageFlowSystem.RefBootstrap = this.gameObject;
            loadStageFlowSystem.CommandService = (GameCommand.ICommandService) this;
            loadStageFlowSystem.FlowControl = (GameCommon.IFlowControl) this;

            loadBattleSystem.RefBootstrap = this.gameObject;
            loadBattleSystem.CommandService = (GameCommand.ICommandService) this;
            loadBattleSystem.FlowControl = (GameCommon.IFlowControl) this;

            loadEnvironmentSystem.CommandService = (GameCommand.ICommandService) this;
            loadEnvironmentSystem.FlowControl = (GameCommon.IFlowControl) this;

            setupStageFlowSystem.FlowControl = (GameCommon.IFlowControl) this;
            setupBattleSystem.FlowControl = (GameCommon.IFlowControl) this;
            setupEnvironmentSystem.FlowControl = (GameCommon.IFlowControl) this;

            manageUnitSystem.FlowControl = (GameCommon.IFlowControl) this;

            //
            createTeamForceUnitSystem.CommandService = (GameCommand.ICommandService) this;
            createTeamForceUnitSystem.FlowControl = (GameCommon.IFlowControl) this;

            teamUnitToPathSystem.FlowControl = (GameCommon.IFlowControl) this;
            teamUnitToPathSystem.AStarPathService = (GameCommon.IAStarPathService) this;
            
            //
            adjustMoveToTargetFlowFieldSystem.FlowControl = (GameCommon.IFlowControl) this;
            
            cleanUpFlowFieldSystem.FlowControl = (GameCommon.IFlowControl) this;

            //
            moveOnPathSystem.FlowControl = (GameCommon.IFlowControl) this;
            assignFlowFieldTileToTeamUnitSystem.FlowControl = (GameCommon.IFlowControl) this;
            moveOnFlowFieldTileSystem.FlowControl = (GameCommon.IFlowControl) this;

            neutralForceUnitHitCheckSystem.FlowControl = (GameCommon.IFlowControl) this;
            // pickupHitCheckSystem.FlowControl = (GameCommon.IFlowControl) this;

#endif

            //
            loadingDoneCheckSystem.Construct();
            settingDoneCheckSystem.Construct();
            cleanupSystem.Construct();
            loadGameFlowSystem.Construct();

            // App-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadAppHudSystem.Construct();
            setupAppHudSystem.Construct();
#endif

            // Preparation-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadPreparationHudSystem.Construct();
            setupPreparationHudSystem.Construct();
#endif

            // Stage-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadStageHudSystem.Construct();
            setupStageHudSystem.Construct();
#endif

#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadStageFlowSystem.Construct();
            loadBattleSystem.Construct();
            loadEnvironmentSystem.Construct();

            setupStageFlowSystem.Construct();
            setupBattleSystem.Construct();
            setupEnvironmentSystem.Construct();
            
            //
            manageUnitSystem.Construct();

            //
            createTeamForceUnitSystem.Construct();
            
            teamUnitToPathSystem.Construct();
            
            //
            adjustMoveToTargetFlowFieldSystem.Construct();

            cleanUpFlowFieldSystem.Construct();

            moveOnPathSystem.Construct();
            assignFlowFieldTileToTeamUnitSystem.Construct();
            moveOnFlowFieldTileSystem.Construct();
            
            neutralForceUnitHitCheckSystem.Construct();
            // pickupHitCheckSystem.Construct();
#endif

            // InitializationSystemGroup
            initializationSystemGroup.AddSystemToUpdateList(loadingDoneCheckSystem);
            initializationSystemGroup.AddSystemToUpdateList(settingDoneCheckSystem);
            initializationSystemGroup.AddSystemToUpdateList(cleanupSystem);
            initializationSystemGroup.AddSystemToUpdateList(cleanupStageUseEntitySystem);
            initializationSystemGroup.AddSystemToUpdateList(loadGameFlowSystem);

            // App-wide - InitializationSystemGroup
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadAppHudSystem);
            initializationSystemGroup.AddSystemToUpdateList(setupAppHudSystem);
#endif

            // Preparation-wide - InitializationSystemGroup
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadPreparationHudSystem);
            initializationSystemGroup.AddSystemToUpdateList(setupPreparationHudSystem);
#endif

            // Stage-wide - InitializationSystemGroup
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadStageHudSystem);
            initializationSystemGroup.AddSystemToUpdateList(setupStageHudSystem);
#endif

#if COMPLETE_PROJECT || BEHAVIOR_PROJECT || LEVEL_FLOW_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadStageFlowSystem);
            initializationSystemGroup.AddSystemToUpdateList(loadBattleSystem);
            initializationSystemGroup.AddSystemToUpdateList(loadEnvironmentSystem);
            
            initializationSystemGroup.AddSystemToUpdateList(setupStageFlowSystem);
            initializationSystemGroup.AddSystemToUpdateList(setupBattleSystem);
            initializationSystemGroup.AddSystemToUpdateList(setupEnvironmentSystem);

            //
            initializationSystemGroup.AddSystemToUpdateList(manageUnitSystem);
            
            initializationSystemGroup.AddSystemToUpdateList(createTeamForceUnitSystem);

            initializationSystemGroup.AddSystemToUpdateList(teamUnitToPathSystem);
            
            //
            simulationSystemGroup.AddSystemToUpdateList(adjustMoveToTargetFlowFieldSystem);
            simulationSystemGroup.AddSystemToUpdateList(cleanUpFlowFieldSystem);

            simulationSystemGroup.AddSystemToUpdateList(moveOnPathSystem);
            simulationSystemGroup.AddSystemToUpdateList(assignFlowFieldTileToTeamUnitSystem);
            simulationSystemGroup.AddSystemToUpdateList(moveOnFlowFieldTileSystem);

            simulationSystemGroup.AddSystemToUpdateList(neutralForceUnitHitCheckSystem);
            // simulationSystemGroup.AddSystemToUpdateList(pickupHitCheckSystem);
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
