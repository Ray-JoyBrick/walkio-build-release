namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Linq;
    using FlowControl.Template;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

#if WALKIO_EXTENSION
    using GameExtension = JoyBrick.Walkio.Game.Extension;
#endif

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

#if WALKIO_LEVEL
    using GameLevel = JoyBrick.Walkio.Game.Level;
#endif

    public partial class Bootstrap

#if WALKIO_FLOWCONTROL
        : GameFlowControl.IFlowControl
#endif

    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Flow Control")]
#endif
        public ScriptableObject flowControlData;
        public ScriptableObject FlowControlData => flowControlData;

        //
        private readonly Subject<GameFlowControl.FlowControlContext> _notifyAssetLoadingStarted =
            new Subject<GameFlowControl.FlowControlContext>();
        public IObservable<GameFlowControl.FlowControlContext> AssetLoadingStarted => _notifyAssetLoadingStarted;

        private readonly Subject<GameFlowControl.FlowControlContext> _notifyIndividualAssetLoadingFinished =
            new Subject<GameFlowControl.FlowControlContext>();
        public IObservable<GameFlowControl.FlowControlContext> IndividualAssetLoadingFinished =>
            _notifyIndividualAssetLoadingFinished;

        // private readonly Subject<GameFlowControl.FlowControlContext> _notifyAssetLoadingDone =
        //     new Subject<GameFlowControl.FlowControlContext>();
        // public IObservable<GameFlowControl.FlowControlContext> AssetLoadingDone => _notifyAssetLoadingDone;

        //
        private readonly Subject<GameFlowControl.FlowControlContext> _notifyAssetSettingStarted =
            new Subject<GameFlowControl.FlowControlContext>();
        public IObservable<GameFlowControl.FlowControlContext> AssetSettingStarted => _notifyAssetSettingStarted;

        private readonly Subject<GameFlowControl.FlowControlContext> _notifyIndividualAssetSettingFinished =
            new Subject<GameFlowControl.FlowControlContext>();
        public IObservable<GameFlowControl.FlowControlContext> IndividualAssetSettingFinished =>
            _notifyIndividualAssetSettingFinished;

        // private readonly Subject<GameFlowControl.FlowControlContext> _notifyAssetSettingDone =
        //     new Subject<GameFlowControl.FlowControlContext>();
        // public IObservable<GameFlowControl.FlowControlContext> AssetSettingDone => _notifyAssetSettingDone;

        private readonly Subject<GameFlowControl.FlowControlContext> _notifyFlowReadyToStart =
            new Subject<GameFlowControl.FlowControlContext>();
        public IObservable<GameFlowControl.FlowControlContext> FlowReadyToStart => _notifyFlowReadyToStart;

        //
        public void StartLoadingAsset(GameFlowControl.FlowControlContext context)
        {
            _logger.Debug($"Bootstrap - StartLoadingAsset");

            _notifyAssetLoadingStarted.OnNext(context);
        }

        private void StartLoadingAsset_App()
        {
            StartLoadingAsset(new GameFlowControl.FlowControlContext
            {
                Name = "App",
                AssetName = "Used Flow Data"
            });
        }

        public void FinishIndividualLoadingAsset(GameFlowControl.FlowControlContext context)
        {
            _logger.Debug($"Bootstrap - FinishIndividualLoadingAsset");

            _notifyIndividualAssetLoadingFinished.OnNext(context);
        }

        public void FinishIndividualSettingAsset(GameFlowControl.FlowControlContext context)
        {
            _logger.Debug($"Bootstrap - FinishIndividualSettingAsset");

            _notifyIndividualAssetSettingFinished.OnNext(context);
        }

        //
        public void AllAssetLoadingDone(GameFlowControl.FlowControlContext context)
        {
            _logger.Debug($"Bootstrap - AllAssetLoadingDone");

            _notifyAssetSettingStarted.OnNext(context);
        }

        public void AllAssetSettingDone(GameFlowControl.FlowControlContext context)
        {
            _logger.Debug($"Bootstrap - AllAssetSettingDone");

            //
            var eventName = $"zz_{context.Name} Done Setting";
            GameExtension.BridgeExtension.SendEvent(eventName);

            //
            _notifyFlowReadyToStart.OnNext(context);
        }
    }
}
