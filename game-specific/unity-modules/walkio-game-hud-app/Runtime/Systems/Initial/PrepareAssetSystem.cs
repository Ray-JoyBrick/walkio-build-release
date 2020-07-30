namespace JoyBrick.Walkio.Game.Hud.App
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
    using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameCommon = JoyBrick.Walkio.Game.Common;

#if WALKIO_EXTENSION
    using GameExtension = JoyBrick.Walkio.Game.Extension;
#endif

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

#if WALKIO_FLOWCONTROL
    [GameFlowControl.DoneLoadingAssetWait("App")]
#endif
    [DisableAutoCreation]
    public partial class PrepareAssetSystem :
        SystemBase
        // GameCommon.ISystemContext
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(PrepareAssetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private ScriptableObject _hudSettingDataAsset;

        private GameObject _canvasPrefab;
        private HudData _hudData;

        private GameObject _canvas;

        //
        public GameCommon.ISceneService SceneService { get; set; }
        public GameCommand.ICommandService CommandService { get; set; }
        public GameExtension.IExtensionService ExtensionService { get; set; }
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        public string AtPart => "App";

        private void ActivateLoadingView(bool flag)
        {
            _logger.Debug($"Module - Hud {AtPart} - PrepareAssetSystem - ActivateLoadingView - flag: {flag}");
            
            // TODO: Rename to follow the system use contract. Starting as "zz_"
            if (flag)
            {
                ExtensionService?.SendExtensionEvent("Activate_Loading_View");
                // GameExtension.BridgeExtension.SendEvent("zz_Activate_Loading_View");
            }
            else
            {
                ExtensionService?.SendExtensionEvent("Deactivate_Loading_View");
                // GameExtension.BridgeExtension.SendEvent("zz_Deactivate_Loading_View");
            }
        }
        
        //
        public void Construct()
        {
            _logger.Debug($"Module - Hud {AtPart} - PrepareAssetSystem - Construct");

            RegisterToLoadFlow();
            RegisterToCleanupFlow();
            
            CommandService.CommandStream
                // .Do(x => _logger.Debug($"Receive Command Stream: {x}"))
                .Where(x => (x as GameCommand.ActivateLoadingViewCommand) != null)
                .Subscribe(x =>
                {
                    _logger.Debug($"LoadAppHudSystem - Construct - Receive ActivateLoadingViewCommand");
                    var activateLoadingViewCommand = (x as GameCommand.ActivateLoadingViewCommand);
                    //
                    ActivateLoadingView(activateLoadingViewCommand.Flag);
                })
                .AddTo(_compositeDisposable);            
        }

        protected override void OnCreate()
        {
            _logger.Debug($"Module - Hud {AtPart} - PrepareAssetSystem - OnCreate");
            base.OnCreate();
        }

        protected override void OnUpdate() { }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable.Dispose();
        }
    }
}
