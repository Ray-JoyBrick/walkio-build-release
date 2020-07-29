namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;

    //
    using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCreature = JoyBrick.Walkio.Game.Creature;

#if WALKIO_EXTENSION
    using GameExtension = JoyBrick.Walkio.Game.Extension;
#endif

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

#if WALKIO_FLOWCONTROL_PREPARATION
    using GameFlowControlPreparation = JoyBrick.Walkio.Game.FlowControl.Preparation;
#endif

#if WALKIO_FLOWCONTROL_STAGE
    using GameFlowControlStage = JoyBrick.Walkio.Game.FlowControl.Stage;
#endif

#if WALKIO_HUD_APP
    using GameHudApp = JoyBrick.Walkio.Game.Hud.App;
#endif

#if WALKIO_HUD_PREPARATION
    using GameHudPreparation = JoyBrick.Walkio.Game.Hud.Preparation;
#endif

#if WALKIO_HUD_STAGE
    using GameHudStage = JoyBrick.Walkio.Game.Hud.Stage;
#endif

#if WALKIO_LEVEL
    using GameLevel = JoyBrick.Walkio.Game.Level;
#endif

#if WALKIO_MOVE_CROWDSIMULATE
    using GameMoveCrowdSimulate = JoyBrick.Walkio.Game.Move.CrowdSimulate;
#endif

#if WALKIO_MOVE_FLOWFIELD
    using GameMoveFlowField = JoyBrick.Walkio.Game.Move.FlowField;
#endif

#if WALKIO_MOVE_WAYPOINT
    using GameMoveWaypoint = JoyBrick.Walkio.Game.Move.Waypoint;
#endif

    public partial class Bootstrap
    {
        private void Creature_PrepareAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_CREATURE
            _logger.Debug($"Bootstrap - Module Creation - Creature_PrepareAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameCreature.PrepareAssetSystem>();

            //
            createdSystem.CreatureProvider = (GameCreature.ICreatureProvider) this;
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - Creature_PrepareAssetSystem");
#endif
        }
        
        private void Creature_SpawnTeamUnitSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_CREATURE
            _logger.Debug($"Bootstrap - Module Creation - Creature_SpawnTeamUnitSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameCreature.SpawnTeamUnitSystem>();

            //
            createdSystem.CreatureProvider = (GameCreature.ICreatureProvider) this;
            createdSystem.EcsSettingProvider = (GameCommon.IEcsSettingProvider) this;
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - Creature_SpawnTeamUnitSystem");
#endif
        }
        
        private void Creature_PresentUnitIndicationSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_CREATURE
            _logger.Debug($"Bootstrap - Module Creation - Creature_PresentUnitIndicationSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameCreature.PresentUnitIndicationSystem>();

            // //
            createdSystem.CreatureProvider = (GameCreature.ICreatureProvider) this;
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.LevelPropProvider = (GameLevel.ILevelPropProvider) this;
            
            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - Creature_PresentUnitIndicationSystem");
#endif
        }

        private void FlowControl_PrepareAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_FLOWCONTROL
            _logger.Debug($"Bootstrap - Module Creation - FlowControl_PrepareAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameFlowControl.PrepareAssetSystem>();

            //
            createdSystem.SceneService = (GameCommon.ISceneService) this;
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.ExtensionService = (GameExtension.IExtensionService) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - FlowControl_PrepareAssetSystem");
#endif
        }

        private void FlowControl_LoadingDoneCheckSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_FLOWCONTROL
            _logger.Debug($"Bootstrap - Module Creation - FlowControl_LoadingDoneCheckSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameFlowControl.LoadingDoneCheckSystem>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - FlowControl_LoadingDoneCheckSystem");
#endif
        }

        private void FlowControl_SettingDoneCheckSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_FLOWCONTROL
            _logger.Debug($"Bootstrap - Module Creation - FlowControl_SettingDoneCheckSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameFlowControl.SettingDoneCheckSystem>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - FlowControl_SettingDoneCheckSystem");
