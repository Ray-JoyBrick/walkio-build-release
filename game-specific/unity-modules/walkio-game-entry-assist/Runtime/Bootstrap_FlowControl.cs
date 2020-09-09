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

#if WALKIO_MOVE_FLOWFIELD
    using GameMoveFlowField = JoyBrick.Walkio.Game.Move.FlowField;
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
//
// #if WALKIO_CREATURE
//             {
//                 var prepareAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameCreature.PrepareAssetSystem>();
//                 prepareAssetSystem.ProvideExternalAsset = true;
//             }
// #endif

// #if WALKIO_FLOWCONTROL
//             {
//                 var prepareAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameFlowControl.PrepareAssetSystem>();
//                 loadAssetSystem.ProvideExternalAsset = true;
//             }
// #endif

// #if WALKIO_HUD_APP
//             {
//                 var prepareAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameHudApp.PrepareAssetSystem>();
//                 prepareAssetSystem.ProvideExternalAsset = true;
//             }
// #endif

// #if WALKIO_HUD_PREPARATION
//             {
//                 var prepareAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameHudPreparation.PrepareAssetSystem>();
//                 prepareAssetSystem.ProvideExternalAsset = true;
//             }
// #endif

// #if WALKIO_HUD_STAGE
//             {
//                 var prepareAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameHudStage.PrepareAssetSystem>();
//                 prepareAssetSystem.ProvideExternalAsset = true;
//             }
// #endif

#if WALKIO_MOVE_CROWDSIMULATE
            {
                var prepareAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameMoveCrowdSimulate.PrepareAssetSystem>();
                prepareAssetSystem.ProvideExternalAsset = true;
            }
#endif

#if WALKIO_MOVE_FLOWFIELD
            {
                var prepareAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameMoveFlowField.PrepareAssetSystem>();
                prepareAssetSystem.ProvideExternalAsset = true;
            }
#endif

#if WALKIO_MOVE_WAYPOINT
            {
                var prepareAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameMoveWaypoint.PrepareAssetSystem>();
                prepareAssetSystem.ProvideExternalAsset = true;
            }
#endif

        }

        private void SignalStartLoadingAssetForStage()
        {
#if WALKIO_FLOWCONTROL
            Observable.Timer(System.TimeSpan.FromMilliseconds(500))
                .Subscribe(_ =>
                {
                    var flowControl = _assistable.RefGameObject.GetComponent<GameFlowControl.IFlowControl>();
                    // flowControl.StartLoadingAsset("Stage");
                    _logger.Debug($"Bootstrap Assist - Flow Control - SignalStartLoadingAssetForStage - Start loading stage");
                    flowControl.StartLoadingAsset(new GameFlowControl.FlowControlContext
                    {
                        Name = "Stage"
                    });
                })
                .AddTo(_compositeDisposable);
#endif
        }

        private void StartGameFlow()
        {
            _logger.Debug($"Bootstrap Assist - StartGameFlow");

            // SignalStartLoadingAssetForStage();
        }
    }
}
