namespace JoyBrick.Walkio.Game.Hud.Preparation
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
    public class LoadPreparationHudSystem : SystemBase
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        //
        private GameObject _canvasPrefab;
        private GameObject _viewLoadingPrefab;
        private ScriptableObject _timelineAsset;
        private ScriptableObject _i2Asset;
        
        private GameObject _canvas;

        //
        public GameCommand.ICommandService CommandService { get; set; }

        //
        public void Construct()
        {
            base.OnCreate();

            //
            CommandService.LoadingPreparationHud
                .Subscribe(x =>
                {
                    //
                    Load().ToObservable()
                        .ObserveOnMainThread()
                        .SubscribeOnMainThread()
                        .Subscribe(result =>
                        {
                            //
                            (_canvasPrefab, _viewLoadingPrefab, _timelineAsset, _i2Asset) = result;
                            
                            //
                            _canvas = GameObject.Instantiate(_canvasPrefab);
                            AddCommandStreamAndInfoStream(_canvas);
                            
                            //
                            CommandService.FinishLoadingPreparationHud();
                        })
                        .AddTo(_compositeDisposable);
                })
                .AddTo(_compositeDisposable);
        }
        
        private async Task<T> GetAsset<T>(string addressName)
        {
            var handle = Addressables.LoadAssetAsync<T>(addressName);
            var r = await handle.Task;
        
            return r;
        }
        
        private async Task<(GameObject, GameObject, ScriptableObject, ScriptableObject)> Load()
        {
            var canvasPrefabTask = GetAsset<GameObject>($"Hud - Canvas - Preparation");
            var viewLoadingPrefabTask = GetAsset<GameObject>($"Hud - Preparation - View - Base Prefab");
            var timelineAssetTask = GetAsset<ScriptableObject>($"Hud - Preparation - View - Base Timeline");
            var i2AssetTask = GetAsset<ScriptableObject>($"Hud - Preparation - I2");

            var (canvasPrefab, viewLoadingPrefab, timelineAsset, i2Asset) =
                (await canvasPrefabTask, await viewLoadingPrefabTask, await timelineAssetTask, await i2AssetTask);

            return (canvasPrefab, viewLoadingPrefab, timelineAsset, i2Asset);
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
            Addressables.ReleaseInstance(_viewLoadingPrefab);
            Addressables.Release(_timelineAsset);
            Addressables.Release(_i2Asset);

            //
            GameObject.Destroy(_canvas);

            //
            _compositeDisposable?.Dispose();
        }
    }
}
