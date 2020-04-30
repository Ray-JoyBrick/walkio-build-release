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

        public IObservable<GameCommon.FlowControlContext> AllDoneSettingAsset =>
            _notifyAllDoneSettingAsset.AsObservable();
        private readonly Subject<GameCommon.FlowControlContext> _notifyAllDoneSettingAsset =
            new Subject<GameCommon.FlowControlContext>();

        public IObservable<GameCommon.FlowControlContext> CleaningAsset => _notifyCleaningAsset.AsObservable();
        private readonly Subject<GameCommon.FlowControlContext> _notifyCleaningAsset =
            new Subject<GameCommon.FlowControlContext>();

        public void StartLoadingAsset(string flowName)
        {
            _notifyLoadingAsset.OnNext(new GameCommon.FlowControlContext
            {
                Name = flowName
            });            
        }

        public void FinishLoadingAsset(GameCommon.FlowControlContext context)
        {
            _notifyDoneLoadingAsset.OnNext(context);
        }

        public void StartSetting(GameCommon.FlowControlContext context)
        {
            _notifySettingAsset.OnNext(context);
        }

        public void FinishSetting(GameCommon.FlowControlContext context)
        {
            _notifyDoneSettingAsset.OnNext(context);
        }

        public void FinishAllSetting(GameCommon.FlowControlContext context)
        {
            _notifyAllDoneSettingAsset.OnNext(context);
        }

        public void StartCleaningAsset(string flowName)
        {
            _notifyCleaningAsset.OnNext(new GameCommon.FlowControlContext
            {
                Name = flowName
            });
        }
    }
}
