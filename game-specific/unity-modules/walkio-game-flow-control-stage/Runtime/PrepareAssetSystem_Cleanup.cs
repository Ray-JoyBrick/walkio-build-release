namespace JoyBrick.Walkio.Game.FlowControl.Stage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

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
            if (_flowInstances.Any())
            {
                _flowInstances.ForEach(x => GameObject.Destroy(x));
                _flowInstances.Clear();
            }

            if (_assetData != null)
            {
                Addressables.Release(_assetData);
            }
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
                    Name = AtPart
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
                            Name = AtPart
                        });
                    });
            }
        }

        //
        private void RegisterToCleanupFlow()
        {
            _logger.Debug($"Module - Flow Control - Stage - PrepareAssetSystem - RegisterToCleanupFlow");

            FlowControl?.AssetUnloadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Flow Control - Stage - PrepareAssetSystem - RegisterToCleanupFlow - Receive AssetLoadingStarted");
                    // var assetName = x.AssetName;
                    UnloadingAsset();
                })
                .AddTo(_compositeDisposable);
        }
    }
}
