namespace JoyBrick.Walkio.Game.Hud.Stage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

#if WALKIO_FLOWCONTROL
    [GameFlowControl.DoneLoadingAssetWait("Stage")]
#endif
    [DisableAutoCreation]
    public class LoadAssetSystem :
        SystemBase
        // GameCommon.ISystemContext
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadAssetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private ScriptableObject _hudSettingDataAsset;

        //
        public GameCommon.ISceneService SceneService { get; set; }        
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        public string AtPart => "Stage";

        //
        public bool ProvideExternalAsset { get; set; }

        private async Task<ScriptableObject> Load(string hudAssetName)
        {
            //
            var hudSettingAssetName = hudAssetName;
            var hudSettingAssetTask = GameCommon.Utility.AssetLoadingHelper.GetAsset<ScriptableObject>(hudSettingAssetName);

            var hudSettingAsset = await hudSettingAssetTask;

            return hudSettingAsset;
        }

        private void InternalLoadAsset(
            string hudAssetName,
            System.Action loadingDoneAction)
        {
            //
            Load(hudAssetName).ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    _hudSettingDataAsset = result;

                    loadingDoneAction();
                })
                .AddTo(_compositeDisposable);
        }

        private void LoadingAsset(string hudAssetName)
        {
            if (ProvideExternalAsset)
            {
                // Since the asset is provided, just notify instantly
#if WALKIO_FLOWCONTROL                
                FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                {
                    Name = "Stage"
                });
#endif
            }
            else
            {
                // Load internally then notify
                InternalLoadAsset(
                    hudAssetName,
                    () =>
                    {
#if WALKIO_FLOWCONTROL                        
                        FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                        {
                            Name = "Stage"
                        });
#endif
                    });
            }
        }

        //
        public void Construct()
        {
            _logger.Debug($"Module - LoadAssetSystem - Construct");

#if WALKIO_FLOWCONTROL
            FlowControl?.AssetLoadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - LoadAssetSystem - Construct - Receive AssetLoadingStarted");
                    // var hudAssetName = x.HudAssetName;
                    var hudAssetName = $"Hud - Stage/Hud Data";
                    LoadingAsset(hudAssetName);
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate() { }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable.Dispose();
        }
    }
}
