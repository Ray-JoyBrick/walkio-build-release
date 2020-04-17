namespace JoyBrick.Walkio.Game.Scene.Stage
{
    using System;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;

    [DisableAutoCreation]
    public class InitializeStagewideServiceSystem : SystemBase
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
            CommandService.InitializingStagewideService
                .Subscribe(x =>
                {
                    //
                    CommandService.StartLoadingStageHud();
                    CommandService.StartLoadingStageEnvironment();
                })
                .AddTo(_compositeDisposable);

            var ob1 = CommandService.DoneLoadingStageHud;
            var ob2 = CommandService.DoneLoadingStageEnvironment;

            var combinedObservable = ob1.Merge(ob2);
            combinedObservable
                .Buffer(2)
                .Subscribe(x =>
                {
                    //
                    CommandService.StartSettingStagewideService();
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
