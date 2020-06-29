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

    public partial class Bootstrap
    {
        public void HandleAfterEcsSetup()
        {
            var loadLevelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameLevel.LoadLevelSystem>();
            loadLevelSystem.ProvideExternalAsset = true;

            loadLevelSystem.WaypointDataAsset = waypointData;
            loadLevelSystem.WaypointPathBlobAssetAuthoringPrefab = waypointPathBlobAssetAuthoringPrefab;
        }
    }
}
