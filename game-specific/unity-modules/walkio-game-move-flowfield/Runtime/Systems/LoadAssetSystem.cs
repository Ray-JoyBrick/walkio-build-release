namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pathfinding;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
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
    public class LoadAssetSystem :
        SystemBase,
        GameCommon.ISystemContext
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadAssetSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private ScriptableObject _settingDataAsset;

        //
        private EntityArchetype _entityArchetype;
        
        //
        public GameCommon.IFlowControl FlowControl { get; set; }
        public Common.IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }
        
        //
        public string AtPart => "Stage";

        //
        public bool ProvideExternalAsset { get; set; }
        
        public ScriptableObject SettingDataAsset
        {
            get => _settingDataAsset;
            set => _settingDataAsset = value;
        }

        private async Task<ScriptableObject> Load(string levelAssetName, string specificLevelName)
        {
            var settingDataAssetName = $"Setting Data.asset";
            var settingDataAssetTask = GameCommon.Utility.AssetLoadingHelper.GetAsset<ScriptableObject>(settingDataAssetName);

            var settingDataAsset = await settingDataAssetTask;

            return settingDataAsset;
        }
        
        private void InternalLoadAsset(
            string levelAssetName, string specificLevelName,
            System.Action loadingDoneAction)
        {
            //
            Load(levelAssetName, specificLevelName).ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    _logger.Debug($"LoadAssetSystem - InternalLoadAsset");

                    //
                    _settingDataAsset = result;

                    var entity = EntityManager.CreateEntity(_entityArchetype);
                    FlowFieldWorldProvider.FlowFieldWorldEntity = entity;

                    var settingData = _settingDataAsset as Template.SettingData;
                    if (settingData != null)
                    {
                        FlowFieldWorldProvider.FlowFieldTileBlobAssetAuthoringPrefab =
                            settingData.flowFieldTileBlobAssetAuthoringPrefab;
                    }
                    
                    loadingDoneAction();
                })
                .AddTo(_compositeDisposable);
        }        

        private void LoadingAsset(string levelAssetName, string specificLevelName)
        {
            if (ProvideExternalAsset)
            {
                // Asset is provided from somewhere else, just notify that the asset loading is done
                GameCommon.Utility.FlowControlHelper.NotifyFinishIndividualLoadingAsset(FlowControl, AtPart);
            }
            else
            {
                InternalLoadAsset(
                    levelAssetName, specificLevelName,
                    () =>
                    {
                        GameCommon.Utility.FlowControlHelper.NotifyFinishIndividualLoadingAsset(FlowControl, AtPart);
                    });
            }
        }

        //
        public void Construct()
        {
            _logger.Debug($"LoadAssetSystem - Construct");

            //
            FlowControl.LoadingAsset
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    // Hard code here, should be given in event
                    var levelAssetName = $"Level Setting.asset";
                    var specificLevelName = $"Level 001";
                    LoadingAsset(levelAssetName, specificLevelName);
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            _logger.Debug($"LoadAssetSystem - OnCreate");

            base.OnCreate();

            _entityArchetype = EntityManager.CreateArchetype(
                typeof(Common.FlowFieldWorld),
                typeof(Common.FlowFieldWorldProperty));
        }

        protected override void OnUpdate() {}

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
