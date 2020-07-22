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

    //
    // [GameCommon.DoneSettingAssetWait("Stage")]
    //
    [DisableAutoCreation]
    public class SetupAssetSystem :
        SystemBase,
        GameCommon.ISystemContext
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupAssetSystem));
        
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
        public string AtPart => "Stage";

        private async Task Setup()
        {
            var loadLevelSystem = World.GetExistingSystem<LoadLevelSystem>();
            var levelSettingDataAsset = loadLevelSystem.LevelSettingDataAsset;

            _levelSettingData = levelSettingDataAsset as Template.LevelSettingData;

            if (_levelSettingData == null)
            {
                _logger.Error($"SetupAssetSystem - Setup - levelSetting is null");
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
                        Name = AtPart
                    });
                })
                .AddTo(_compositeDisposable);
        }

        //
        public void Construct()
        {
            _logger.Debug($"SetupAssetSystem - Construct");

            //
            FlowControl.SettingAsset
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"SetupAssetSystem - Construct - Receive SettingAsset");

                    _levelSettingEntity = EntityManager.CreateEntity(_levelSettingEntityArchetype);

#if UNITY_EDITOR
                    World.DefaultGameObjectInjectionWorld.EntityManager.SetName(
                        _levelSettingEntity, $"Level Setting");
#endif

                    SettingAsset();
                })
                .AddTo(_compositeDisposable);
        }


        protected override void OnCreate()
        {
            _logger.Debug($"SetupAssetSystem - OnCreate");

            base.OnCreate();
            
            //
            _levelSettingEntityArchetype = EntityManager.CreateArchetype(
                typeof(GameCommon.LevelSetting),
                typeof(GameCommon.WaypointPathLookupAttachment),
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
