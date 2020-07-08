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
    using GameLevel = JoyBrick.Walkio.Game.Level;
    using GameMove = JoyBrick.Walkio.Game.Move;

    public partial class Bootstrap
    {
        public void HandleAfterEcsSetup()
        {
            // Provide the testing use asset to LoadLevelSystem
            var loadLevelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameLevel.LoadLevelSystem>();
            loadLevelSystem.ProvideExternalAsset = true;

            loadLevelSystem.LevelSettingDataAsset = levelSettingData;
            loadLevelSystem.SceneInstance = sceneInstance;

            // Provide the testing use asset to LoadFlowFieldSystem
            var loadFlowFieldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameMove.FlowField.LoadFlowFieldSystem>();
            loadFlowFieldSystem.ProvideExternalAsset = true;

            loadFlowFieldSystem.SettingDataAsset = flowFieldSettingData;
        }
    }
}
