namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Linq;
#if COMPLETE_PROJECT
    using Microsoft.AppCenter.Unity.Distribute;
#endif
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;
    
    using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameCommon = JoyBrick.Walkio.Game.Common;

#if COMPLETE_PROJECT || BEHAVIOR_PROJECT

    using GameHudAppAssist = JoyBrick.Walkio.Game.Hud.App.Assist;
    // using GameHudPreparation = JoyBrick.Walkio.Game.Hud.Preparation;
    using GameHudStageAssist = JoyBrick.Walkio.Game.Hud.Stage.Assist;
    
#endif

    public partial class Bootstrap :
        MonoBehaviour,
        IBootstrapAssistant
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(Bootstrap));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private IBootstrapAssistable _assistable;
        
        //
        void Awake()
        {
            SetupAppCenterDistribute();
            
            var sceneLoadedObservable =
                Observable.FromEvent<UnityAction<Scene, LoadSceneMode>, Tuple<Scene, LoadSceneMode>>(
                    h => (x, y) => h(Tuple.Create(x, y)),
                    h => SceneManager.sceneLoaded += h,
                    h => SceneManager.sceneLoaded -= h);
            sceneLoadedObservable
                .Subscribe(x =>
                {
                    var (scene, _) = x;
                    //
                    if (string.CompareOrdinal(scene.name, "Entry") == 0)
                    {
                        AddSelfToAssistable(scene);
                    }
                })
                .AddTo(_compositeDisposable);
        }

        private void AddSelfToAssistable(Scene scene)
        {
            Debug.Log($"Bootstrap Assist - AddSelfToAssistable");
    
            if (scene.IsValid())
            {
                var assistables =
                    scene.GetRootGameObjects()
                        .Where(s => s.GetComponent<IBootstrapAssistable>() != null)
                        .ToList();

                if (assistables.Any())
                {
                    Debug.Log($"Bootstrap Assist - AddSelfToAssistable - Found Assistables on scene: {scene.name}");
                    _assistable = assistables.First().GetComponent<IBootstrapAssistable>();
                    _assistable?.AddAssistant((IBootstrapAssistant) this);
                }
            }
        }

        void Start()
        {
            _logger.Debug($"Bootstrap Assist - Start");

            _assistable?.CanStartInitialSetup
                .Subscribe(x =>
                {
                    //
                    SetupEcsSystem();
                })
                .AddTo(_compositeDisposable);
        }

        private void SetupEcsSystem()
        {
            _logger.Debug($"Bootstrap Assist - SetupEcsSystem");

            //
            var initializationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();
            var simulationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>();
            var presentationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PresentationSystemGroup>();
            
            // App-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            var loadAppHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudAppAssist.LoadAppHudSystem>();
            // var setupAppHudSystem =
            //     World.DefaultGameObjectInjectionWorld
            //         .GetOrCreateSystem<GameHudAppAssist.SetupAppHudSystem>();
#endif
            
            // Stage-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            var loadStageHudSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameHudStageAssist.LoadStageHudSystem>();
#endif

            // App-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadAppHudSystem.RefBootstrap = _assistable.RefGameObject;
            loadAppHudSystem.CommandService = _assistable.RefGameObject.GetComponent<GameCommand.ICommandService>();
            // // loadAppHudSystem.InfoPresenter = (GameCommand.IInfoPresenter) this;
            loadAppHudSystem.FlowControl = _assistable.RefGameObject.GetComponent<GameCommon.IFlowControl>();
            // // setupAppHudSystem.FlowControl = (GameCommon.IFlowControl) this;
#endif

            // Stage-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadStageHudSystem.RefBootstrap = _assistable.RefGameObject;
            loadStageHudSystem.CommandService = _assistable.RefGameObject.GetComponent<GameCommand.ICommandService>();
            // // loadStageHudSystem.InfoPresenter = (GameCommand.IInfoPresenter) this;
            loadStageHudSystem.FlowControl = _assistable.RefGameObject.GetComponent<GameCommon.IFlowControl>();
#endif

            // App-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadAppHudSystem.Construct();
            // // setupAppHudSystem.Construct();
#endif

            // Stage-wide
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            loadStageHudSystem.Construct();
#endif

            // App-wide - InitializationSystemGroup
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadAppHudSystem);
            // // initializationSystemGroup.AddSystemToUpdateList(setupAppHudSystem);
#endif

            // Stage-wide - InitializationSystemGroup
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            initializationSystemGroup.AddSystemToUpdateList(loadStageHudSystem);
#endif
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
