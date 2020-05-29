namespace JoyBrick.Walkio.Game.Hud.Preparation
{
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class HudUnloadSceneSystem : SystemBase
    {
        public GameCommon.IServiceManagement ServiceManagement { get; set; }
        public GameCommon.ICommandHandler CommandHandler { get; set; }
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public void LoadZone(int index)
        {
            CommandHandler.LoadZone(index);
        }
        
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public void Setup()
        {
            ServiceManagement.UnloadPreparationHud
                .Subscribe(x =>
                {
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
}
