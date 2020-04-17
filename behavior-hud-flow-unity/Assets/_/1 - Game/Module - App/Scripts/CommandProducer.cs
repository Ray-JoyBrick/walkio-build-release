namespace JoyBrick.Walkio.Game
{
    using System;
    using UniRx;
    using UnityEngine;
    
    using GameCommand = JoyBrick.Walkio.Game.Command;

    // Stubbed command
    public class LoadDataCommand : GameCommand.ICommand
    {
        
    }

    public class CommandProducer :
        MonoBehaviour,
        GameCommand.ICommandStreamProducer
    {
        public IObservable<GameCommand.ICommand> CommandStream => _notifyCommandStream.AsObservable();
        private readonly Subject<GameCommand.ICommand> _notifyCommandStream = new Subject<GameCommand.ICommand>();

        public void LoadDataCommand()
        {
            _notifyCommandStream.OnNext(new LoadDataCommand());
        }
    }
}
