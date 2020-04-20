namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    
    using GameCommand = JoyBrick.Walkio.Game.Command;
    
    public partial class Bootstrap :
        GameCommand.ICommandService
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        private IObservable<int> SetupECSDone => _notifySetupECSDone.AsObservable();
        private readonly Subject<int> _notifySetupECSDone = new Subject<int>();
        
        // public IObservable<int> LoadAppHud => _notifyLoadAppHud.AsObservable();
        // private readonly Subject<int> _notifyLoadAppHud = new Subject<int>();
        
        //
        public readonly List<GameCommand.ICommandStreamProducer> _commandStreamProducers = new List<GameCommand.ICommandStreamProducer>();

        public IObservable<GameCommand.ICommand> CommandStream => _rpCommands.Select(x => x).Switch();
        private readonly ReactiveProperty<IObservable<GameCommand.ICommand>> _rpCommands =
            new ReactiveProperty<IObservable<GameCommand.ICommand>>(Observable.Empty<GameCommand.ICommand>());
        private readonly Subject<GameCommand.ICommand> _notifyCommand = new Subject<GameCommand.ICommand>();
        
        public void AddCommandStreamProducer(GameCommand.ICommandStreamProducer commandStreamProducer)
        {
            var existed =_commandStreamProducers.Exists(x => x == commandStreamProducer);
            if (existed) return;
            
            _commandStreamProducers.Add(commandStreamProducer);
            ReformCommandStream();
        }
        
        void RemoveCommandStreamProducer(GameCommand.ICommandStreamProducer commandStreamProducer)
        {
            var existed =_commandStreamProducers.Exists(x => x == commandStreamProducer);
            if (!existed) return;
            
            _commandStreamProducers.Remove(commandStreamProducer);
            ReformCommandStream();
        }

        void ReformCommandStream()
        {
            var combinedObs =
                _commandStreamProducers
                    .Select(x => x.CommandStream)
                    // .Aggregate(Observable.Empty<ICommand>(), (acc, next) => acc.Merge(next));
                    .Aggregate(_notifyCommand.AsObservable(), (acc, next) => acc.Merge(next));
            
            _rpCommands.Value = combinedObs;            
        }
        
        public readonly List<GameCommand.IInfoPresenter> _infoPresenters = new List<GameCommand.IInfoPresenter>();

        public IObservable<GameCommand.IInfo> InfoStream => _rpInfos.Select(x => x).Switch();
        private readonly ReactiveProperty<IObservable<GameCommand.IInfo>> _rpInfos =
            new ReactiveProperty<IObservable<GameCommand.IInfo>>(Observable.Empty<GameCommand.IInfo>());

        public void AddInfoStreamPresenter(GameCommand.IInfoPresenter infoPresenter)
        {
            var existed =_infoPresenters.Exists(x => x == infoPresenter);
            if (existed) return;
            
            _infoPresenters.Add(infoPresenter);
            ReformInfoStream();
        }

        void ReformInfoStream()
        {
            var combinedObs =
                _infoPresenters
                    .Select(x => x.InfoStream)
                    .Aggregate(Observable.Empty<GameCommand.IInfo>(), (acc, next) => acc.Merge(next));
            
            _rpInfos.Value = combinedObs;
        }

        public void SendCommand(string commandName)
        {
            _logger.Debug($"Bootstrap - SendCommand - commandName: {commandName}");
            
            if (String.CompareOrdinal(commandName, "Activating Loading View") == 0)
            {
                _notifyCommand.OnNext(new GameCommand.ActivateLoadingViewCommand
                {
                    flag = true
                });
            }
            else if (String.CompareOrdinal(commandName, "Deactivating Loading View") == 0)
            {
                _notifyCommand.OnNext(new GameCommand.ActivateLoadingViewCommand
                {
                    flag = false
                });
            }
        }

        //
        public IObservable<int> InitializingAppwideService => _notifyInitializingAppwideService.AsObservable();
        private readonly Subject<int> _notifyInitializingAppwideService = new Subject<int>();
        public void StartInitializingAppwideService()
        {
            _notifyInitializingAppwideService.OnNext(1);
        }
        
        //
        public IObservable<int> LoadingAppHud => _notifyLoadingAppHud.AsObservable();
        private readonly Subject<int> _notifyLoadingAppHud = new Subject<int>();

        public void StartLoadingAppHud()
        {
            _notifyLoadingAppHud.OnNext(1);
        }

        public void FinishLoadingAppHud()
        {
            _notifyDoneLoadingAppHud.OnNext(1);
        }

        public IObservable<int> SettingAppwideService => _notifySettingAppwideService.AsObservable();
        private readonly Subject<int> _notifySettingAppwideService = new Subject<int>();

        public void StartSettingAppwideService()
        {
            _notifySettingAppwideService.OnNext(1);
        }

        public IObservable<int> DoneSettingAppwideService => _notifyDoneSettingAppwideService.AsObservable();
        private readonly Subject<int> _notifyDoneSettingAppwideService = new Subject<int>();

        public void FinishSetupAppwideService()
        {
            //
            _notifyDoneSettingAppwideService.OnNext(1);
            
            // Extension
            
            _notifyInitializingPreparationwideService.OnNext(1);
        }

        public void ActivateViewLoading(bool flag)
        {
            _notifyCommand.OnNext(new GameCommand.ActivateLoadingViewCommand
            {
                flag = flag
            });
        }

        public IObservable<int> InitializingPreparationwideService =>
            _notifyInitializingPreparationwideService.AsObservable();
        private readonly Subject<int> _notifyInitializingPreparationwideService = new Subject<int>();

        public IObservable<int> DoneLoadingAppHud => _notifyDoneLoadingAppHud.AsObservable();
        private readonly Subject<int> _notifyDoneLoadingAppHud = new Subject<int>();

        public IObservable<int> EnteringPreparationScene => _notifyEnteringPreparationScene.AsObservable();
        private readonly Subject<int> _notifyEnteringPreparationScene = new Subject<int>();
        public IObservable<int> ExitingPreparationScene => _notifyExitingPreparationScene.AsObservable();
        private readonly Subject<int> _notifyExitingPreparationScene = new Subject<int>();

        //
        public void StartEnteringPreparationScene()
        {
            _notifyEnteringPreparationScene.OnNext(1);
        }

        public void StartExitingPreparationScene()
        {
            _notifyExitingPreparationScene.OnNext(1);
        }

        public IObservable<int> LoadingPreparationHud => _notifyLoadingPreparationHud.AsObservable();
        private readonly Subject<int> _notifyLoadingPreparationHud = new Subject<int>();
        public IObservable<int> DoneLoadingPreparationHud => _notifyDoneLoadingPreparationHud.AsObservable();
        private readonly Subject<int> _notifyDoneLoadingPreparationHud = new Subject<int>();
        
        public void StartLoadingPreparationHud()
        {
            _notifyLoadingPreparationHud.OnNext(1);
        }

        public void FinishLoadingPreparationHud()
        {
            _notifyDoneLoadingPreparationHud.OnNext(1);
        }

        public IObservable<int> SettingPreparationwideService => _notifySettingPreparationwideService.AsObservable();
        private readonly Subject<int> _notifySettingPreparationwideService = new Subject<int>();

        public void StartSettingPreparationService()
        {
            _notifySettingPreparationwideService.OnNext(1);
        }

        public IObservable<int> DoneSettingPreparationwideService =>
            _notifyDoneSettingPreparationwideService.AsObservable();
        private readonly Subject<int> _notifyDoneSettingPreparationwideService = new Subject<int>();

        public void FinishSetupPreparationwideService()
        {
            _notifyDoneSettingPreparationwideService.OnNext(1);
        }

        public IObservable<int> CleaningPreparationwideService => _notifyCleaningPreparationwideService.AsObservable();
        private readonly Subject<int> _notifyCleaningPreparationwideService = new Subject<int>();

        //
        public IObservable<int> InitializingStagewideService => _notifyInitializingStagewideService.AsObservable();
        private readonly Subject<int> _notifyInitializingStagewideService = new Subject<int>();

        public void StartInitializingStagewideService()
        {
            _notifyInitializingStagewideService.OnNext(1);
        }

        public IObservable<int> LoadingStageHud => _notifyLoadingStageHud.AsObservable();
        private readonly Subject<int> _notifyLoadingStageHud = new Subject<int>();

        public IObservable<int> DoneLoadingStageHud => _notifyDoneLoadingStageHud.AsObservable();
        private readonly Subject<int> _notifyDoneLoadingStageHud = new Subject<int>();

        public void StartLoadingStageHud()
        {
            _notifyLoadingStageHud.OnNext(1);
        }

        public void FinishLoadingStageHud()
        {
            _notifyDoneLoadingStageHud.OnNext(1);
        }

        public IObservable<int> LoadingStageEnvironment => _notifyLoadingStageEnvironment.AsObservable();
        private readonly Subject<int> _notifyLoadingStageEnvironment = new Subject<int>();

        public IObservable<int> DoneLoadingStageEnvironment => _notifyDoneLoadingStageEnvironment.AsObservable();
        private readonly Subject<int> _notifyDoneLoadingStageEnvironment = new Subject<int>();

        public void StartLoadingStageEnvironment()
        {
            _notifyLoadingStageEnvironment.OnNext(1);
        }

        public void FinishLoadingStageEnvironment()
        {
            _notifyDoneLoadingStageEnvironment.OnNext(1);
        }

        public IObservable<int> SettingStagewideService => _notifySettingStagewideService.AsObservable();
        private readonly Subject<int> _notifySettingStagewideService = new Subject<int>();

        public void StartSettingStagewideService()
        {
            _notifySettingStagewideService.OnNext(1);
        }

        public IObservable<int> DoneSettingStagewideService =>
            _notifyDoneSettingStagewideService.AsObservable();
        private readonly Subject<int> _notifyDoneSettingStagewideService = new Subject<int>();

        public void FinishSetupStagewideService()
        {
            _notifyDoneSettingStagewideService.OnNext(1);
        }

        public IObservable<int> CleaningStagewideService => _notifyCleaningStagewideService.AsObservable();
        private readonly Subject<int> _notifyCleaningStagewideService = new Subject<int>();
    }
}
