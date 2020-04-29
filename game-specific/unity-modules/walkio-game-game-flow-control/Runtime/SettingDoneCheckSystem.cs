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

                    // This actually hard-code  the starting control flow to Preparation. It is possible that
                    // at the development, there should be an easy way to start with something else for testing.
                    // It would be far better to decide what to do in extension, in the current implementation
                    // it is PlayMaker FSM.
                    // The change might be sending netural event or informative event, such as
                    // "App Done Setting" and let the visual tool side decides what to do.
                    // Same for the exiting event, it could use naming such as
                    // "Exit Current Flow" to let visual tool handle what to do.
                    // This more like event routing design in efficient manner or in workable
                    // manner.
                    // GameExtension.BridgeExtension.SendEvent("Enter Preparation");
                    GameExtension.BridgeExtension.SendEvent("zz_App Done Setting");

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
                .Buffer(4)
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
