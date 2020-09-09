namespace JoyBrick.Walkio.Game.Hud.Preparation
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

    using GameLevel = JoyBrick.Walkio.Game.Level;

    //
#if WALKIO_FLOWCONTROL
    [GameFlowControl.DoneSettingAssetWait("Preparation")]
#endif
    [DisableAutoCreation]
    public class SetupAssetSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupAssetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        public GameCreature.ICreatureOverviewProvider CreatureOverviewProvider { get; set; }
        public GameLevel.ILevelOverviewProvider LevelOverviewProvider { get; set; }
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        private void SettingAsset()
        {
            CreatureOverviewProvider.TeamLeaderOverviews.ForEach(x =>
            {
                _logger.Debug($"Module - Hud Preparation - SetupAssetSystem - SettingAsset - team leader {x} set to hud");
            });
            CreatureOverviewProvider.TeamMinionOverviews.ForEach(x =>
            {
                _logger.Debug($"Module - Hud Preparation - SetupAssetSystem - SettingAsset - team minion {x} set to hud");
            });
            
            LevelOverviewProvider.LeveOverviewDetails.ForEach(x =>
            {
                _logger.Debug($"Module - Hud Preparation - SetupAssetSystem - SettingAsset - level {x} set to hud");
            });
        }
        
        public void Construct()
        {
            _logger.Debug($"Module - Hud Preparation - SetupAssetSystem - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.AssetSettingStarted
                .Where(x => x.Name.Contains("Preparation"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Hud Preparation - SetupAssetSystem - Construct - Receive SettingAsset");

                    SettingAsset();

                    FlowControl?.FinishIndividualSettingAsset(new GameFlowControl.FlowControlContext
                    {
                        Name = "Preparation",
                        Description = "Hud - Preparation"
                    });
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            _logger.Debug($"Module - Hud Preparation - SetupAssetSystem - OnCreate");

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
