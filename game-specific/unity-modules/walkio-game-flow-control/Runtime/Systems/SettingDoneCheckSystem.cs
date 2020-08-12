namespace JoyBrick.Walkio.Game.FlowControl
{
    using System;
    // using Command;
    using UniRx;
    using Unity.Entities;

    using GameCommon = JoyBrick.Walkio.Game.Common;
    // using GameExtension = JoyBrick.Walkio.Game.Extension;

    [DisableAutoCreation]
    public class SettingDoneCheckSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SettingDoneCheckSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        // Should separate into
        // Flow state
        // Command
        // public GameCommon.IGameSettingProvider GameSettingProvider { get; set; }
        // public ICommandService CommandService { get; set; }

        public IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            var flowControlData = FlowControl.FlowControlData as Template.FlowControlData;
            if (flowControlData == null) return;

            _logger.Debug($"Module - SettingDoneCheckSystem - Construct");

            var doneSettingAssetWaitForAppAll = flowControlData.doneSettingAssetWaitForApp;
            var doneSettingAssetWaitForAppExcludeAssist =
                doneSettingAssetWaitForAppAll - flowControlData.doneSettingAssetWaitForAppAssist;

            var doneSettingAssetWaitForPreparationAll = flowControlData.doneSettingAssetWaitForPreparation;
            var doneSettingAssetWaitForPreparationExcludeAssist =
                doneSettingAssetWaitForPreparationAll - flowControlData.doneSettingAssetWaitForPreparationAssist;

            var doneSettingAssetWaitForStageAll = flowControlData.doneSettingAssetWaitForStage;
            var doneSettingAssetWaitForStageExcludeAssist =
                doneSettingAssetWaitForStageAll - flowControlData.doneSettingAssetWaitForStageAssist;

            var doneSettingAssetWaitForApp = doneSettingAssetWaitForAppAll;
            var doneSettingAssetWaitForPreparation = doneSettingAssetWaitForPreparationAll;
            var doneSettingAssetWaitForStage = doneSettingAssetWaitForStageAll;

            FlowControl?.IndividualAssetSettingFinished
                .Where(x => x.Name.Contains("App"))
                .Buffer(doneSettingAssetWaitForApp)
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - SettingDoneCheckSystem - Construct - IndividualAssetSettingFinished for App");

                    // // This actually hard-code  the starting control flow to Preparation. It is possible that
                    // // at the development, there should be an easy way to start with something else for testing.
                    // // It would be far better to decide what to do in extension, in the current implementation
                    // // it is PlayMaker FSM.
                    // // The change might be sending netural event or informative event, such as
                    // // "App Done Setting" and let the visual tool side decides what to do.
                    // // Same for the exiting event, it could use naming such as
                    // // "Exit Current Flow" to let visual tool handle what to do.
                    // // This more like event routing design in efficient manner or in workable
                    // // manner.
                    // // GameExtension.BridgeExtension.SendEvent("Enter Preparation");
                    // GameExtension.BridgeExtension.SendEvent("zz_App Done Setting");

                    //
                    FlowControl?.AllAssetSettingDone(new FlowControlContext
                    {
                        Name = "App"
                    });
                })
                .AddTo(_compositeDisposable);

            FlowControl?.IndividualAssetSettingFinished
                .Where(x => x.Name.Contains("Preparation"))
                .Buffer(doneSettingAssetWaitForPreparation)
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - SettingDoneCheckSystem - Construct - IndividualAssetSettingFinished for Preparation");
                    //
                    FlowControl?.AllAssetSettingDone(new FlowControlContext
                    {
                        Name = "Preparation"
                    });
                })
                .AddTo(_compositeDisposable);

            FlowControl?.IndividualAssetSettingFinished
                .Where(x => x.Name.Contains("Stage"))
                // Should be loading from some settings
                .Buffer(doneSettingAssetWaitForStage)
                // .Buffer(2)
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - SettingDoneCheckSystem - Construct - IndividualAssetSettingFinished for Stage");

                    //
                    FlowControl?.AllAssetSettingDone(new FlowControlContext
                    {
                        Name = "Stage"
                    });
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnUpdate() {}

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
