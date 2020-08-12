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
    using UnityEngine.AddressableAssets;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif


    public partial class PrepareAssetSystem
    {
        private async Task Unload()
        {
            _canvasPrefabs.Clear();
            
            if (_canvasList.Any())
            {
                _canvasList.ForEach(x =>
                {
                    GameObject.Destroy(x);
                });
                
                _canvasList.Clear();
            }

            if (_hudSettingDataAsset != null)
            {
                Addressables.Release(_hudSettingDataAsset);
            }
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
                // Asset is provided from somewhere else, just notify that the asset loading is done
#if WALKIO_FLOWCONTROL
                FlowControl.FinishIndividualUnloadingAsset(new GameFlowControl.FlowControlContext
                {
                    Name = "Stage"
                });
#endif
            }
            else
            {
                InternalUnloadAsset(
                    () =>
                    {
#if WALKIO_FLOWCONTROL
                        FlowControl.FinishIndividualUnloadingAsset(new GameFlowControl.FlowControlContext
                        {
                            Name = "Stage"
                        });
#endif
                    });
            }
        }

        //
        private void RegisterToCleanupFlow()
        {
            _logger.Debug($"Module - Hud Stage - PrepareAssetSystem - RegisterToCleanupFlow");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.AssetUnloadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Hud Stage - PrepareAssetSystem - Construct - Receive AssetUnloadingStarted");

                    UnloadingAsset();
                })
                .AddTo(_compositeDisposable);
#endif
        }
    }
}
