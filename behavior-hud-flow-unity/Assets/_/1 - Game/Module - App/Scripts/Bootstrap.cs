namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.AddressableAssets.ResourceLocators;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public partial class Bootstrap :
        MonoBehaviour
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        private IObservable<int> SetupECSDone => _notifySetupECSDone.AsObservable();
        private readonly Subject<int> _notifySetupECSDone = new Subject<int>();
        
        // public IObservable<int> LoadAppHud => _notifyLoadAppHud.AsObservable();
        // private readonly Subject<int> _notifyLoadAppHud = new Subject<int>();
        
        //
        public readonly List<ICommandStreamProducer> _commandStreamProducers = new List<ICommandStreamProducer>();

        public IObservable<ICommand> CommandStream => _rpCommands.Select(x => x).Switch();
        private readonly ReactiveProperty<IObservable<ICommand>> _rpCommands =
            new ReactiveProperty<IObservable<ICommand>>(Observable.Empty<ICommand>());
        private readonly Subject<ICommand> _notifyCommand = new Subject<ICommand>();
        
        public void AddCommandStreamProducer(ICommandStreamProducer commandStreamProducer)
        {
            var existed =_commandStreamProducers.Exists(x => x == commandStreamProducer);
            if (existed) return;
            
            _commandStreamProducers.Add(commandStreamProducer);
            ReformCommandStream();
        }
        
        void RemoveCommandStreamProducer(ICommandStreamProducer commandStreamProducer)
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
        
        public readonly List<IInfoPresenter> _infoPresenters = new List<IInfoPresenter>();

        public IObservable<IInfo> InfoStream => _rpInfos.Select(x => x).Switch();
        private readonly ReactiveProperty<IObservable<IInfo>> _rpInfos =
            new ReactiveProperty<IObservable<IInfo>>(Observable.Empty<IInfo>());

        public void AddInfoStreamPresenter(IInfoPresenter infoPresenter)
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
                    .Aggregate(Observable.Empty<IInfo>(), (acc, next) => acc.Merge(next));
            
            _rpInfos.Value = combinedObs;
        }
        
        //
        void Start()
        {
            //
            SetupECSDone
                .Subscribe(x =>
                {
                    // _notifyLoadAppHud.OnNext(1);
                    StartInitializingAppwideService();
                })
                .AddTo(_compositeDisposable);

            CommandStream
                .Subscribe(x =>
                {
                    //
                    Debug.Log($"command: {x}");
                })
                .AddTo(_compositeDisposable);
            
            //
            SetupAddressable();
        }
        
        private void SetupAddressable()
        {
            var addressableInitializeAsync = Addressables.InitializeAsync();
            
            // This might cause Exception: Attempting to use an invalid operation handle
            // Workaround is to not unregister the event
            var addressableInitializeAsyncObservable =
                Observable
                    .FromEvent<AsyncOperationHandle<IResourceLocator>>(
                        h => addressableInitializeAsync.Completed += h,
                        h => { });
            addressableInitializeAsyncObservable                
                .Subscribe(x =>
                {
                    //
                    Debug.Log($"Bootstrap - addressableInitializeAsync is received");

                    HandleAddressableInitializeAsyncCompleted();
                    _notifySetupECSDone.OnNext(1);
                })
                .AddTo(_compositeDisposable);            
        }


        private void HandleAddressableInitializeAsyncCompleted()
        {
            Debug.Log($"Bootstrap - HandleAddressableInitializeAsyncCompleted");
            
            //
            var initializationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();
            var simulationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>();

            // Appwide
            var initializeAppwideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<InitializeAppwideServiceSystem>();
            var loadAppHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<LoadAppHudSystem>();
            
            var setupAppwideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<SetupAppwideServiceSystem>();
            
            // Preparationwide
            var initializePreparationwideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<InitializePreparationwideServiceSystem>();
            var loadPreparationHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<LoadPreparationHudSystem>();
            var setupPreparationwideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<SetupPreparationwideServiceSystem>();
            var cleanupPreparationwideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<CleanupPreparationwideServiceSystem>();

            // Stagewide
            var initializeStagewideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<InitializeStagewideServiceSystem>();
            var loadStageHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<LoadStageHudSystem>();
            var loadStageEnvironmentSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<LoadStageEnvironmentSystem>();
            var setupStagewideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<SetupStagewideServiceSystem>();
            var cleanupStagewideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<CleanupStagewideServiceSystem>();

            // Appwide
            initializeAppwideServiceSystem.CommandService = (ICommandService) this;
            loadAppHudSystem.CommandService = (ICommandService) this;
            setupAppwideServiceSystem.CommandService = (ICommandService) this;
            
            // Preparationwide
            initializePreparationwideServiceSystem.CommandService = (ICommandService) this;
            loadPreparationHudSystem.CommandService = (ICommandService) this;
            setupPreparationwideServiceSystem.CommandService = (ICommandService) this;
            cleanupPreparationwideServiceSystem.CommandService = (ICommandService) this;

            // Stagewide
            initializeStagewideServiceSystem.CommandService = (ICommandService) this;
            loadStageHudSystem.CommandService = (ICommandService) this;
            loadStageEnvironmentSystem.CommandService = (ICommandService) this;
            setupStagewideServiceSystem.CommandService = (ICommandService) this;
            cleanupStagewideServiceSystem.CommandService = (ICommandService) this;

            // Appwide
            initializeAppwideServiceSystem.Construct();
            loadAppHudSystem.Construct();
            setupAppwideServiceSystem.Construct();
            
            // Preparationwide
            initializePreparationwideServiceSystem.Construct();
            loadPreparationHudSystem.Construct();
            setupPreparationwideServiceSystem.Construct();
            cleanupPreparationwideServiceSystem.Construct();
            
            // Stagewide
            initializeStagewideServiceSystem.Construct();
            loadStageHudSystem.Construct();
            loadStageEnvironmentSystem.Construct();
            setupStagewideServiceSystem.Construct();
            cleanupStagewideServiceSystem.Construct();

            // Appwide - InitializationSystemGroup
            initializationSystemGroup.AddSystemToUpdateList(initializeAppwideServiceSystem);
            initializationSystemGroup.AddSystemToUpdateList(loadAppHudSystem);
            initializationSystemGroup.AddSystemToUpdateList(setupAppwideServiceSystem);
            
            // Preparationwide - InitializationSystemGroup
            initializationSystemGroup.AddSystemToUpdateList(initializePreparationwideServiceSystem);
            initializationSystemGroup.AddSystemToUpdateList(loadPreparationHudSystem);
            initializationSystemGroup.AddSystemToUpdateList(setupPreparationwideServiceSystem);
            initializationSystemGroup.AddSystemToUpdateList(cleanupPreparationwideServiceSystem);
            
            // Stagewide - InitializationSystemGroup
            initializationSystemGroup.AddSystemToUpdateList(initializeStagewideServiceSystem);
            initializationSystemGroup.AddSystemToUpdateList(loadStageHudSystem);
            initializationSystemGroup.AddSystemToUpdateList(loadStageEnvironmentSystem);
            initializationSystemGroup.AddSystemToUpdateList(setupStagewideServiceSystem);
            initializationSystemGroup.AddSystemToUpdateList(cleanupStagewideServiceSystem);
        }
        
        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
