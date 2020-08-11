namespace JoyBrick.Walkio.Game.Level
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Cinemachine;
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
        public ILevelPropProvider LevelPropProvider { get; set; }

        private void SpawnTeamLeaderNpc(
            Template.LevelData levelData,
            GameCreature.ICreatureProvider creatureProvider)
        {
            var teamLeaderNpcCount = levelData.teamLeaderNpcSpawnCount;
            // creatureProvider.
            for (var i = 0; i < teamLeaderNpcCount; ++i)
            {
                var location = levelData.teamLeaderNpcSpawnLocations[i];

                var adjustedLocation = new Vector3(location.x + 0.5f, 0, location.y + 0.5f);

                creatureProvider.CreateTeamLeaderNpcAt(adjustedLocation);
            }
        }

        private void SpawnTeamLeaderPlayer(
            Template.LevelData levelData,
            GameCreature.ICreatureProvider creatureProvider)
        {
            // Suppose there is only one for now

            var randomIndex = UnityEngine.Random.Range(0, levelData.teamLeaderPlayerSpawnLocations.Count);
            var location = levelData.teamLeaderPlayerSpawnLocations[randomIndex];

            var adjustedLocation = new Vector3(location.x + 0.5f, 0, location.y + 0.5f);

            creatureProvider.CreateTeamLeaderPlayerAt(adjustedLocation);
        }

        private void SetupPlayerFollowingCamera(
            ILevelPropProvider levelPropProvider,
            GameCreature.ICreatureProvider creatureProvider)
        {
            var player = creatureProvider.GetCurrentPlayer();
            levelPropProvider.SetupFollowingCamera(player);
        }

        private void SetupPlayerFollowingVirtualCamera(
            ILevelPropProvider levelPropProvider,
            GameCreature.ICreatureProvider creatureProvider)
        {
            if (levelPropProvider.MainPlayerVirtualCamera != null)
            {
                var mainPlayerVirtualCamera = levelPropProvider.MainPlayerVirtualCamera.GetComponent<CinemachineVirtualCamera>();

                if (mainPlayerVirtualCamera != null)
                {
                    var player = creatureProvider.GetCurrentPlayer();

                    if (player != null)
                    {
                        mainPlayerVirtualCamera.Follow = player.transform;
                        mainPlayerVirtualCamera.LookAt = player.transform;
                    }
                    else
                    {
                        _logger.Error($"Module - Level - SetupAssetSystem - SetupPlayerFollowingVirtualCamera - player is null");
                    }
                }
                else
                {
                    _logger.Error($"Module - Level - SetupAssetSystem - SetupPlayerFollowingVirtualCamera - mainPlayerVirtualCamera is null");

                }
            }
            else
            {
                _logger.Error($"Module - Level - SetupAssetSystem - SetupPlayerFollowingVirtualCamera - levelPropProvider.MainPlayerVirtualCamera is null");

            }
        }

        private async Task Setup()
        {
            _logger.Debug($"Module - Level - SetupAssetSystem - Setup");
            var prepareAssetSsytem = World.GetExistingSystem<PrepareAssetSystem>();

            // Spawn first
            SpawnTeamLeaderNpc(prepareAssetSsytem.LevelData, CreatureProvider);
            SpawnTeamLeaderPlayer(prepareAssetSsytem.LevelData, CreatureProvider);

            // Assign the value to camera, etc.
            SetupPlayerFollowingCamera(LevelPropProvider, CreatureProvider);
            SetupPlayerFollowingVirtualCamera(LevelPropProvider, CreatureProvider);

            await Task.Delay(System.TimeSpan.FromMilliseconds(2000));
        }

        private void SettingAsset()
        {
            //
            Setup().ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
#if WALKIO_FLOWCONTROL
                    FlowControl?.FinishIndividualSettingAsset(new GameFlowControl.FlowControlContext
                    {
                        Name = "Stage",
                        Description = "Module - Level - SetupAssetSystem"
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
