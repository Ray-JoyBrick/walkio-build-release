namespace JoyBrick.Walkio.Game.Scene.App
{
    using System;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;

    [DisableAutoCreation]
    // public class InitializeAppwideServiceSystem<T> : SystemBase
    //     where T : GameCommand.ICommandService
    public class InitializeAppwideServiceSystem : SystemBase
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        public GameCommand.ICommandService CommandService { get; set; }

        //
        public void Construct()
        {
            base.OnCreate();

            //
            CommandService.InitializingAppwideService
                .Subscribe(x =>
                {
                    //
                    CommandService.StartLoadingAppHud();
                })
                .AddTo(_compositeDisposable);

            var ob1 = CommandService.DoneLoadingAppHud;

            var combinedObservable = ob1;
            combinedObservable
                .Buffer(1)
                .Subscribe(x =>
                {
                    //
                    CommandService.StartSettingAppwideService();
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnUpdate() {}

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
}
