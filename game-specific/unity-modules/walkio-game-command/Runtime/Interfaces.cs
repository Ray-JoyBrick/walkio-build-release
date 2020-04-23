namespace JoyBrick.Walkio.Game.Command
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public interface ICommand
    {
    }

    public interface ICommandStreamProducer
    {
        IObservable<ICommand> CommandStream { get; }
    }

    public interface IInfo
    {
        
    }

    public interface IInfoPresenter
    {
        IObservable<IInfo> InfoStream { get; }
    }

    public class ServiceCommandContext
    {
    }

    public interface ICategoryServiceCommand
    {
        //
        IObservable<ServiceCommandContext> InitializingService { get; }
        void StartInitializingService();
        
        void StartLoadingSubService();
        void FinishLoadingSubService();
        
        IObservable<ServiceCommandContext> DoneInitializingService { get; }
        void FinishInitializingService();
        
        IObservable<ServiceCommandContext> SettingService { get; }
        void StartSettingService();
        IObservable<ServiceCommandContext> DoneSettingService { get; }
        void FinishSettingService();
    }
    
    public interface ICommandService
    {
        //
        IObservable<ICommand> CommandStream { get; }

        void AddCommandStreamProducer(ICommandStreamProducer commandStreamProducer);
        
        IObservable<IInfo> InfoStream { get; }
        void AddInfoStreamPresenter(IInfoPresenter infoPresenter);

        void SendCommand(string commandName);
    }
}
