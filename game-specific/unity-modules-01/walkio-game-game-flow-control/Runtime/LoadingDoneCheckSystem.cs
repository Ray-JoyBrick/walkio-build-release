namespace JoyBrick.Walkio.Game.GameFlowControl
{
    using System;
    using Command;
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

        // Should separate into
        // Flow state
        // Command
        public GameCommon.IGameSettingProvider GameSettingProvider { get; set; }
        public ICommandService CommandService { get; set; }
        
        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            FlowControl?.DoneLoadingAsset
                .Where(x => x.Name.Contains("App"))
                .Buffer(GameSettingProvider.GameSettings.doneLoadingAssetWaitForApp)
                .Subscribe(x =>
                {
                    _logger.Debug($"LoadingDoneCheckSystem - Construct - DoneLoadingAsset for App");
                    //
                    FlowControl.StartSetting(new GameCommon.FlowControlContext
                    {
                        Name = "App"
                    });
                })
                .AddTo(_compositeDisposable);

            FlowControl?.DoneLoadingAsset
                .Where(x => x.Name.Contains("Preparation"))
                .Buffer(GameSettingProvider.GameSettings.doneLoadingAssetWaitForPreparation)
                .Subscribe(x =>
                {
                    _logger.Debug($"LoadingDoneCheckSystem - Construct - DoneLoadingAsset for Preparation");

                    //
                    FlowControl.StartSetting(new GameCommon.FlowControlContext
                    {
                        Name = "Preparation"
                    });
                })
                .AddTo(_compositeDisposable);
            
            FlowControl?.DoneLoadingAsset
                .Where(x => x.Name.Contains("Stage"))
                // Should be loading from some settings
                .Buffer(GameSettingProvider.GameSettings.doneLoadingAssetWaitForStage)
                .Subscribe(x =>
                {
                    _logger.Debug($"LoadingDoneCheckSystem - Construct - DoneLoadingAsset for Stage");

                    //
                    FlowControl.StartSetting(new GameCommon.FlowControlContext
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
