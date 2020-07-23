namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

#if WALKIO_CREATURE
    using GameCreature = JoyBrick.Walkio.Game.Creature;
#endif

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
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

#if WALKIO_MOVE_CROWDSIMULATE
    using GameMoveCrowdSimulate = JoyBrick.Walkio.Game.Move.CrowdSimulate;
#endif

#if WALKIO_MOVE_WAYPOINT
    using GameMoveWaypoint = JoyBrick.Walkio.Game.Move.Waypoint;
#endif

    public partial class Bootstrap
    {
        //
        private void HandleSetupAfterEcs()
        {
            _logger.Debug($"Bootstrap Assist - HandleSetupAfterEcs");

#if WALKIO_CREATURE
            {
                var loadAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameCreature.LoadAssetSystem>();
                loadAssetSystem.ProvideExternalAsset = true;
            }
#endif

// #if WALKIO_FLOWCONTROL
//             {
//                 var loadAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameFlowControl.LoadAssetSystem>();
//                 loadAssetSystem.ProvideExternalAsset = true;
//             }
// #endif

// #if WALKIO_HUD_APP
//             {
//                 var loadAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameHudApp.LoadAssetSystem>();
//                 loadAssetSystem.ProvideExternalAsset = true;
//             }
// #endif

#if WALKIO_HUD_PREPARATION
            {
                var loadAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameHudPreparation.LoadAssetSystem>();
                loadAssetSystem.ProvideExternalAsset = true;
            }
#endif

#if WALKIO_HUD_STAGE
            {
                var loadAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameHudStage.LoadAssetSystem>();
                loadAssetSystem.ProvideExternalAsset = true;
            }
#endif

#if WALKIO_MOVE_CROWDSIMULATE
            {
                var loadAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameMoveCrowdSimulate.LoadAssetSystem>();
                loadAssetSystem.ProvideExternalAsset = true;
            }
#endif

#if WALKIO_MOVE_WAYPOINT
            {
                var loadAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameMoveWaypoint.LoadAssetSystem>();
                loadAssetSystem.ProvideExternalAsset = true;
            }
#endif

        }

        private void StartGameFlow()
        {
            _logger.Debug($"Bootstrap Assist - StartGameFlow");
        }
    }
}
