namespace JoyBrick.Walkio.Game.Hud.App
{
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameTemplate = JoyBrick.Walkio.Game.Template;
    
    public class LoadAssetSystem :
        SystemBase
    {
        private class LoadContext
        {
            public string appHudScene;
        }
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        // public void Setup(GameTemplate.AppwideSettings appwideSettings)
        public void Setup(GameCommon.AppwideSettings appwideSettings)
        {
            // ServiceManagement.LoadAppHud
            Observable.Empty<int>()
                .Subscribe(x =>
                {
                    var loadContext = new LoadContext
                    {
                        appHudScene = appwideSettings.appHudScene
                    };

                    Load(loadContext).ToObservable()
                        .ObserveOnMainThread()
                        .SubscribeOnMainThread()
                        .Subscribe(result =>
                        {
                            //
                            var sceneInstance = result;
                            
                            // ServiceManagement.LoadAppHudDone();
                        })
                        .AddTo(_compositeDisposable);
                })
                .AddTo(_compositeDisposable);
        }
        
        private async Task<SceneInstance> Load(LoadContext context)
        {
            var handle1 = Addressables.LoadSceneAsync(context.appHudScene, LoadSceneMode.Additive);

            var t1 = await handle1.Task;

            return t1;
        }        
        
        protected override void OnUpdate()
        {
        }
    }
}
