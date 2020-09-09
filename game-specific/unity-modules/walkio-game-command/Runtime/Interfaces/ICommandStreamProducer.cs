namespace JoyBrick.Walkio.Game.Command
{
    using System;
    using UniRx;

    public interface ICommandStreamProducer
    {
        IObservable<ICommand> CommandStream { get; }
    }
}
