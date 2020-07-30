namespace JoyBrick.Walkio.Game.Hud.Stage.Assist
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
        public UnityEngine.UI.Button hideButton;
        public UnityEngine.UI.Button showButton;
        
        //
        private readonly Subject<GameCommand.ICommand> _notifyCommandStreamGiven = new Subject<GameCommand.ICommand>();
        public IObservable<GameCommand.ICommand> CommandStream => _notifyCommandStreamGiven.AsObservable();

        //
        private readonly Subject<GameCommand.IInfo> _notifyInfoGiven = new Subject<GameCommand.IInfo>();
        public IObservable<GameCommand.IInfo> InfoStream => _notifyInfoGiven.AsObservable();

        void Start()
        {
            _logger.Debug($"Game Assist - InfoPresenter - Start");
            // InfoStream
            //     .Subscribe(x =>
            //     {
            //         _logger.Debug($"Game - InfoPresenter - Start - some info stream in {x}");
            //         //
            //         if (x is TeamUnitCountInfo teamUnitCountInfo)
            //         {
            //             _logger.Debug($"Game - InfoPresenter - Start - team unit count: {teamUnitCountInfo}");
            //         }
            //     })
            //     .AddTo(_compositeDisposable);

            if (hideButton != null)
            {
                hideButton.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        _logger.Debug($"Game Assist - InfoPresenter - Start - hideButton clicked");
                        //
                        _notifyCommandStreamGiven.OnNext(new GameCommand.ShowHideSceneObjectCommand
                        {
                            Category = 1,
                            Hide = true
                        });
                    })
                    .AddTo(_compositeDisposable);
            }
            
            if (showButton != null)
            {
                showButton.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        _logger.Debug($"Game Assist - InfoPresenter - Start - showButton clicked");
                        //
                        _notifyCommandStreamGiven.OnNext(new GameCommand.ShowHideSceneObjectCommand
                        {
                            Category = 1,
                            Hide = false
                        });
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
