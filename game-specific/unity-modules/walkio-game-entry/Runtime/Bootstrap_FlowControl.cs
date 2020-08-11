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
    using GameExtension = JoyBrick.Walkio.Game.Extension;
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
    using GameLevel = JoyBrick.Walkio.Game.Level;

    //
    public partial class Bootstrap
        : GameFlowControl.IFlowControl
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Flow Control")]
#endif
        public ScriptableObject flowControlData;
        public ScriptableObject FlowControlData => flowControlData;

        //
        private readonly Subject<GameFlowControl.FlowControlContext> _notifyAssetLoadingStarted =
            new Subject<GameFlowControl.FlowControlContext>();
        public IObservable<GameFlowControl.FlowControlContext> AssetLoadingStarted =>
            _notifyAssetLoadingStarted.AsObservable();

        private readonly Subject<GameFlowControl.FlowControlContext> _notifyIndividualAssetLoadingFinished =
            new Subject<GameFlowControl.FlowControlContext>();
        public IObservable<GameFlowControl.FlowControlContext> IndividualAssetLoadingFinished =>
            _notifyIndividualAssetLoadingFinished.AsObservable();

        // private readonly Subject<GameFlowControl.FlowControlContext> _notifyAssetLoadingDone =
        //     new Subject<GameFlowControl.FlowControlContext>();
        // public IObservable<GameFlowControl.FlowControlContext> AssetLoadingDone => _notifyAssetLoadingDone;

        //
        private readonly Subject<GameFlowControl.FlowControlContext> _notifyAssetSettingStarted =
            new Subject<GameFlowControl.FlowControlContext>();
        public IObservable<GameFlowControl.FlowControlContext> AssetSettingStarted =>
            _notifyAssetSettingStarted.AsObservable();

        private readonly Subject<GameFlowControl.FlowControlContext> _notifyIndividualAssetSettingFinished =
            new Subject<GameFlowControl.FlowControlContext>();
        public IObservable<GameFlowControl.FlowControlContext> IndividualAssetSettingFinished =>
            _notifyIndividualAssetSettingFinished.AsObservable();

        // private readonly Subject<GameFlowControl.FlowControlContext> _notifyAssetSettingDone =
        //     new Subject<GameFlowControl.FlowControlContext>();
        // public IObservable<GameFlowControl.FlowControlContext> AssetSettingDone => _notifyAssetSettingDone;

        private readonly Subject<GameFlowControl.FlowControlContext> _notifyFlowReadyToStart =
            new Subject<GameFlowControl.FlowControlContext>();
        public IObservable<GameFlowControl.FlowControlContext> FlowReadyToStart =>
            _notifyFlowReadyToStart.AsObservable();

        //
        private readonly Subject<GameFlowControl.FlowControlContext> _notifyAssetUnloadingStarted =
            new Subject<GameFlowControl.FlowControlContext>();
        public IObservable<GameFlowControl.FlowControlContext> AssetUnloadingStarted =>
            _notifyAssetUnloadingStarted.AsObservable();

        private readonly Subject<GameFlowControl.FlowControlContext> _notifyIndividualAssetUnloadingFinished =
            new Subject<GameFlowControl.FlowControlContext>();
        public IObservable<GameFlowControl.FlowControlContext> IndividualAssetUnloadingFinished =>
            _notifyIndividualAssetUnloadingFinished.AsObservable();

        //
        public void StartLoadingAsset(GameFlowControl.FlowControlContext context)
        {
            _logger.Debug($"Bootstrap - StartLoadingAsset - {context}");

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
            _logger.Debug($"Bootstrap - FinishIndividualLoadingAsset - {context}");

            _notifyIndividualAssetLoadingFinished.OnNext(context);
        }

        public void FinishIndividualSettingAsset(GameFlowControl.FlowControlContext context)
        {
            _logger.Debug($"Bootstrap - FinishIndividualSettingAsset - {context}");

            _notifyIndividualAssetSettingFinished.OnNext(context);
        }

        //
        public void AllAssetLoadingDone(GameFlowControl.FlowControlContext context)
        {
            _logger.Debug($"Bootstrap - AllAssetLoadingDone - {context}");

            _notifyAssetSettingStarted.OnNext(context);
        }

        public void AllAssetSettingDone(GameFlowControl.FlowControlContext context)
        {
            _logger.Debug($"Bootstrap - AllAssetSettingDone - {context}");

            //
            var eventName = $"zz_{context.Name} Done Setting";
            SendExtensionEvent(eventName);

            //
            _notifyFlowReadyToStart.OnNext(context);
        }

        public void StartUnloadingAsset(GameFlowControl.FlowControlContext context)
        {
            _logger.Debug($"Bootstrap - StartUnloadingAsset - {context}");

            _notifyAssetUnloadingStarted.OnNext(context);
        }

        public void FinishIndividualUnloadingAsset(GameFlowControl.FlowControlContext context)
        {
            _logger.Debug($"Bootstrap - FinishIndividualUnloadingAsset - {context}");

            _notifyIndividualAssetUnloadingFinished.OnNext(context);
        }
    }
}
