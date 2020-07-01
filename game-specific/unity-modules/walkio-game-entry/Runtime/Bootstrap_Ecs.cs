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
    
    using GameLevel = JoyBrick.Walkio.Game.Level;

    using GamePlaceholder = JoyBrick.Walkio.Game.Placeholder;
    using GameCreature = JoyBrick.Walkio.Game.Creature;
    using GameMove = JoyBrick.Walkio.Game.Move;

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
        GameCommon.IEcsSettingProvider
    {
        // //
        // private EntityManager _entityManager;

        private GameObjectConversionSettings _gameObjectConversionSettings;

        public GameObjectConversionSettings GameObjectConversionSettings => _gameObjectConversionSettings;

        private void SetupEcsWorld()
        {
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
            var emptyAppLoadingSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GamePlaceholder.EmptyAppLoadingSystem>();
            var emptyAppSettingSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GamePlaceholder.EmptyAppSettingSystem>();
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            var loadAppHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudApp.LoadAppHudSystem>();
            var setupAppHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudApp.SetupAppHudSystem>();
#endif
            
            // Preparation-wide
            var emptyPreparationLoadingSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GamePlaceholder.EmptyPreparationLoadingSystem>();
            var emptyPreparationSettingSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GamePlaceholder.EmptyPreparationSettingSystem>();
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            var loadPreparationHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudPreparation.LoadPreparationHudSystem>();
            var setupPreparationHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudPreparation.SetupPreparationHudSystem>();
#endif

            // Stage-wide
            var emptyStageLoadingSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GamePlaceholder.EmptyStageLoadingSystem>();

            var loadLevelSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameLevel.LoadLevelSystem>();

            var emptyStageSettingSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GamePlaceholder.EmptyStageSettingSystem>();

            var setupLevelSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameLevel.SetupLevelSystem>();

            var replaceWaypointMoveIndicationSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMove.Waypoint.ReplaceWaypointMoveIndicationSystem>();

            var replaceFlowFieldMoveIndicationSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMove.FlowField.ReplaceFlowFieldMoveIndicationSystem>();

            var spawnNeutralUnitSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameCreature.SpawnNeutralUnitSystem>();

            var spawnTeamUnitSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameCreature.SpawnTeamUnitSystem>();
            
            var moveOnWaypointPathSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMove.Waypoint.MoveOnWaypointPathSystem>();

            //
            var teamUnitToPathSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMove.FlowField.TeamUnitToPathSystem>();
            
            var adjustMoveToTargetFlowFieldSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMove.FlowField.AdjustMoveToTargetFlowFieldSystem>();
            var cleanUpFlowFieldSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMove.FlowField.CleanUpFlowFieldSystem>();

            
            var assignFlowFieldTileToTeamUnitSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMove.FlowField.AssignFlowFieldTileToTeamUnitSystem>();

            var moveOnFlowFieldTileSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMove.FlowField.MoveOnFlowFieldTileSystem>();

            var radixSortSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMove.CrowdSim.RadixSortSystem>();
            var crowdSimSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMove.CrowdSim.CrowdSimSystem>();
            
            //
            var unitIndicationRenderSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameCreature.UnitIndicationRenderSystem>();

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

            // //
            // var adjustMoveToTargetFlowFieldSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameBattle.AdjustMoveToTargetFlowFieldSystem>();
            // var cleanUpFlowFieldSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameBattle.CleanUpFlowFieldSystem>();
            //
            // //
            // var moveOnPathSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameBattle.MoveOnPathSystem>();
            //
            // var assignFlowFieldTileToTeamUnitSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameBattle.AssignFlowFieldTileToTeamUnitSystem>();
            //
            // var moveOnFlowFieldTileSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameBattle.MoveOnFlowFieldTileSystem>();

            var neutralForceUnitHitCheckSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameBattle.NeutralForceUnitHitCheckSystem>();

            // var pickupHitCheckSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameBattle.PickupHitCheckSystem>();
            
#endif

            //
            loadingDoneCheckSystem.GameSettingProvider = (GameCommon.IGameSettingProvider) this;
            loadingDoneCheckSystem.FlowControl = (GameCommon.IFlowControl) this;
            settingDoneCheckSystem.GameSettingProvider = (GameCommon.IGameSettingProvider) this;
            settingDoneCheckSystem.FlowControl = (GameCommon.IFlowControl) this;
            cleanupSystem.FlowControl = (GameCommon.IFlowControl) this;
            loadGameFlowSystem.RefBootstrap = this.gameObject;
            loadGameFlowSystem.FlowControl = (GameCommon.IFlowControl) this;
            
            // App-wide
            emptyAppLoadingSystem.FlowControl = (GameCommon.IFlowControl) this;
            emptyAppSettingSystem.FlowControl = (GameCommon.IFlowControl) this;
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadAppHudSystem.RefBootstrap = this.gameObject;
            loadAppHudSystem.CommandService = (GameCommand.ICommandService) this;
            // loadAppHudSystem.InfoPresenter = (GameCommand.IInfoPresenter) this;
            loadAppHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            setupAppHudSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif
            
            // Preparation-wide
            emptyPreparationLoadingSystem.FlowControl = (GameCommon.IFlowControl) this;
            emptyPreparationSettingSystem.FlowControl = (GameCommon.IFlowControl) this;
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadPreparationHudSystem.RefBootstrap = this.gameObject;
            loadPreparationHudSystem.CommandService = (GameCommand.ICommandService) this;
            // loadPreparationHudSystem.InfoPresenter = (GameCommand.IInfoPresenter) this;
            loadPreparationHudSystem.FlowControl = (GameCommon.IFlowControl) this;
            setupPreparationHudSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif

            // Stage-wide
            emptyStageLoadingSystem.FlowControl = (GameCommon.IFlowControl) this;
            loadLevelSystem.FlowControl = (GameCommon.IFlowControl) this;
            
            emptyStageSettingSystem.FlowControl = (GameCommon.IFlowControl) this;
            setupLevelSystem.FlowControl = (GameCommon.IFlowControl) this;
            
            spawnNeutralUnitSystem.FlowControl = (GameCommon.IFlowControl) this;
            spawnNeutralUnitSystem.EcsSettingProvider = (GameCommon.IEcsSettingProvider) this;
            spawnNeutralUnitSystem.NeutralUnitPrefab = neutralUnitPrefab;

            spawnTeamUnitSystem.FlowControl = (GameCommon.IFlowControl) this;
            spawnTeamUnitSystem.EcsSettingProvider = (GameCommon.IEcsSettingProvider) this;
            spawnTeamUnitSystem.TeamUnitPrefabs = teamUnitPrefabs;
            
            moveOnWaypointPathSystem.FlowControl = (GameCommon.IFlowControl) this;

            //
            teamUnitToPathSystem.FlowControl = (GameCommon.IFlowControl) this;
            teamUnitToPathSystem.AStarPathService = (GameCommon.IAStarPathService) this;
            
            adjustMoveToTargetFlowFieldSystem.FlowControl = (GameCommon.IFlowControl) this;
            
            cleanUpFlowFieldSystem.FlowControl = (GameCommon.IFlowControl) this;

            assignFlowFieldTileToTeamUnitSystem.FlowControl = (GameCommon.IFlowControl) this;
            moveOnFlowFieldTileSystem.FlowControl = (GameCommon.IFlowControl) this;

            crowdSimSystem.FlowControl = (GameCommon.IFlowControl) this;

            //
            unitIndicationRenderSystem.SceneCamera = camera;
            unitIndicationRenderSystem.UnitMeshs = unitMeshs;
            unitIndicationRenderSystem.UnitMaterials = unitMaterials;

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
            emptyAppLoadingSystem.Construct();
            emptyAppSettingSystem.Construct();
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadAppHudSystem.Construct();
            setupAppHudSystem.Construct();
#endif

            // Preparation-wide
            emptyPreparationLoadingSystem.Construct();
            emptyPreparationSettingSystem.Construct();
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadPreparationHudSystem.Construct();
            setupPreparationHudSystem.Construct();
#endif

            // Stage-wide
            emptyStageLoadingSystem.Construct();
            loadLevelSystem.Construct();

            emptyStageSettingSystem.Construct();
            setupLevelSystem.Construct();
            
            spawnNeutralUnitSystem.Construct();
            spawnTeamUnitSystem.Construct();
            
            moveOnWaypointPathSystem.Construct();
            
            //
            teamUnitToPathSystem.Construct();
            
            adjustMoveToTargetFlowFieldSystem.Construct();

            cleanUpFlowFieldSystem.Construct();

            assignFlowFieldTileToTeamUnitSystem.Construct();
            moveOnFlowFieldTileSystem.Construct();
            
            //
            crowdSimSystem.Construct();
            
            //
            unitIndicationRenderSystem.Construct();

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
            initializationSystemGroup.AddSystemToUpdateList(emptyAppLoadingSystem);
            initializationSystemGroup.AddSystemToUpdateList(emptyAppSettingSystem);
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadAppHudSystem);
            initializationSystemGroup.AddSystemToUpdateList(setupAppHudSystem);
#endif

            // Preparation-wide - InitializationSystemGroup
            initializationSystemGroup.AddSystemToUpdateList(emptyPreparationLoadingSystem);
            initializationSystemGroup.AddSystemToUpdateList(emptyPreparationSettingSystem);
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadPreparationHudSystem);
            initializationSystemGroup.AddSystemToUpdateList(setupPreparationHudSystem);
