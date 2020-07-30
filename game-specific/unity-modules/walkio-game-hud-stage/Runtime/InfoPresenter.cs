namespace JoyBrick.Walkio.Game.Hud.Stage
{
    using System;
    using UniRx;
    using UnityEngine;

    using GameCommand = JoyBrick.Walkio.Game.Command;
    
    public class InfoPresenter :
        MonoBehaviour,
        
        GameCommand.ICommandStreamProducer,
        
        GameCommand.IInfoPresenter
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(InfoPresenter));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        public UnityEngine.UI.Button button;
        
        //
        private readonly Subject<GameCommand.ICommand> _notifyCommandStreamGiven = new Subject<GameCommand.ICommand>();
        public IObservable<GameCommand.ICommand> CommandStream => _notifyCommandStreamGiven.AsObservable();

        //
        private readonly Subject<GameCommand.IInfo> _notifyInfoGiven = new Subject<GameCommand.IInfo>();
        public IObservable<GameCommand.IInfo> InfoStream => _notifyInfoGiven.AsObservable();

        void Start()
        {
            _logger.Debug($"Game - InfoPresenter - Start");
            InfoStream
                .Subscribe(x =>
                {
                    _logger.Debug($"Game - InfoPresenter - Start - some info stream in {x}");
                    //
                    if (x is TeamUnitCountInfo teamUnitCountInfo)
                    {
                        _logger.Debug($"Game - InfoPresenter - Start - team unit count: {teamUnitCountInfo}");
                    }
                })
                .AddTo(_compositeDisposable);

            if (button != null)
            {
                button.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        _logger.Debug($"Game - InfoPresenter - Start - button clicked");
                        //
                        _notifyCommandStreamGiven.OnNext(new GameCommand.TestUseCommand());
                    })
                    .AddTo(_compositeDisposable);
            }
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }

    }
}
