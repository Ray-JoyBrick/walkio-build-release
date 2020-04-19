namespace JoyBrick.Walkio.Game.GameFlowControl
{
    using System;
    using Command;
    using UniRx;
    using Unity.Entities;

    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameExtension = JoyBrick.Walkio.Game.Extension;

    [DisableAutoCreation]
    public class SettingDoneCheckSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SettingDoneCheckSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        // Should separate into
        // Flow state
        // Command
        public ICommandService CommandService { get; set; }
        
        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            FlowControl.DoneSettingAsset
                .Where(x => x.Name.Contains("App"))
                .Buffer(1)
                .Subscribe(x =>
                {
                    _logger.Debug($"SettingDoneCheckSystem - Construct - Receive DoneSettingAsset");

                    GameExtension.BridgeExtension.SendEvent("Enter Preparation");

                    //
                    FlowControl.FinishSetting(new GameCommon.FlowControlContext
                    {
                        Name = "App"
                    });
                })
                .AddTo(_compositeDisposable);

            FlowControl.DoneSettingAsset
                .Where(x => x.Name.Contains("Preparation"))
                .Buffer(1)
                .Subscribe(x =>
                {
                    //
                    FlowControl.FinishSetting(new GameCommon.FlowControlContext
                    {
                        Name = "Preparation"
                    });
                })
                .AddTo(_compositeDisposable);
            
            FlowControl.DoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                // Should be loading from some settings
                .Buffer(2)
                .Subscribe(x =>
                {
                    //
                    FlowControl.FinishSetting(new GameCommon.FlowControlContext
                    {
                        Name = "Stage"
                    });
                })
                .AddTo(_compositeDisposable);            
        }

        protected override void OnUpdate() {}
    }
}
