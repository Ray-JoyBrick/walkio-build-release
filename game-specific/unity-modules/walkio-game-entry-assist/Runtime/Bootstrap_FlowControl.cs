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

    public partial class Bootstrap
    {
        //
        private void HandleSetupAfterEcs()
        {
            _logger.Debug($"Bootstrap Assist - HandleSetupAfterEcs");

// #if WALKIO_FLOWCONTROL
//             {
//                 var loadAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameFlowControl.LoadAssetSystem>();
//                 loadAssetSystem.ProvideExternalAsset = true;
//             }
// #endif

#if WALKIO_HUD_APP
            {
                var loadAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameHudApp.LoadAssetSystem>();
                loadAssetSystem.ProvideExternalAsset = true;
            }
#endif

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

        }

        private void StartGameFlow()
        {
            _logger.Debug($"Bootstrap Assist - StartGameFlow");
        }
    }
}
