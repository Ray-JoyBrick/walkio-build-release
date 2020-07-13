namespace JoyBrick.Walkio.Game.Scene.Preparation
{
    using System;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;

    [DisableAutoCreation]
    public class InitializePreparationwideServiceSystem : SystemBase
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
            CommandService.InitializingPreparationwideService
                .Subscribe(x =>
                {
                    //
                    CommandService.StartLoadingPreparationHud();
                })
                .AddTo(_compositeDisposable);

            var ob1 = CommandService.DoneLoadingPreparationHud;

            var combinedObservable = ob1;
            combinedObservable
                .Buffer(1)
                .Subscribe(x =>
                {
                    //
                    CommandService.StartSettingPreparationService();
                })
                .AddTo(_compositeDisposable);

            //
            CommandService.ActivateViewLoading(true);
        }

        protected override void OnUpdate() {}

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
}
