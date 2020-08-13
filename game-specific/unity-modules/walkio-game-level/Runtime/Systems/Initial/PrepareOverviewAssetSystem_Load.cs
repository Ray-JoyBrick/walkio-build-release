namespace JoyBrick.Walkio.Game.Level
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
        //
        public bool ProvideExternalAsset { get; set; }

        private async Task<ScriptableObject> Load(string levelAssetName)
        {
            // What to load is defined below with async task
            var levelDataAssetName = levelAssetName;
            var levelDataAssetTask = GameCommon.Utility.AssetLoadingHelper.GetAsset<ScriptableObject>(levelDataAssetName);

            var levelSettingAsset = await levelDataAssetTask;

            return levelSettingAsset;
        }


        private void InternalLoadAsset(
            string levelAssetName,
            System.Action loadingDoneAction)
        {
            Load(levelAssetName).ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    var levelOverviewDataAsset = result;
                    var levelOverviewData = levelOverviewDataAsset as Template.LevelOverviewData;
                    if (levelOverviewData != null)
                    {
                        levelOverviewData.levelOverviewDetails.ForEach(x =>
                        {
                            LevelOverviewProvider.AddLevelOverviewDetail(x);
                        });
                    }
                    
                    loadingDoneAction();
                })
                .AddTo(_compositeDisposable);
        }

        private void LoadingAsset(string levelAssetName)
        {
            if (ProvideExternalAsset)
            {
                // Asset is provided from somewhere else, just notify that the asset loading is done
#if WALKIO_FLOWCONTROL
                FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                {
                    Name = AtPart
                });
#endif
            }
            else
            {
                InternalLoadAsset(
                    levelAssetName,
                    () =>
                    {
                        // Since internal loading might be very time consuming, after it is finished, it will
                        // send an event entity. This event entity is caught in Update and process further.

                        FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                        {
                            Name = AtPart
                        });
                    });
            }
        }

        //
        private void RegisterToLoadFlow()
        {
            _logger.Debug($"Module - Level - PrepareOverviewAssetSystem - RegisterToLoadFlow");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.AssetLoadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Level - PrepareOverviewAssetSystem - RegisterToLoadFlow - Receive AssetLoadingStarted");

                    var levelAssetName = $"Level Overview Data";
                    LoadingAsset(levelAssetName);
                })
                .AddTo(_compositeDisposable);
#endif
        }
    }
}
