namespace JoyBrick.Walkio.Game
{
    using System;
    using UniRx;
    using UnityEngine;
    
    using GameCommand = JoyBrick.Walkio.Game.Command;

    public class InfoPresenter :
        MonoBehaviour,
        GameCommand.IInfoPresenter
    {
        public IObservable<GameCommand.IInfo> InfoStream => Observable.Empty<GameCommand.IInfo>();
    }
}
