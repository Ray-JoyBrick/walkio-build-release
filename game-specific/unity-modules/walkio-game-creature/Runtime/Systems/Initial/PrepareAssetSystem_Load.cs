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

    public partial class PrepareAssetSystem
        // GameCommon.ISystemContext
    {
        //
        public bool ProvideExternalAsset { get; set; }

        private async Task<ScriptableObject> Load()
        //private async Task<ScriptableObject> Load(string levelAssetName, string specificLevelName)
        {
            _logger.Debug($"Module - Creature - PrepareAssetSystem - Load");

            var creatureRepoDataAssetTask = GameCommon.Utility.AssetLoadingHelper.GetAsset<ScriptableObject>($"Creature Repo Data");

            var creatureRepoDataAsset = await creatureRepoDataAssetTask;

            return creatureRepoDataAsset;
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
                    _creatureRepoDataAsset = result;

                    var creatureRepoData = _creatureRepoDataAsset as Template.CreatureRepoData;
                    
                    //
                    creatureRepoData.teamLeaderNpcAssets.ForEach(x =>
                    {
                        var creatureData = x as Template.CreatureData;
                        CreatureProvider.AddTeamLeaderNpcPrefab(creatureData.avatarPrefab);
                    });
                    
                    //
                    creatureRepoData.teamMinionAssets.ForEach(x =>
                    {
                        var creatureData = x as Template.CreatureData;
                        CreatureProvider.AddTeamMinionPrefab(creatureData.avatarPrefab);
                        CreatureProvider.AddTeamMinionData(new MinionData
                        {
                            material = creatureData.material,
                            mesh = creatureData.mesh
                        });
                    });

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
                        // Since internal loading might be very time consuming, after it is finished, it will
                        // send an event entity. This event entity is caught in Update and process further.

                        FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                        {
                            Name = "Stage"
                        });
                    });
            }
        }

        //
        private void RegisterToLoadFlow()
        {
            _logger.Debug($"Module - Creature - PrepareAssetSystem - RegisterToLoadFlow");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.AssetLoadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Creature - LoadAssetSystem - Construct - Receive AssetLoadingStarted");

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
