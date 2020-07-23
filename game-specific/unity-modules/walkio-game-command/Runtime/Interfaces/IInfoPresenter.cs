namespace JoyBrick.Walkio.Game.Command
{
    using System;

    public interface IInfoPresenter
    {
        IObservable<IInfo> InfoStream { get; }
    }
}
