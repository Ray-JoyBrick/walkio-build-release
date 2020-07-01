namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;
    
    using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameCommon = JoyBrick.Walkio.Game.Common;
    
    using GameCreature = JoyBrick.Walkio.Game.Creature;

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
                    SetupEcsWorld();
                    SetupEcsSystem();
                    
                    //
                    StartGameFlow();
                })
                .AddTo(_compositeDisposable);
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
