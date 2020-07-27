namespace JoyBrick.Walkio.Game.Hud.App.Assist
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

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    //
#if WALKIO_FLOWCONTROL
    [GameFlowControl.DoneSettingAssetWait("App")]
#endif
    [DisableAutoCreation]
    public class SetupAssetSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupAssetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        public void Construct()
        {
            _logger.Debug($"Module Assist - Hud - App - SetupAssetSystem - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.AssetSettingStarted
                .Where(x => x.Name.Contains("App"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module Assist - Hud - App - SetupAssetSystem - Construct - Receive SettingAsset");

                    // _canSetup = true;
                    // _doingSetup = true;
                    //
                    // SettingAsset();
                    FlowControl?.FinishIndividualSettingAsset(new GameFlowControl.FlowControlContext
                    {
                        Name = "App",
                        Description = "Hud - App"
                    });
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            _logger.Debug($"Module Assist - Hud - App - SetupAssetSystem - OnCreate");

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