#endif
        }

        private void FlowControlPreparation_PrepareAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_FLOWCONTROL
            _logger.Debug($"Bootstrap - Module Creation - FlowControlPreparation_PrepareAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameFlowControlPreparation.PrepareAssetSystem>();

            //
            createdSystem.SceneService = (GameCommon.ISceneService) this;
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.ExtensionService = (GameExtension.IExtensionService) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - FlowControlPreparation_PrepareAssetSystem");
#endif
        }

        private void FlowControlStage_PrepareAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_FLOWCONTROL_STAGE
            _logger.Debug($"Bootstrap - Module Creation - FlowControlStage_PrepareAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameFlowControlStage.PrepareAssetSystem>();

            //
            createdSystem.SceneService = (GameCommon.ISceneService) this;
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.ExtensionService = (GameExtension.IExtensionService) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - FlowControlStage_PrepareAssetSystem");
#endif
        }

        private void HudApp_PrepareAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_HUD_APP
            _logger.Debug($"Bootstrap - Module Creation - HudApp_PrepareAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudApp.PrepareAssetSystem>();

            //
            createdSystem.SceneService = (GameCommon.ISceneService) this;
            createdSystem.CommandService = (GameCommand.ICommandService) this;
            createdSystem.ExtensionService = (GameExtension.IExtensionService) this;
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - HudApp_PrepareAssetSystem");
#endif
        }

        private void HudApp_SetupAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_HUD_APP
            _logger.Debug($"Bootstrap - Module Creation - HudApp_SetupAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudApp.SetupAssetSystem>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - HudApp_SetupAssetSystem");
#endif
        }

        private void HudPreparation_PrepareAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_HUD_PREPARATION
            _logger.Debug($"Bootstrap - Module Creation - HudPreparation_PrepareAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudPreparation.PrepareAssetSystem>();

            //
            createdSystem.SceneService = (GameCommon.ISceneService) this;
            createdSystem.CommandService = (GameCommand.ICommandService) this;
            createdSystem.ExtensionService = (GameExtension.IExtensionService) this;

            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - HudPreparation_PrepareAssetSystem");
#endif
        }

        private void HudPreparation_SetupAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_HUD_APP
            _logger.Debug($"Bootstrap - Module Creation - HudPreparation_SetupAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudPreparation.SetupAssetSystem>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - HudPreparation_SetupAssetSystem");
#endif
        }

        private void HudStage_PrepareAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_HUD_STAGE
            _logger.Debug($"Bootstrap - Module Creation - HudStage_PrepareAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudStage.PrepareAssetSystem>();

            //
            createdSystem.SceneService = (GameCommon.ISceneService) this;
            createdSystem.CommandService = (GameCommand.ICommandService) this;
            createdSystem.ExtensionService = (GameExtension.IExtensionService) this;
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - HudStage_PrepareAssetSystem");
#endif
        }

        private void HudStage_SetupAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_HUD_APP
            _logger.Debug($"Bootstrap - Module Creation - HudStage_SetupAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudStage.SetupAssetSystem>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - HudStage_SetupAssetSystem");
#endif
        }

        private void Level_PrepareAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_LEVEL
            _logger.Debug($"Bootstrap - Module Creation - Level_PrepareAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameLevel.PrepareAssetSystem>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.GridWorldProvider = (GameLevel.IGridWorldProvider) this;
            createdSystem.LevelPropProvider = (GameLevel.ILevelPropProvider) this;
                //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - Level_PrepareAssetSystem");
#endif
        }

        private void Level_SetupAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_LEVEL
            _logger.Debug($"Bootstrap - Module Creation - Level_SetupAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameLevel.SetupAssetSystem>();

            createdSystem.CreatureProvider = (GameCreature.ICreatureProvider) this;
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - Level_SetupAssetSystem");
#endif
        }

        private void MoveCrowdSimulate_PrepareAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_CROWDSIMULATE
            _logger.Debug($"Bootstrap - Module Creation - MoveCrowdSimulate_PrepareAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveCrowdSimulate.PrepareAssetSystem>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            // createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveCrowdSimulate_PrepareAssetSystem");
