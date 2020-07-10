namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameLevel = JoyBrick.Walkio.Game.Level;

    public partial class Bootstrap
    {
        private void StartGameFlow()
        {
            _logger.Debug($"Bootstrap Assist - StartGameFlow");

#if CREATURE_DESIGN_PROJECT
            SignalStartLoadingAssetForStage();

            SendShowHideRequestEventForStage(1, true, 5000);
            SendShowHideRequestEventForStage(2, true, 5100);
            SendShowHideRequestEventForStage(1,false, 15000);
            SendShowHideRequestEventForStage(2,false, 15100);
#endif
        }

#if CREATURE_DESIGN_PROJECT

        private void SignalStartLoadingAssetForStage()
        {
            Observable.Timer(System.TimeSpan.FromMilliseconds(500))
                .Subscribe(_ =>
                {
                    var flowControl = _assistable.RefGameObject.GetComponent<GameCommon.IFlowControl>();
                    flowControl.StartLoadingAsset("Stage");
                })
                .AddTo(_compositeDisposable);
        }

        private void SendShowHideRequestEventForStage(int category, bool hide, int timeInMs)
        {
            Observable.Timer(System.TimeSpan.FromMilliseconds(timeInMs))
                .Subscribe(_ =>
                {
                    var archetype =
                        World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(
                            typeof(GameLevel.Assist.ShowHideRequest),
                            typeof(GameLevel.Assist.ShowHideRequestProperty));

                    var eventEntity =
                        World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(archetype);
                    World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(
                        eventEntity, new GameLevel.Assist.ShowHideRequestProperty
                        {
                            Category = category,
                            Hide = hide
                        });
                })
                .AddTo(_compositeDisposable);
        }

#endif
    }
}