#endif

            // Stage-wide - InitializationSystemGroup
            initializationSystemGroup.AddSystemToUpdateList(emptyStageLoadingSystem);
            initializationSystemGroup.AddSystemToUpdateList(loadLevelSystem);

            initializationSystemGroup.AddSystemToUpdateList(emptyStageSettingSystem);
            initializationSystemGroup.AddSystemToUpdateList(setupLevelSystem);

            initializationSystemGroup.AddSystemToUpdateList(replaceWaypointMoveIndicationSystem);
            initializationSystemGroup.AddSystemToUpdateList(replaceFlowFieldMoveIndicationSystem);
            
            initializationSystemGroup.AddSystemToUpdateList(spawnNeutralUnitSystem);
            initializationSystemGroup.AddSystemToUpdateList(spawnTeamUnitSystem);

            initializationSystemGroup.AddSystemToUpdateList(teamUnitToPathSystem);

            simulationSystemGroup.AddSystemToUpdateList(moveOnWaypointPathSystem);

            //
            simulationSystemGroup.AddSystemToUpdateList(adjustMoveToTargetFlowFieldSystem);
            simulationSystemGroup.AddSystemToUpdateList(cleanUpFlowFieldSystem);

            simulationSystemGroup.AddSystemToUpdateList(adjustMoveToTargetFlowFieldSystem);
            simulationSystemGroup.AddSystemToUpdateList(assignFlowFieldTileToTeamUnitSystem);
            simulationSystemGroup.AddSystemToUpdateList(moveOnFlowFieldTileSystem);
            
            //
            simulationSystemGroup.AddSystemToUpdateList(radixSortSystem);
            simulationSystemGroup.AddSystemToUpdateList(crowdSimSystem);

            //
            presentationSystemGroup.AddSystemToUpdateList(unitIndicationRenderSystem);
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
    }
}
