namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    
    using GameCommand = JoyBrick.Walkio.Game.Command;
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT || HUD_FLOW_PROJECT
    using GameExtension = JoyBrick.Walkio.Game.Extension;
#endif
    
    public partial class Bootstrap :
        GameCommand.ICommandService
    {
        //
        public readonly List<GameCommand.ICommandStreamProducer> _commandStreamProducers =
            new List<GameCommand.ICommandStreamProducer>();

        public IObservable<GameCommand.ICommand> CommandStream =>
            _rpCommands.Select(x => x).Switch();
        private readonly ReactiveProperty<IObservable<GameCommand.ICommand>> _rpCommands =
            new ReactiveProperty<IObservable<GameCommand.ICommand>>(Observable.Empty<GameCommand.ICommand>());
        private readonly Subject<GameCommand.ICommand> _notifyCommand = new Subject<GameCommand.ICommand>();
        
        public void AddCommandStreamProducer(GameCommand.ICommandStreamProducer commandStreamProducer)
        {
            _logger.Debug($"Bootstrap - AddCommandStreamProducer - {commandStreamProducer}");
            
            var existed =_commandStreamProducers.Exists(x => x == commandStreamProducer);
            if (existed) return;
            
            _commandStreamProducers.Add(commandStreamProducer);
            ReformCommandStream();
        }
        
        public void RemoveCommandStreamProducer(GameCommand.ICommandStreamProducer commandStreamProducer)
        {
            _logger.Debug($"Bootstrap - RemoveCommandStreamProducer - {commandStreamProducer}");
            
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
        
        public readonly List<GameCommand.IInfoPresenter> _infoPresenters =
            new List<GameCommand.IInfoPresenter>();

        public IObservable<GameCommand.IInfo> InfoStream =>
            _rpInfos.Select(x => x).Switch();
        private readonly ReactiveProperty<IObservable<GameCommand.IInfo>> _rpInfos =
            new ReactiveProperty<IObservable<GameCommand.IInfo>>(Observable.Empty<GameCommand.IInfo>());

        public void AddInfoStreamPresenter(GameCommand.IInfoPresenter infoPresenter)
        {
            _logger.Debug($"Bootstrap - AddInfoStreamPresenter - {infoPresenter}");
            
            var existed =_infoPresenters.Exists(x => x == infoPresenter);
            if (existed) return;
            
            _infoPresenters.Add(infoPresenter);
            ReformInfoStream();
        }

        public void RemoveInfoStreamPresenter(GameCommand.IInfoPresenter infoPresenter)
        {
            _logger.Debug($"Bootstrap - RemoveInfoStreamPresenter - {infoPresenter}");
            
            var existed =_infoPresenters.Exists(x => x == infoPresenter);
            if (!existed) return;

            _infoPresenters.Remove(infoPresenter);
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
                    Flag = true
                });
            }
            else if (String.CompareOrdinal(commandName, "Deactivating Loading View") == 0)
            {
                _notifyCommand.OnNext(new GameCommand.ActivateLoadingViewCommand
                {
                    Flag = false
                });
            }
            else if (String.CompareOrdinal(commandName, "Bring Up Help") == 0)
            {
                // _notifyCommand.OnNext(new GameCommand.ActivateLoadingViewCommand
                // {
                //     Flag = false
                // });
            }
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            else if (String.CompareOrdinal(commandName, "Load Preparation") == 0)
            {
                GameExtension.BridgeExtension.SendEvent("zz_Exit Current Flow");
                // GameExtension.BridgeExtension.SendEvent("zz_Enter Preparation");
            }
            else if (String.CompareOrdinal(commandName, "Load Stage") == 0)
            {
                GameExtension.BridgeExtension.SendEvent("zz_Exit Current Flow");
                // GameExtension.BridgeExtension.SendEvent("zz_Enter Stage");
                // _notifyCommand.OnNext(new GameCommand.ChangeToGameFlow
                // {
                //     FlowName = "Stage"
                // });
            }
            else if (String.CompareOrdinal(commandName, "Create Neutral Force Unit") == 0)
            {
                _notifyCommand.OnNext(new GameCommand.CreateNeutralForceUnit
                {
                });
            }
            else if (String.CompareOrdinal(commandName, "Stage Play Time Up") == 0)
            {
                // _notifyCommand.OnNext(new GameCommand.CreateNeutralForceUnit
                // {
                // });
                
                GameExtension.BridgeExtension.SendEvent("zz_Exit Current Flow");
            }
            else if (String.CompareOrdinal(commandName, "Place All Team Force Leader") == 0)
            {
                // var pool = FindObjectOfType<HellTap.PoolKit.Pool>();
                
                _notifyCommand.OnNext(new GameCommand.PlaceTeamForceLeader
                {
                });
            }
            else if (String.CompareOrdinal(commandName, "Create Team Leader From Pool") == 0)
            {
                CreateTeamLeaderFromPool();
            }

#endif
        }
    }
}
