namespace JoyBrick.Walkio.Game.Hud.Stage
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
    public class LoadStageHudSystem : SystemBase
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private GameObject _canvasPrefab;
        private GameObject _canvas;

        //
        public GameCommand.ICommandService CommandService { get; set; }

        //
        public void Construct()
        {
            base.OnCreate();

            //
            CommandService.LoadingStageHud
                .Subscribe(x =>
                {
                    //
                    Load().ToObservable()
                        .ObserveOnMainThread()
                        .SubscribeOnMainThread()
                        .Subscribe(result =>
                        {
                            //
                            _canvasPrefab = result;
                            
                            //
                            _canvas = GameObject.Instantiate(_canvasPrefab);
                            AddCommandStreamAndInfoStream(_canvas);
                            
                            //
                            CommandService.FinishLoadingStageHud();
                        })
                        .AddTo(_compositeDisposable);
                })
                .AddTo(_compositeDisposable);
        }
        
        private async Task<GameObject> Load()
        {
            var addressName = $"Hud - Stage";
            var handle = Addressables.LoadAssetAsync<GameObject>(addressName);

            var r = await handle.Task;

            return r;
        }
        
        private void AddCommandStreamAndInfoStream(GameObject inGO)
        {
            var commandStreamProducer = inGO.GetComponent<GameCommand.ICommandStreamProducer>();
            if (commandStreamProducer != null)
            {
                CommandService.AddCommandStreamProducer(commandStreamProducer);
            }

            var infoPresenter = inGO.GetComponent<GameCommand.IInfoPresenter>();
            if (infoPresenter != null)
            {
                CommandService.AddInfoStreamPresenter(infoPresenter);
            }            
        }

        protected override void OnUpdate() {}
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            //
            Addressables.ReleaseInstance(_canvasPrefab);
            GameObject.Destroy(_canvas);
            
            _compositeDisposable?.Dispose();
        }
    }
}
