namespace JoyBrick.Walkio.Game.Move.Waypoint
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

    public partial class PrepareAssetSystem : SystemBase
    {
        //
        public bool ProvideExternalAsset { get; set; }

        //
        private async Task<ScriptableObject> Load()
        {
            var settingDataAssetName = $"Setting Data.asset";
            var settingDataAssetTask = GameCommon.Utility.AssetLoadingHelper.GetAsset<ScriptableObject>(settingDataAssetName);

            var settingDataAsset = await settingDataAssetTask;

            return settingDataAsset;
        }

        private void InternalLoadAsset(
            System.Action loadingDoneAction)
        {
            // //
            // Load().ToObservable()
            //     .ObserveOnMainThread()
            //     .SubscribeOnMainThread()
            //     .Subscribe(result =>
            //     {
            //         _logger.Debug($"Module - Move - Waypoint - LoadAssetSystem - InternalLoadAsset");
            //
            //         //
            //         // _settingDataAsset = result;
            //
            //         loadingDoneAction();
            //     })
            //     .AddTo(_compositeDisposable);
            Observable.Timer(System.TimeSpan.FromMilliseconds(1000))
                .Subscribe(_ =>
                {
                    _logger.Debug($"Module - Move - Waypoint - LoadAssetSystem - InternalLoadAsset");

                    //
                    // _settingDataAsset = result;

                    loadingDoneAction();

                })
                .AddTo(_compositeDisposable);

        }

        private void LoadingAsset()
        {
            if (ProvideExternalAsset)
            {
                // Asset is provided from somewhere else, just notify that the asset loading is done
#if WALKIO_FLOWCONTROL
                FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                {
                    Name = "Stage"
                });
#endif
            }
            else
            {
                InternalLoadAsset(
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

        private void RegisterToLoadFlow()
        {
#if WALKIO_FLOWCONTROL

            //
            FlowControl?.AssetLoadingStarted
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - Waypoint - LoadAssetSystem - Construct - Receive AssetLoadingStarted");

                    LoadingAsset();
                })
                .AddTo(_compositeDisposable);
#endif
        }
    }
}
