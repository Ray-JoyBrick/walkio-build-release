namespace JoyBrick.Walkio.Game.Move.FlowField
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

    //
    [GameCommon.DoneLoadingAssetWait("Stage")]
    //
    [DisableAutoCreation]
    public class LoadFlowFieldSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadFlowFieldSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private ScriptableObject _settingDataAsset;

        //
        private EntityArchetype _entityArchetype;

        //
        public GameCommon.IFlowControl FlowControl { get; set; }
        
        //
        public bool ProvideExternalAsset { get; set; }

        public ScriptableObject SettingDataAsset
        {
            get => _settingDataAsset;
            set => _settingDataAsset = value;
        }

        //
        public void Construct()
        {
            _logger.Debug($"LoadFlowFieldSystem - Construct");

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
                        _settingDataAsset = result;

                        //
                        FlowControl.FinishLoadingAsset(new GameCommon.FlowControlContext
                        {
                            Name = "Stage"
                        });
                    })
                    .AddTo(_compositeDisposable);
            }
        }

        private async Task<ScriptableObject> Load()
        {
            var settingDataAssetName = $"Setting Data.asset";
            var settingDataAssetTask = GameCommon.Utility.AssetLoadingHelper.GetAsset<ScriptableObject>(settingDataAssetName);

            var settingDataAsset = await settingDataAssetTask;

            return settingDataAsset;
        }

        protected override void OnCreate()
        {
            _logger.Debug($"LoadFlowFieldSystem - OnCreate");

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
