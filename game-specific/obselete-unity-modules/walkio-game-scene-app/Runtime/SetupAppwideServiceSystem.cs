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
    public class SetupAppwideServiceSystem : SystemBase
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
            CommandService.SettingAppwideService
                .Subscribe(x =>
                {
                    //
                    CommandService.FinishSetupAppwideService();
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
