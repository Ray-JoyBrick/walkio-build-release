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
#if WALKIO_LEVEL
    using GameLevel = JoyBrick.Walkio.Game.Level;
#endif

#if WALKIO_FLOWFIELD || WALKIO_WAYPOINT || WALKIO_CROWDSIM
    using GameMove = JoyBrick.Walkio.Game.Move;
#endif

    public partial class Bootstrap
    {
        public void HandleAfterEcsSetup()
        {
            // Provide the testing use asset to LoadLevelSystem
#if WALKIO_LEVEL
            var loadLevelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameLevel.LoadLevelSystem>();
            loadLevelSystem.ProvideExternalAsset = true;

            loadLevelSystem.LevelSettingDataAsset = levelSettingData;
            loadLevelSystem.SceneInstance = sceneInstance;
#endif

            // Provide the testing use asset to LoadFlowFieldSystem
#if WALKIO_FLOWFIELD
            var loadFlowFieldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameMove.FlowField.LoadFlowFieldSystem>();
            loadFlowFieldSystem.ProvideExternalAsset = true;

            loadFlowFieldSystem.SettingDataAsset = flowFieldSettingData;
#endif
        }
    }
}
