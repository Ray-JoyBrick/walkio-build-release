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
        MonoBehaviour
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(Bootstrap));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        void Awake()
        {
            //
            SetupAppCenterDistribute();

            //
            HandleSceneLoad();

            //
            RegisterToDrawingManager();
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
