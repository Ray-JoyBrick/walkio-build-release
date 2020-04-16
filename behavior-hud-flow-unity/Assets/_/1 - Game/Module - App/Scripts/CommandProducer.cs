namespace JoyBrick.Walkio.Game
{
    using System;
    using UniRx;
    using UnityEngine;

    // Stubbed command
    public class LoadDataCommand : ICommand
    {
        
    }

    public class CommandProducer :
        MonoBehaviour,
        ICommandStreamProducer
    {
        public IObservable<ICommand> CommandStream => _notifyCommandStream.AsObservable();
        private readonly Subject<ICommand> _notifyCommandStream = new Subject<ICommand>();

        public void LoadDataCommand()
        {
            _notifyCommandStream.OnNext(new LoadDataCommand());
        }
    }
}
