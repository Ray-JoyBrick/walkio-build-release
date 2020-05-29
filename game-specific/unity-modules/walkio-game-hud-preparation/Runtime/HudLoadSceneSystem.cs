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
    public class HudLoadSceneSystem : SystemBase
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
            ServiceManagement.LoadPreparationHud
                .Subscribe(x =>
                {
                    //
                    Load().ToObservable()
                        .ObserveOnMainThread()
                        .SubscribeOnMainThread()
                        .Subscribe(result =>
                        {
                            //
                            var sceneInstance = result;
                        })
                        .AddTo(_compositeDisposable);
                })
                .AddTo(_compositeDisposable);
        }

        private async Task<SceneInstance> Load()
        {
            var zoneSceneAddress = $"Preparation Hud Scene";
            var handle1 = Addressables.LoadSceneAsync(zoneSceneAddress, LoadSceneMode.Additive);

            var t1 = await handle1.Task;

            return t1;
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
