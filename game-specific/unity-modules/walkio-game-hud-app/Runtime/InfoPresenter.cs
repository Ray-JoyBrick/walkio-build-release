namespace JoyBrick.Walkio.Game.Hud.App
{
    using System;
    using UniRx;
    using UnityEngine;

    using GameCommand = JoyBrick.Walkio.Game.Command;
    
    public class InfoPresenter :
        MonoBehaviour,
        GameCommand.IInfoPresenter
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(InfoPresenter));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public IObservable<GameCommand.IInfo> InfoStream => Observable.Empty<GameCommand.IInfo>();

        void Start()
        {
            InfoStream
                .Subscribe(x =>
                {
                    //
                })
                .AddTo(_compositeDisposable);
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