#endif
        }

        private void MoveCrowdSimulate_SetupAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_CROWDSIMULATE
            _logger.Debug($"Bootstrap - Module Creation - MoveCrowdSimulate_SetupAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveCrowdSimulate.SetupAssetSystem>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            // createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveCrowdSimulate_SetupAssetSystem");
#endif
        }

        private void MoveCrowdSimulate_SystemA(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_CROWDSIMULATE
            _logger.Debug($"Bootstrap - Module Creation - MoveCrowdSimulate_SystemA");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveCrowdSimulate.SystemA>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            // createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveCrowdSimulate_SystemA");
#endif
        }

        private void MoveFlowField_PrepareAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_PrepareAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.PrepareAssetSystem>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_PrepareAssetSystem");
#endif
        }

        private void MoveFlowField_SetupAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SetupAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SetupAssetSystem>();

            //
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SetupAssetSystem");
#endif
        }

        private void MoveFlowField_SetupInitialLeadingToSetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SetupInitialLeadingToSetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SetupInitialLeadingToSetSystem>();

            //
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SetupInitialLeadingToSetSystem");
#endif
        }

        private void MoveFlowField_CheckTargetAtTileChangeSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_CheckTargetAtTileChangeSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.CheckTargetAtTileChangeSystem>();

            //
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_CheckTargetAtTileChangeSystem");
#endif
        }

        private void MoveFlowField_SystemA(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SystemA");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SystemA>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SystemA");
#endif
        }

        private void MoveFlowField_SystemB(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SystemB");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SystemB>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SystemB");
#endif
        }

        private void MoveFlowField_SystemC(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SystemC");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SystemC>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SystemC");
#endif
        }

        private void MoveFlowField_SystemD(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SystemD");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SystemD>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SystemD");
#endif
        }

        private void MoveFlowField_SystemE(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SystemE");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SystemE>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SystemE");
#endif
        }

        private void MoveFlowField_SystemH01(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SystemH01");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SystemH01>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            // createdSystem.GridWorldProvider = (GameLevel.IGridWorldProvider) this;
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SystemH01");
#endif
        }


        private void MoveFlowField_SystemM01(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SystemM01");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SystemM01>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            // createdSystem.GridWorldProvider = (GameLevel.IGridWorldProvider) this;
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SystemM01");
#endif
        }

        private void MoveFlowField_SystemM02(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SystemM02");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SystemM02>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SystemM02");
#endif
        }
        
        private void MoveFlowField_SystemM02_2(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SystemM02_2");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SystemM02_2>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SystemM02_2");
#endif
        }

        private void MoveFlowField_SystemM03(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SystemM03");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SystemM03>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SystemM03");
#endif
        }

        private void MoveFlowField_SystemM08(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SystemM08");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SystemM08>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SystemM08");
#endif
        }

        private void MoveFlowField_SystemM09(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SystemM09");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SystemM09>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SystemM09");
#endif
        }

        private void MoveWaypoint_PrepareAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_WAYPOINT
            _logger.Debug($"Bootstrap - Module Creation - MoveWaypoint_PrepareAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveWaypoint.PrepareAssetSystem>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            // createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveWaypoint_PrepareAssetSystem");
#endif
        }

        private void MoveWaypoint_SetupAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_WAYPOINT
            _logger.Debug($"Bootstrap - Module Creation - MoveWaypoint_SetupAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveWaypoint.SetupAssetSystem>();

            //
            createdSystem.FlowControl = (GameFlowControl.IFlowControl) this;
            // createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveWaypoint_SetupAssetSystem");
#endif
        }
    }
}
