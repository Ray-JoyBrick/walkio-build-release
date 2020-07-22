namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Events;

    //
#if WALKIO_LEVEL_ASSIST
    using GameLevelAssist = JoyBrick.Walkio.Game.Level.Assist;
#endif

    public partial class Bootstrap
    {
        // private void SendShowHideRequestEventForStage(int category, bool hide, int timeInMs)
        private void SendShowHideRequestEventForStage(int category, bool hide)
        {
            var archetype =
                World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(
                    typeof(GameLevelAssist.ShowHideRequest),
                    typeof(GameLevelAssist.ShowHideRequestProperty));

            // Observable.Timer(System.TimeSpan.FromMilliseconds(timeInMs))
            //     .Subscribe(_ =>
            //     {
            var eventEntity =
                World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(archetype);
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(
                eventEntity, new GameLevelAssist.ShowHideRequestProperty
                {
                    Category = category,
                    Hide = hide
                });
                // })
                // .AddTo(_compositeDisposable);
        }
    }
}
