namespace JoyBrick.Walkio.Game.FlowControl
{
    using System;
    // using Command;
    using UniRx;
    using Unity.Entities;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class LoadingDoneCheckSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadingDoneCheckSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        // // Should separate into
        // // Flow state
        // // Command
        // public GameCommon.IGameSettingProvider GameSettingProvider { get; set; }
        // public ICommandService CommandService { get; set; }

        public IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            // var flowControlData = FlowControl.FlowControlData as Template.FlowControlData;
            // if (flowControlData == null) return;

            var flowControlData = FlowControl.FlowControlData;
           if (flowControlData == null) return;
           
            _logger.Debug($"Module - LoadingDoneCheckSystem - Construct");

            // The main purpose is to count loading finished event of each individual asset,
            // If it reaches the total, just issues Start Asset Setting

            var doneLoadingAssetWaitForAppAll = flowControlData.doneLoadingAssetWaitForApp;
            var doneLoadingAssetWaitForAppExcludeAssist =
                doneLoadingAssetWaitForAppAll - flowControlData.doneLoadingAssetWaitForAppAssist;
            
            var doneLoadingAssetWaitForPreparationAll = flowControlData.doneLoadingAssetWaitForPreparation;
            var doneLoadingAssetWaitForPreparationExcludeAssist =
                doneLoadingAssetWaitForPreparationAll - flowControlData.doneLoadingAssetWaitForPreparationAssist;
            
            var doneLoadingAssetWaitForStageAll = flowControlData.doneLoadingAssetWaitForStage;
            var doneLoadingAssetWaitForStageExcludeAssist =
                doneLoadingAssetWaitForStageAll - flowControlData.doneLoadingAssetWaitForStageAssist;
            
            // Use flags to decide which to use
            var doneLoadingAssetWaitForApp = doneLoadingAssetWaitForAppAll;
            var doneLoadingAssetWaitForPreparation = doneLoadingAssetWaitForPreparationAll;
            var doneLoadingAssetWaitForStage = doneLoadingAssetWaitForStageAll;
            
            // var doneLoadingAssetWaitForApp = doneLoadingAssetWaitForAppExcludeAssist;
            // var doneLoadingAssetWaitForPreparation = doneLoadingAssetWaitForPreparationExcludeAssist;
            // var doneLoadingAssetWaitForStage = doneLoadingAssetWaitForStageExcludeAssist;

            // var doneLoadingAssetWaitForAppAll = 5;
            // var doneLoadingAssetWaitForAppExcludeAssist =
            //     doneLoadingAssetWaitForAppAll - 1;
            //
            // var doneLoadingAssetWaitForPreparationAll = 2;
            // var doneLoadingAssetWaitForPreparationExcludeAssist =
            //     doneLoadingAssetWaitForPreparationAll - 0;
            //
            // var doneLoadingAssetWaitForStageAll = 8;
            // var doneLoadingAssetWaitForStageExcludeAssist =
            //     doneLoadingAssetWaitForStageAll - 1;
            //
            // // Use flags to decide which to use
            // // var doneLoadingAssetWaitForApp = doneLoadingAssetWaitForAppAll;
            // // var doneLoadingAssetWaitForPreparation = doneLoadingAssetWaitForPreparationAll;
            // // var doneLoadingAssetWaitForStage = doneLoadingAssetWaitForStageAll;
            //
            // var doneLoadingAssetWaitForApp = doneLoadingAssetWaitForAppExcludeAssist;
            // var doneLoadingAssetWaitForPreparation = doneLoadingAssetWaitForPreparationExcludeAssist;
            // var doneLoadingAssetWaitForStage = doneLoadingAssetWaitForStageExcludeAssist;

            FlowControl?.IndividualAssetLoadingFinished
                .Where(x => x.Name.Contains("App"))
                .Buffer(doneLoadingAssetWaitForApp)
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - LoadingDoneCheckSystem - Construct - IndividualAssetLoadingFinished for App");
                    //
                    FlowControl?.AllAssetLoadingDone(new FlowControlContext
                    {
                        Name = "App"
                    });
                })
                .AddTo(_compositeDisposable);

            FlowControl?.IndividualAssetLoadingFinished
                .Where(x => x.Name.Contains("Preparation"))
                .Buffer(doneLoadingAssetWaitForPreparation)
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - LoadingDoneCheckSystem - Construct - IndividualAssetLoadingFinished for Preparation");

                    //
                    FlowControl?.AllAssetLoadingDone(new FlowControlContext
                    {
                        Name = "Preparation"
                    });
                })
                .AddTo(_compositeDisposable);

            FlowControl?.IndividualAssetLoadingFinished
                .Where(x => x.Name.Contains("Stage"))
                // Should be loading from some settings
                .Buffer(doneLoadingAssetWaitForStage)
                // .Buffer(2)
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - LoadingDoneCheckSystem - Construct - IndividualAssetLoadingFinished for Stage");

                    //
                    FlowControl?.AllAssetLoadingDone(new FlowControlContext
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
