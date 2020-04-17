namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;

    using GameCommon = JoyBrick.Walkio.Game.Common;

    public partial class Bootstrap :
        GameCommon.IFlowControl
    {
        public IObservable<GameCommon.FlowControlContext> LoadingAsset => _notifyLoadingAsset.AsObservable();
        private readonly Subject<GameCommon.FlowControlContext> _notifyLoadingAsset =
            new Subject<GameCommon.FlowControlContext>();

        public IObservable<GameCommon.FlowControlContext> DoneLoadingAsset => _notifyDoneLoadingAsset.AsObservable();
        private readonly Subject<GameCommon.FlowControlContext> _notifyDoneLoadingAsset =
            new Subject<GameCommon.FlowControlContext>();

        public IObservable<GameCommon.FlowControlContext> SettingAsset => _notifySettingAsset.AsObservable();
        private readonly Subject<GameCommon.FlowControlContext> _notifySettingAsset =
            new Subject<GameCommon.FlowControlContext>();

        public IObservable<GameCommon.FlowControlContext> DoneSettingAsset => _notifyDoneSettingAsset.AsObservable();
        private readonly Subject<GameCommon.FlowControlContext> _notifyDoneSettingAsset =
            new Subject<GameCommon.FlowControlContext>();
        
        public void StartSetting(GameCommon.FlowControlContext context)
        {
            _notifySettingAsset.OnNext(context);
        }
    }
}
