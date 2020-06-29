namespace JoyBrick.Walkio.Game.Level
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pathfinding;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;
    
    using GameMove = JoyBrick.Walkio.Game.Move;

    //
    [GameCommon.DoneSettingAssetWait("Stage")]
    //
    [DisableAutoCreation]
    public class SetupLevelSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupLevelSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private EntityArchetype _entityArchetype;

        //
        public GameCommand.ICommandService CommandService { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }
        
        //
        public bool ProvideExternalAsset { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"SetupLevelSystem - Construct");

            base.OnCreate();

            //
            FlowControl.SettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"SetupLevelSystem - Construct - Receive SettingAsset");

                    EntityManager.CreateEntity(_entityArchetype);
                    
                    SettingAsset();
                })
                .AddTo(_compositeDisposable);             
        }

        private void SettingAsset()
        {
            //
            Setup().ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    FlowControl.FinishSetting(new GameCommon.FlowControlContext
                    {
                        Name = "Stage"
                    });
                })
                .AddTo(_compositeDisposable);
        }

        private async Task Setup()
        {
            var loadLevelSystem = World.GetExistingSystem<LoadLevelSystem>();
            var waypointDataAsset = loadLevelSystem.WaypointDataAsset;
            var waypointPathBlobAssetAuthoringPrefb = loadLevelSystem.WaypointPathBlobAssetAuthoringPrefab;

            var waypointData = waypointDataAsset as GameMove.Template.WaypointData;
            var waypointPathBlobAssetAuthoring = waypointPathBlobAssetAuthoringPrefb
                .GetComponent<GameMove.WaypointPathBlobAssetAuthoring>();

            if (waypointData != null && waypointPathBlobAssetAuthoring != null)
            {
                waypointPathBlobAssetAuthoring.waypointDataAsset = waypointData;
                
                // Let conversion handles from game object to ecs entity
                GameObject.Instantiate(waypointPathBlobAssetAuthoringPrefb);
                _logger.Debug($"SetupLevelSystem - Setup - waypoint authoring is created");
            }
            else
            {
                _logger.Debug($"SetupLevelSystem - Setup - waypoint data null");
            }
            
            await Task.Delay(System.TimeSpan.FromMilliseconds(2000));
        }

        protected override void OnCreate()
        {
            _logger.Debug($"SetupLevelSystem - OnCreate");

            base.OnCreate();
            
            //
            _entityArchetype = EntityManager.CreateArchetype(
                typeof(GameMove.Waypoint.WaypointPathLookupAttachment),
                typeof(GameCommon.StageUse));
        }

        protected override void OnUpdate() {}

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
