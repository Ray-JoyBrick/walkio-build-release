namespace JoyBrick.Walkio.Game.Move.Waypoint
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pathfinding;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    
    using GameLevel = JoyBrick.Walkio.Game.Level;
    using GameMove = JoyBrick.Walkio.Game.Move;

    //
    [GameCommon.DoneSettingAssetWait("Stage")]
    //
    [DisableAutoCreation]
    public class SetupWaypointSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupWaypointSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private EntityQuery _levelSettingEntityQuery;
        
        //
        public GameCommon.IFlowControl FlowControl { get; set; }
        
        //
        public bool ProvideExternalAsset { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"SetupWaypointSystem - Construct");

            //
            FlowControl.SettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"SetupWaypointSystem - Construct - Receive SettingAsset");

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
            _logger.Debug($"SetupWaypointSystem - Setup");

            var entity = _levelSettingEntityQuery.GetSingletonEntity();
            var levelSetting = EntityManager.GetComponentData<GameCommon.LevelSetting>(entity);

            var loadLevelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameLevel.LoadLevelSystem>();
            var levelSettingDataAsset = loadLevelSystem.LevelSettingDataAsset;
            var levelSettingData = levelSettingDataAsset as GameLevel.Template.LevelSettingData;

            if (levelSettingData)
            {
                _logger.Debug($"SetupWaypointSystem - Setup - has level setting data");

                // Setup for waypoint
                var waypointPathBlobAssetAuthoringPrefb = levelSettingData.WaypointPathBlobAssetAuthoringPrefab;

                // TODO: Remove the dependency to waypoint module if possible
                var waypointPaths = levelSettingData.waypointPaths;
                var waypointPathBlobAssetAuthoring = waypointPathBlobAssetAuthoringPrefb
                    .GetComponent<GameMove.Waypoint.WaypointPathBlobAssetAuthoring>();

                if (waypointPaths != null && waypointPathBlobAssetAuthoring != null)
                {
                    waypointPathBlobAssetAuthoring.waypointPaths = waypointPaths;
                
                    // Let conversion handles from game object to ecs entity
                    GameObject.Instantiate(waypointPathBlobAssetAuthoringPrefb);
                    _logger.Debug($"SetupLevelSystem - Setup - waypoint authoring is created");
                }
                else
                {
                    _logger.Debug($"SetupLevelSystem - Setup - waypoint data null");
                }
            }
        }

        protected override void OnCreate()
        {
            _logger.Debug($"SetupWaypointSystem - OnCreate");

            base.OnCreate();
            
            _levelSettingEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<GameCommon.LevelSetting>() 
                }
            });
        }

        protected override void OnUpdate() {}

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
