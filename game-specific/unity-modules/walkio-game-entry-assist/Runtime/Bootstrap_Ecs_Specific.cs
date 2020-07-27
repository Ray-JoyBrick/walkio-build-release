namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using UniRx;
    using UnityEngine;
    using Unity.Entities;

    //
    using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameExtension = JoyBrick.Walkio.Game.Extension;

    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;

    using GameHudApp = JoyBrick.Walkio.Game.Hud.App;
    using GameHudPreparation = JoyBrick.Walkio.Game.Hud.Preparation;
    using GameHudStage = JoyBrick.Walkio.Game.Hud.Stage;

    //
#if WALKIO_LEVEL
    using GameLevel = JoyBrick.Walkio.Game.Level;
#endif

#if WALKIO_MOVE_FLOWFIELD
    using GameMoveFlowField = JoyBrick.Walkio.Game.Move.FlowField;
#endif

#if WALKIO_LEVEL_ASSIST
    using GameLevelAssist = JoyBrick.Walkio.Game.Level.Assist;
#endif

#if WALKIO_MOVE_FLOWFIELD_ASSIST
    using GameMoveFlowFieldAssist = JoyBrick.Walkio.Game.Move.FlowField.Assist;
#endif

    public partial class Bootstrap
    {
        private void HudAppAssist_PrepareAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_HUD_APP_ASSIST
            _logger.Debug($"Bootstrap Assist - Module Creation - HudAppAssist_PrepareAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudApp.Assist.PrepareAssetSystem>();

            //
            createdSystem.SceneService = _assistable.RefGameObject.GetComponent<GameCommon.ISceneService>();
            createdSystem.CommandService = _assistable.RefGameObject.GetComponent<GameCommand.ICommandService>();
            createdSystem.ExtensionService = _assistable.RefGameObject.GetComponent<GameExtension.IExtensionService>();
            createdSystem.FlowControl = _assistable.RefGameObject.GetComponent<GameFlowControl.IFlowControl>();

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap Assist - No Module - HudAppAssist_PrepareAssetSystem");
#endif
        }

        private void HudAppAssist_SetupAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_HUD_APP_ASSIST
            _logger.Debug($"Bootstrap Assist - Module Creation - HudAppAssist_SetupAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudApp.SetupAssetSystem>();

            //
            createdSystem.FlowControl = _assistable.RefGameObject.GetComponent<GameFlowControl.IFlowControl>();

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap Assist - No Module - HudAppAssist_SetupAssetSystem");
#endif
        }

        private void HudStageAssist_PrepareAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_HUD_APP_ASSIST
            _logger.Debug($"Bootstrap Assist - Module Creation - HudStageAssist_PrepareAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudStage.Assist.PrepareAssetSystem>();

            //
            createdSystem.SceneService = _assistable.RefGameObject.GetComponent<GameCommon.ISceneService>();
            createdSystem.CommandService = _assistable.RefGameObject.GetComponent<GameCommand.ICommandService>();
            createdSystem.ExtensionService = _assistable.RefGameObject.GetComponent<GameExtension.IExtensionService>();
            createdSystem.FlowControl = _assistable.RefGameObject.GetComponent<GameFlowControl.IFlowControl>();

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap Assist - No Module - HudStageAssist_PrepareAssetSystem");
#endif
        }

        private void HudStageAssist_SetupAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_HUD_APP_ASSIST
            _logger.Debug($"Bootstrap Assist - Module Creation - HudStageAssist_SetupAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudStage.Assist.SetupAssetSystem>();

            //
            createdSystem.FlowControl = _assistable.RefGameObject.GetComponent<GameFlowControl.IFlowControl>();

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap Assist - No Module - HudStageAssist_SetupAssetSystem");
#endif
        }

        private void Level_PresentWorldSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - Level_PresentWorldSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameLevelAssist.PresentWorldSystem>();

            createdSystem.AssistGridWorldProvider = (GameLevelAssist.IGridWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - Level_PresentWorldSystem");
#endif
        }

        private void MoveFlowField_PresentChasedTargetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_PresentChasedTargetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowFieldAssist.PresentChasedTargetSystem>();

            //
            createdSystem.FlowFieldWorldProvider = _assistable.RefGameObject.GetComponent<GameMoveFlowField.IFlowFieldWorldProvider>();

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_PresentChasedTargetSystem");
#endif
        }

        private void MoveFlowField_PresentFlowFieldTileSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_PresentFlowFieldTileSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowFieldAssist.PresentFlowFieldTileSystem>();

            //
            createdSystem.GridWorldProvider = _assistable.RefGameObject.GetComponent<GameLevel.IGridWorldProvider>();
            createdSystem.FlowFieldWorldProvider = _assistable.RefGameObject.GetComponent<GameMoveFlowField.IFlowFieldWorldProvider>();
            createdSystem.AssistFlowFieldWorldProvider = (GameMoveFlowFieldAssist.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_PresentFlowFieldTileSystem");
#endif
        }

        private void MoveFlowField_PresentIndicationSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_PresentIndicationSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowFieldAssist.PresentIndicationSystem>();

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_PresentIndicationSystem");
#endif
        }

        private void MoveFlowField_PresentWorldSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_PresentWorldSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowFieldAssist.PresentWorldSystem>();

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_LoadAssetSystem");
#endif
        }


        private void MoveFlowField_PresentTemporaryPointIndicationSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_PresentTemporaryPointIndicationSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowFieldAssist.PresentTemporaryPointIndicationSystem>();

            //
            createdSystem.FlowFieldWorldProvider = _assistable.RefGameObject.GetComponent<GameMoveFlowField.IFlowFieldWorldProvider>();
            createdSystem.AssistFlowFieldWorldProvider = (GameMoveFlowFieldAssist.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_PresentTemporaryPointIndicationSystem");
#endif
        }

        private void MoveFlowField_RemoveTemporaryPointIndicationSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_RemoveTemporaryPointIndicationSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowFieldAssist.RemoveTemporaryPointIndicationSystem>();

            //
            createdSystem.FlowFieldWorldProvider = _assistable.RefGameObject.GetComponent<GameMoveFlowField.IFlowFieldWorldProvider>();

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_RemoveTemporaryPointIndicationSystem");
#endif
        }
    }
}
