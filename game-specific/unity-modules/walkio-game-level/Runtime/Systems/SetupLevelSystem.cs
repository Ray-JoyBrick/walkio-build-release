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
    
    // using GameMove = JoyBrick.Walkio.Game.Move;

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
        private EntityArchetype _levelSettingEntityArchetype;
        private EntityArchetype _tileEntityArchetype;
        
        //
        private Entity _levelSettingEntity;

        private Template.LevelSettingData _levelSettingData;

        //
        public GameCommand.ICommandService CommandService { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }
        
        //
        public bool ProvideExternalAsset { get; set; }
        
        //
        public void Construct()
        {
            _logger.Debug($"SetupLevelSystem - Construct");

            //
            FlowControl.SettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"SetupLevelSystem - Construct - Receive SettingAsset");

                    _levelSettingEntity = EntityManager.CreateEntity(_levelSettingEntityArchetype);
#if UNITY_EDITOR

                    // var entityName = World.DefaultGameObjectInjectionWorld.EntityManager.GetName(_entity);
                    World.DefaultGameObjectInjectionWorld.EntityManager.SetName(
                        _levelSettingEntity, $"Level Setting");

#endif
                    
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
            var levelSettingDataAsset = loadLevelSystem.LevelSettingDataAsset;

            _levelSettingData = levelSettingDataAsset as Template.LevelSettingData;

            if (_levelSettingData == null)
            {
                _logger.Error($"SetupLevelSystem - Setup - levelSetting is null");
                return;
            }
            
            // Setup for level setting
            EntityManager.SetComponentData(_levelSettingEntity, new GameCommon.LevelSetting
            {
                HorizontalCellCount = 32,
                VerticalCellCount = 32
            });
            
            // Should delegate the move setup to individual move modules
        }

        protected override void OnCreate()
        {
            _logger.Debug($"SetupLevelSystem - OnCreate");

            base.OnCreate();
            
            //
            _levelSettingEntityArchetype = EntityManager.CreateArchetype(
                typeof(GameCommon.LevelSetting),
                typeof(GameCommon.WaypointPathLookupAttachment),
                typeof(GameCommon.StageUse));

            // _tileEntityArchetype = EntityManager.CreateArchetype(
            //     typeof(GameMove.LevelSetting),
            //     typeof(GameCommon.WaypointPathLookupAttachment),
            //     typeof(GameCommon.StageUse));
        }

        protected override void OnUpdate() {}

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
