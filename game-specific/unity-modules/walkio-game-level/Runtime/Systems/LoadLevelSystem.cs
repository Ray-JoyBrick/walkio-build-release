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
    [GameCommon.DoneLoadingAssetWait("Stage")]
    //
    [DisableAutoCreation]
    public class LoadLevelSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadLevelSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private ScriptableObject _levelSettingDataAsset;
        private SceneInstance _sceneInstance;

        //
        private EntityArchetype _entityArchetype;

        //
        public GameCommand.ICommandService CommandService { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }
        
        //
        public bool ProvideExternalAsset { get; set; }

        public ScriptableObject LevelSettingDataAsset
        {
            get => _levelSettingDataAsset;
            set => _levelSettingDataAsset = value;
        }

        public SceneInstance SceneInstance
        {
            get => _sceneInstance;
            set => _sceneInstance = value;
        }

        //
        public void Construct()
        {
            _logger.Debug($"LoadLevelSystem - Construct");

            //
            FlowControl.LoadingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    LoadingAsset();
                })
                .AddTo(_compositeDisposable);
        }

        private void LoadingAsset()
        {
            if (ProvideExternalAsset)
            {
                FlowControl.FinishLoadingAsset(new GameCommon.FlowControlContext
                {
                    Name = "Stage"
                });
            }
            else
            {
                //
                Load().ToObservable()
                    .ObserveOnMainThread()
                    .SubscribeOnMainThread()
                    .Subscribe(result =>
                    {
                        //
                        (_levelSettingDataAsset, _sceneInstance) = result;

                        //
                        FlowControl.FinishLoadingAsset(new GameCommon.FlowControlContext
                        {
                            Name = "Stage"
                        });
                    })
                    .AddTo(_compositeDisposable);
            }
        }

        private async Task<(ScriptableObject, SceneInstance)> Load()
        {
            //
            var levelSettingAssetName = $"Level Setting.asset";
            var levelSettingAssetTask = GameCommon.Utility.AssetLoadingHelper.GetAsset<ScriptableObject>(levelSettingAssetName);

            var levelMainSceneAssetName = $"Level 001";
            var levelMainSceneAssetTask = GameCommon.Utility.AssetLoadingHelper.GetScene(levelMainSceneAssetName);
            
            var levelSettingAsset = await levelSettingAssetTask;
            var sceneInstance = await levelMainSceneAssetTask;

            return (levelSettingAsset, sceneInstance);
        }

        protected override void OnCreate()
        {
            _logger.Debug($"LoadLevelSystem - OnCreate");

            base.OnCreate();
        }

        protected override void OnUpdate() {}

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
