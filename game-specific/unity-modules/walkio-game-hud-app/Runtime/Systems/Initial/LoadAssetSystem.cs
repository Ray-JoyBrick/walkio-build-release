namespace JoyBrick.Walkio.Game.Move.FlowField
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

#if WALKIO_FLOWCONTROL_SYSTEM
        //
        private GameCommon.IFlowControl FlowControl { get; set; }
#endif

        public string AtPart => "App";

        //
        public bool ProvideExternalAsset { get; set; }

        // private async Task<ScriptableObject> Load(string hudAssetName)
        // {
        //     //
        //     var hudSettingAssetName = hudAssetName;
        //     var hudSettingAssetTask = GameCommon.Utility.AssetLoadingHelper.GetAsset<ScriptableObject>(hudSettingAssetName);

        //     var hudSettingAsset = await hudSettingAssetTask;

        //     return hudSettingAsset;
        // }

        private void InternalLoadAsset(
            string hudAssetName,
            System.Action loadingDoneAction)
        {
            // //
            // Load(hudAssetName).ToObservable()
            //     .ObserveOnMainThread()
            //     .SubscribeOnMainThread()
            //     .Subscribe(result =>
            //     {
            //         _hudSettingDataAsset = result;

            //         loadingDoneAction();
            //     })
            //     .AddTo(_compositeDisposable);
        }

        private void LoadingAsset(string hudAssetName)
        {
            void NotifyFinishIndividualLoadingAsset()
            {
#if WALKIO_FLOWCONTROL_SYSTEM                
                FlowControl.FinishIndividualLoadingAsset(new GameCommon.FlowControlContext
                {
                    Name = AtPart
                });
#endif
            }
            
            if (ProvideExternalAsset)
            {
                // Since the asset is provided, just notify instantly
                NotifyFinishIndividualLoadingAsset();
            }
            else
            {
                // Load internally then notify
                InternalLoadAsset(
                    hudAssetName,
                    NotifyFinishIndividualLoadingAsset);
            }
        }

        //
        public void Construct()
        {
            _logger.Debug($"Moduel - LoadAssetSystem - Construct");

#if WALKIO_FLOWCONTROL_SYSTEM
            //
            FlowControl.AssetLoadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    var hudAssetName = x.HudAssetName;
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
