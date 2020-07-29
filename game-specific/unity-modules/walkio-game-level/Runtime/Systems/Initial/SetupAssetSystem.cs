namespace JoyBrick.Walkio.Game.Level
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
    using GameCreature = JoyBrick.Walkio.Game.Creature;

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    //
#if WALKIO_FLOWCONTROL
    [GameFlowControl.DoneSettingAssetWait("Stage")]
#endif
    [DisableAutoCreation]
    public class SetupAssetSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupAssetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        public GameCreature.ICreatureProvider CreatureProvider { get; set; }
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        private void SpawnTeamLeaderNpc(
            Template.LevelData levelData,
            GameCreature.ICreatureProvider creatureProvider)
        {
            var teamLeaderNpcCount = levelData.teamLeaderNpcSpawnCount;
            // creatureProvider.
            for (var i = 0; i < teamLeaderNpcCount; ++i)
            {
                var index = levelData.teamLeaderNpcSpawnLocations[i];

                var location = new Vector3(index.x + 0.5f, 0, index.y + 0.5f);

                creatureProvider.CreateTeamLeaderNpcAt(location);
            }
        }

        private async Task Setup()
        {
            _logger.Debug($"Module - Level - SetupAssetSystem - Setup");
            var prepareAssetSsytem = World.GetExistingSystem<PrepareAssetSystem>();

            //
            SpawnTeamLeaderNpc(prepareAssetSsytem.LevelData, CreatureProvider);

        }

        private void SettingAsset()
        {
            //
            Setup().ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    // _canSetup = false;

#if WALKIO_FLOWCONTROL
                    FlowControl?.FinishIndividualSettingAsset(new GameFlowControl.FlowControlContext
                    {
                        Name = "Stage"
                    });
#endif
                })
                .AddTo(_compositeDisposable);
        }

        public void Construct()
        {
            _logger.Debug($"Module - Level - SetupAssetSystem - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.AssetSettingStarted
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Level - SetupAssetSystem - Construct - Receive SettingAsset");

                    // _canSetup = true;
                    // _doingSetup = true;


                    // At this time, creature data should be loaded, can create on-level creature
                    SettingAsset();
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            _logger.Debug($"Module - Level - SetupAssetSystem - OnCreate");

            base.OnCreate();
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
