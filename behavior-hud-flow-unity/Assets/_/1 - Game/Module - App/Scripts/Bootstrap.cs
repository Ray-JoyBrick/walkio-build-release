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

    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameHudApp = JoyBrick.Walkio.Game.Hud.App;
    using GameHudPreparation = JoyBrick.Walkio.Game.Hud.Preparation;
    using GameHudStage = JoyBrick.Walkio.Game.Hud.Stage;
    using GameSceneApp = JoyBrick.Walkio.Game.Scene.App;
    using GameScenePreparation = JoyBrick.Walkio.Game.Scene.Preparation;
    using GameSceneStage = JoyBrick.Walkio.Game.Scene.Stage;
    
    public partial class Bootstrap :
        MonoBehaviour
    {
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
            
            SetupECS();
        }

        private void SetupECS()
        {
            Debug.Log($"Bootstrap - SetupECS");

            //
            var initializationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();
            var simulationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>();

            // Appwide
            var initializeAppwideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameSceneApp.InitializeAppwideServiceSystem>();
            var loadAppHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudApp.LoadAppHudSystem>();
            
            var setupAppwideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameSceneApp.SetupAppwideServiceSystem>();
            
            // Preparationwide
            var initializePreparationwideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameScenePreparation.InitializePreparationwideServiceSystem>();
            var loadPreparationHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudPreparation.LoadPreparationHudSystem>();
            var setupPreparationwideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameScenePreparation.SetupPreparationwideServiceSystem>();
            var cleanupPreparationwideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameScenePreparation.CleanupPreparationwideServiceSystem>();

            // Stagewide
            var initializeStagewideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameSceneStage.InitializeStagewideServiceSystem>();
            var loadStageHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudStage.LoadStageHudSystem>();
            var loadStageEnvironmentSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameSceneStage.LoadStageEnvironmentSystem>();
            var setupStagewideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameSceneStage.SetupStagewideServiceSystem>();
            var cleanupStagewideServiceSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameSceneStage.CleanupStagewideServiceSystem>();

            // Appwide
            initializeAppwideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            loadAppHudSystem.CommandService = (GameCommand.ICommandService) this;
            setupAppwideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            
            // Preparationwide
            initializePreparationwideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            loadPreparationHudSystem.CommandService = (GameCommand.ICommandService) this;
            setupPreparationwideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            cleanupPreparationwideServiceSystem.CommandService = (GameCommand.ICommandService) this;

            // Stagewide
            initializeStagewideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            loadStageHudSystem.CommandService = (GameCommand.ICommandService) this;
            loadStageEnvironmentSystem.CommandService = (GameCommand.ICommandService) this;
            setupStagewideServiceSystem.CommandService = (GameCommand.ICommandService) this;
            cleanupStagewideServiceSystem.CommandService = (GameCommand.ICommandService) this;

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
