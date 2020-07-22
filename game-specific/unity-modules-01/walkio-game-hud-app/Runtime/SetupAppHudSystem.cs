namespace JoyBrick.Walkio.Game.Hud.App
{
    using UniRx;
    using Unity.Entities;

    using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class SetupAppHudSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupAppHudSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        public GameCommon.IFlowControl FlowControl { get; set; }
        
        public void Construct()
        {
            FlowControl.SettingAsset
                .Where(x => x.Name.Contains("App"))
                .Subscribe(x =>
                {
                    _logger.Debug($"SetupAppHudSystem - Construct - Receive SettingAsset");
                    
                    FlowControl.FinishSetting(new GameCommon.FlowControlContext
                    {
                        Name = "App"
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
