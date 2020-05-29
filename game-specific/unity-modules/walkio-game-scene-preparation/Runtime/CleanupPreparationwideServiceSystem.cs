namespace JoyBrick.Walkio.Game.Scene.Preparation
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
    public class CleanupPreparationwideServiceSystem : SystemBase
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
            CommandService.CleaningPreparationwideService
                .Subscribe(x =>
                {
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
