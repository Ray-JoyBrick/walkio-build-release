namespace JoyBrick.Walkio.Game.FlowControl.Preparation
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
    using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameCommon = JoyBrick.Walkio.Game.Common;

#if WALKIO_EXTENSION
    using GameExtension = JoyBrick.Walkio.Game.Extension;
#endif

    public partial class PrepareAssetSystem :
        SystemBase
        // GameCommon.ISystemContext
    {
        private async Task Unload()
        {
        }

        private void InternalUnloadAsset(
            System.Action unloadingDoneAction)
        {
            //
            Unload().ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    unloadingDoneAction();
                })
                .AddTo(_compositeDisposable);
        }

        private void UnloadingAsset()
        {
            if (ProvideExternalAsset)
            {
                // Since the asset is provided, just notify instantly
                FlowControl?.FinishIndividualUnloadingAsset(new FlowControlContext
                {
                    Name = "Preparation"
                });
            }
            else
            {
                // Load internally then notify
                InternalUnloadAsset(
                    () =>
                    {
                        FlowControl?.FinishIndividualUnloadingAsset(new FlowControlContext
                        {
                            Name = "Preparation"
                        });
                    });
            }
        }

        //
        private void RegisterToCleanupFlow()
        {
            _logger.Debug($"Module - Flow Control - Preparation - PrepareAssetSystem - RegisterToCleanupFlow");

            FlowControl?.AssetUnloadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Flow Control - Preparation - PrepareAssetSystem - RegisterToCleanupFlow - Receive AssetLoadingStarted");
                    // var assetName = x.AssetName;
                    UnloadingAsset();
                })
                .AddTo(_compositeDisposable);
        }
    }
}
