namespace JoyBrick.Walkio.Game.Creature
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
    // using GameCommand = JoyBrick.Walkio.Game.Command;

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    public partial class PrepareOverviewAssetSystem
    {
        private async Task Unload()
        {
        }

        private void InternalUnloadAsset(
            System.Action unloadingDoneAction)
        {
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
                // Asset is provided from somewhere else, just notify that the asset unloading is done
#if WALKIO_FLOWCONTROL
                FlowControl?.FinishIndividualUnloadingAsset(new GameFlowControl.FlowControlContext
                {
                    Name = AtPart
                });
#endif
            }
            else
            {
                InternalUnloadAsset(
                    () =>
                    {
#if WALKIO_FLOWCONTROL
                        FlowControl?.FinishIndividualUnloadingAsset(new GameFlowControl.FlowControlContext
                        {
                            Name = AtPart
                        });
#endif
                    });
            }
        }

        //
        private void RegisterToCleanupFlow()
        {
            _logger.Debug($"Module - Creature - PrepareOverviewAssetSystem - RegisterToCleanupFlow");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.AssetUnloadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Creature - PrepareOverviewAssetSystem - RegisterToCleanupFlow - Receive AssetUnloadingStarted");

                    UnloadingAsset();
                })
                .AddTo(_compositeDisposable);

#endif
        }
    }
}
