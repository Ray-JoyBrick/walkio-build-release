namespace JoyBrick.Walkio.Game.Command
{
    using System;
    using UniRx;
    using UnityEngine;
    
    public class InfoPresenter :
        MonoBehaviour,
        IInfoPresenter
    {
        public IObservable<IInfo> InfoStream => Observable.Empty<IInfo>();
    }
}
