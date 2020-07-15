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
    
#if WALKIO_LEVEL
    using GameLevel = JoyBrick.Walkio.Game.Level;
#endif

    using GameCreature = JoyBrick.Walkio.Game.Creature;
    
#if WALKIO_FLOWFIELD || WALKIO_WAYPOINT || WALKIO_CROWDSIM
    using GameMove = JoyBrick.Walkio.Game.Move;
#endif

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

#if WALKIO_LEVEL_ASSIST
            var showHideEnvironmentSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameLevel.Assist.ShowHideEnvironmentSystem>();
#endif

#if WALKIO_FLOWFIELD_ASSIST
            var attachFlowFieldTileIndicationSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMove.FlowField.Assist.AttachFlowFieldTileIndicationSystem>();

            var flowFieldTileRenderSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMove.FlowField.Assist.FlowFieldTileRenderSystem>();

            var groupFlowFieldTileRenderSystems =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMove.FlowField.Assist.GroupFlowFieldTileRenderSystems>();
#endif


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

#if WALKIO_LEVEL_ASSIST
            showHideEnvironmentSystem.FlowControl = flowControl;
#endif

#if WALKIO_FLOWFIELD_ASSIST
            attachFlowFieldTileIndicationSystem.FlowControl = flowControl;
            
            flowFieldTileRenderSystem.SceneCamera = sceneProvider.SceneCamera;
            flowFieldTileRenderSystem.FlowControl = flowControl;
            flowFieldTileRenderSystem.SceneAssistProvider = (GameCommon.ISceneAssistProvider) this;

            groupFlowFieldTileRenderSystems.SceneCamera = sceneProvider.SceneCamera;
            groupFlowFieldTileRenderSystems.FlowControl = flowControl;
            groupFlowFieldTileRenderSystems.SceneAssistProvider = (GameCommon.ISceneAssistProvider) this;
#endif

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

#if WALKIO_LEVEL_ASSIST
            showHideEnvironmentSystem.Construct();
#endif

#if WALKIO_FLOWFIELD_ASSIST
            flowFieldTileRenderSystem.Construct();
            groupFlowFieldTileRenderSystems.Construct();
#endif

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

#if WALKIO_LEVEL_ASSIST
            initializationSystemGroup.AddSystemToUpdateList(showHideEnvironmentSystem);
#endif
            
#if WALKIO_FLOWFIELD_ASSIST
            initializationSystemGroup.AddSystemToUpdateList(attachFlowFieldTileIndicationSystem);
            
            presentationSystemGroup.AddSystemToUpdateList(flowFieldTileRenderSystem);
            presentationSystemGroup.AddSystemToUpdateList(groupFlowFieldTileRenderSystems);
#endif
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadStageHudSystem);
#endif
        }
    }
}
