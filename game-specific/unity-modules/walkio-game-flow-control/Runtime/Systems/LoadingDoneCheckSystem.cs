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
            var flowControlData = FlowControl.FlowControlData as Template.FlowControlData;
            if (flowControlData == null) return;

            _logger.Debug($"Module - LoadingDoneCheckSystem - Construct");

            // The main purpose is to count loading finished event of each individual asset,
            // If it reaches the total, just issues Start Asset Setting

            FlowControl?.IndividualAssetLoadingFinished
                .Where(x => x.Name.Contains("App"))
                .Buffer(flowControlData.doneLoadingAssetWaitForApp)
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
                .Buffer(flowControlData.doneLoadingAssetWaitForPreparation)
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
                .Buffer(flowControlData.doneLoadingAssetWaitForStage)
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
