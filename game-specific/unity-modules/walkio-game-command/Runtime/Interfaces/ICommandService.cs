namespace JoyBrick.Walkio.Game.Command
{
    using System;
    using UnityEngine;

    public interface ICommandService
    {
        //
        IObservable<ICommand> CommandStream { get; }

        void AddCommandStreamProducer(ICommandStreamProducer commandStreamProducer);
        void RemoveCommandStreamProducer(ICommandStreamProducer commandStreamProducer);
        
        //
        void AddCommandStreamProducer(GameObject inGO);
        
        //
        IObservable<IInfo> InfoStream { get; }
        void AddInfoStreamPresenter(IInfoPresenter infoPresenter);
        void RemoveInfoStreamPresenter(IInfoPresenter infoPresenter);

        void AddInfoStreamPresenter(GameObject inGO);

        //
        void SendCommand(string commandName);        
    }
}
