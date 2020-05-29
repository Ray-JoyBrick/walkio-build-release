namespace JoyBrick.Walkio.Game.Hud.Stage
{
    using UniRx;
    using Unity.Entities;

    using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class SetupStageHudSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupStageHudSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        public GameCommon.IFlowControl FlowControl { get; set; }
        
        public void Construct()
        {
            FlowControl.SettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"SetupStageHudSystem - Construct - Receive SettingAsset");
                    
                    FlowControl.FinishSetting(new GameCommon.FlowControlContext
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
