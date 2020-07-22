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
    
#if WALKIO_HUD_APP
    using GameHudApp = JoyBrick.Walkio.Game.Hud.App;
#endif
    
    public partial class Bootstrap
    {
        //
        private void HandleSetupAfterEcs()
        {
            _logger.Debug($"Bootstrap Assist - HandleSetupAfterEcs");

#if WALKIO_HUD_APP
            {
                var loadAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameHudApp.LoadAssetSystem>();
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
