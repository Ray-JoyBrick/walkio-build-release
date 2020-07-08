namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;
    
    using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameCommon = JoyBrick.Walkio.Game.Common;
    
    using GameLevel = JoyBrick.Walkio.Game.Level;
    using GameCreature = JoyBrick.Walkio.Game.Creature;
    using GameMove = JoyBrick.Walkio.Game.Move;

#if COMPLETE_PROJECT || BEHAVIOR_PROJECT

    using GameHudAppAssist = JoyBrick.Walkio.Game.Hud.App.Assist;
    // using GameHudPreparation = JoyBrick.Walkio.Game.Hud.Preparation;
    using GameHudStageAssist = JoyBrick.Walkio.Game.Hud.Stage.Assist;
    
#endif

    public partial class Bootstrap
    {
        private void SetupEcsWorld()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            var eventEntityArchetype =
                entityManager.CreateArchetype(
                    typeof(GameCreature.Assist.TeamUnitSpawnTimer),
                    typeof(GameCreature.Assist.TeamUnitSpawnTimerProperty));
            var eventEntity = entityManager.CreateEntity(eventEntityArchetype);

            entityManager.SetComponentData(eventEntity, new GameCreature.Assist.TeamUnitSpawnTimerProperty
            {
                IntervalMax = timeToSpawnTeamUnit
            });
        }
        
        private void SetupEcsSystem()
        {
            _logger.Debug($"Bootstrap Assist - SetupEcsSystem");

            var flowControl = _assistable.RefGameObject.GetComponent<GameCommon.IFlowControl>();
            var sceneProvider = _assistable.RefGameObject.GetComponent<GameCommon.ISceneProvider>();
            
            //
            var initializationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();
            var simulationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>();
            var presentationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PresentationSystemGroup>();
            
            // App-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            var loadAppHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudAppAssist.LoadAppHudSystem>();
            // var setupAppHudSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameHudAppAssist.SetupAppHudSystem>();
#endif
            
            // Stage-wide
            
            var timedSpawnTeamUnitSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameCreature.Assist.TimedSpawnTeamUnitSystem>();

            var showHideEnvironmentSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameLevel.Assist.ShowHideEnvironmentSystem>();

            var flowFieldTileRenderSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMove.FlowField.Assist.FlowFieldTileRenderSystem>();

#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            var loadStageHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudStageAssist.LoadStageHudSystem>();
#endif

            // App-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadAppHudSystem.RefBootstrap = _assistable.RefGameObject;
            loadAppHudSystem.CommandService = _assistable.RefGameObject.GetComponent<GameCommand.ICommandService>();
            // // loadAppHudSystem.InfoPresenter = (GameCommand.IInfoPresenter) this;
            loadAppHudSystem.FlowControl = _assistable.RefGameObject.GetComponent<GameCommon.IFlowControl>();
            // // setupAppHudSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif

            // Stage-wide
            timedSpawnTeamUnitSystem.FlowControl = flowControl;

            showHideEnvironmentSystem.FlowControl = flowControl;

            flowFieldTileRenderSystem.SceneCamera = sceneProvider.SceneCamera;
            flowFieldTileRenderSystem.FlowControl = flowControl;
            flowFieldTileRenderSystem.SceneAssistProvider = (GameCommon.ISceneAssistProvider) this;
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadStageHudSystem.RefBootstrap = _assistable.RefGameObject;
            loadStageHudSystem.CommandService = _assistable.RefGameObject.GetComponent<GameCommand.ICommandService>();
            // // loadStageHudSystem.InfoPresenter = (GameCommand.IInfoPresenter) this;
            loadStageHudSystem.FlowControl = _assistable.RefGameObject.GetComponent<GameCommon.IFlowControl>();
#endif

            // App-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadAppHudSystem.Construct();
            // // setupAppHudSystem.Construct();
#endif

            // Stage-wide
            timedSpawnTeamUnitSystem.Construct();

            showHideEnvironmentSystem.Construct();
            flowFieldTileRenderSystem.Construct();
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadStageHudSystem.Construct();
#endif

            // App-wide - InitializationSystemGroup
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadAppHudSystem);
            // // initializationSystemGroup.AddSystemToUpdateList(setupAppHudSystem);
#endif

            // Stage-wide - InitializationSystemGroup
            initializationSystemGroup.AddSystemToUpdateList(timedSpawnTeamUnitSystem);
            
            initializationSystemGroup.AddSystemToUpdateList(showHideEnvironmentSystem);
            
            presentationSystemGroup.AddSystemToUpdateList(flowFieldTileRenderSystem);
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadStageHudSystem);
#endif
        }
    }
}
