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
        //
        public bool ProvideExternalAsset { get; set; }

        private async Task<ScriptableObject> Load()
        //private async Task<ScriptableObject> Load(string levelAssetName, string specificLevelName)
        {
            _logger.Debug($"Module - Creature - PrepareOverviewAssetSystem - Load");

            var creatureOverviewRepoDataAssetTask = GameCommon.Utility.AssetLoadingHelper.GetAsset<ScriptableObject>($"Creature Overview Repo Data");

            var creatureOverviewRepoDataAsset = await creatureOverviewRepoDataAssetTask;

            return creatureOverviewRepoDataAsset;
        }

        private void InternalLoadAsset(
            System.Action loadingDoneAction)
        {
            Load().ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    //
                    _creatureOverviewRepoDataAsset = result;

                    var creatureOverviewRepoData = _creatureOverviewRepoDataAsset as Template.CreatureOverviewRepoData;

                    if (creatureOverviewRepoData != null)
                    {
                        creatureOverviewRepoData.teamLeaderOverviews.ForEach(x =>
                        {
                            CreatureOverviewProvider.AddTeamLeaderOverview(x);
                        });

                        creatureOverviewRepoData.teamMinionOverviews.ForEach(x =>
                        {
                            CreatureOverviewProvider.AddTeamMinionOverview(x);
                        });
                    }

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
                FlowControl?.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                {
                    Name = AtPart,
                    Description = "Creature Overview"
                });
#endif
            }
            else
            {
                InternalLoadAsset(
                    () =>
                    {
                        // Since internal loading might be very time consuming, after it is finished, it will
                        // send an event entity. This event entity is caught in Update and process further.

                        FlowControl?.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                        {
                            Name = AtPart,
                            Description = "Creature Overview"
                        });
                    });
            }
        }

        //
        private void RegisterToLoadFlow()
        {
            _logger.Debug($"Module - Creature - PrepareOverviewAssetSystem - RegisterToLoadFlow");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.AssetLoadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Creature - PrepareOverviewAssetSystem - RegisterToLoadFlow - Receive AssetLoadingStarted");

                    // Hard code here, should be given in event
                    // var levelAssetName = $"Level Setting.asset";
                    // var specificLevelName = $"Level 001";
                    LoadingAsset();
                })
                .AddTo(_compositeDisposable);
#endif
        }

    }
}
