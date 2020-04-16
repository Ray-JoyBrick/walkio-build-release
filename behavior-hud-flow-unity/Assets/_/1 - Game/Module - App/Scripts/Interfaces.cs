namespace JoyBrick.Walkio.Game
{
    using System;

    public interface ICommand
    {
        
    }

    public class ActivateLoadingViewCommand : ICommand
    {
        public bool flag;
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
    
    public interface ICommandService
    {
        //
        IObservable<ICommand> CommandStream { get; }

        void AddCommandStreamProducer(ICommandStreamProducer commandStreamProducer);
        
        IObservable<IInfo> InfoStream { get; }
        void AddInfoStreamPresenter(IInfoPresenter infoPresenter);

        // Appwide

        IObservable<int> InitializingAppwideService { get; }

        void StartInitializingAppwideService();
        
        //
        IObservable<int> LoadingAppHud { get; }

        void StartLoadingAppHud();
        void FinishLoadingAppHud();
        
        IObservable<int> SettingAppwideService { get; }

        void StartSettingAppwideService();

        IObservable<int> DoneSettingAppwideService { get; }

        IObservable<int> DoneLoadingAppHud { get; }

        void FinishSetupAppwideService();
        
        //
        void ActivateViewLoading(bool flag);
        
        // Preparationwide
        IObservable<int> InitializingPreparationwideService { get; }

        IObservable<int> LoadingPreparationHud { get; }
        
        IObservable<int> DoneLoadingPreparationHud { get; }
        
        void StartLoadingPreparationHud();
        void FinishLoadingPreparationHud();

        IObservable<int> SettingPreparationwideService { get; }
        void StartSettingPreparationService();

        IObservable<int> DoneSettingPreparationwideService { get; }

        void FinishSetupPreparationwideService();
        
        IObservable<int> CleaningPreparationwideService { get; }

        // Stagewide

        IObservable<int> InitializingStagewideService { get; }

        void StartInitializingStagewideService();
        
        IObservable<int> LoadingStageHud { get; }
        
        IObservable<int> DoneLoadingStageHud { get; }
        
        void StartLoadingStageHud();
        void FinishLoadingStageHud();

        IObservable<int> LoadingStageEnvironment { get; }
        
        IObservable<int> DoneLoadingStageEnvironment { get; }

        void StartLoadingStageEnvironment();
        void FinishLoadingStageEnvironment();

        void StartSettingStagewideService();
        
        IObservable<int> SettingStagewideService { get; }

        IObservable<int> DoneSettingStagewideService { get; }
        void FinishSetupStagewideService();
        
        IObservable<int> CleaningStagewideService { get; }
    }
}
