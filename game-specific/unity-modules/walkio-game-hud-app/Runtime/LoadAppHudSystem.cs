namespace JoyBrick.Walkio.Game.Hud.App
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
    using GameExtension = JoyBrick.Walkio.Game.Extension;

    [DisableAutoCreation]
    public class LoadAppHudSystem : SystemBase
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private GameObject _canvasPrefab;
        private GameObject _viewLoadingPrefab;
        private ScriptableObject _timelineAsset;
        private ScriptableObject _i2Asset;

        //
        private GameObject _canvas;

        // private View _loadView;

        //
        public GameCommand.ICommandService CommandService { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            base.OnCreate();
            
            //
            FlowControl.LoadingAsset
                .Where(x => x.Name.Contains("App"))
                .Subscribe(x =>
                {
                    LoadingAsset();
                })
                .AddTo(_compositeDisposable);            

            //
            CommandService.CommandStream
                .Where(x => (x as GameCommand.ActivateLoadingViewCommand) != null)
                .Subscribe(x =>
                {
                    var activateLoadingViewCommand = (x as GameCommand.ActivateLoadingViewCommand);
                    //
                    ActivateLoadingView(activateLoadingViewCommand.flag);
                })
                .AddTo(_compositeDisposable);
        }

        private void LoadingAsset()
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

                    ExtractView();
                            
                    //
                    CommandService.FinishLoadingAppHud();
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
            var canvasPrefabTask = GetAsset<GameObject>($"Hud - Canvas - App");
            var viewLoadingPrefabTask = GetAsset<GameObject>($"Hud - App - View - Loading Prefab");
            var timelineAssetTask = GetAsset<ScriptableObject>($"Hud - App - View - Loading Timeline");
            var i2AssetTask = GetAsset<ScriptableObject>($"Hud - App - I2");

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
        
        private void ExtractView()
        {
            foreach (Transform v in _canvas.transform)
            {
                // var view = v.GetComponent<View>();
                // if (view != null)
                // {
                //     _loadView = view;
                // }
            }
        }

        private void ActivateLoadingView(bool flag)
        {
            if (flag)
            {
                GameExtension.BridgeExtension.SendEvent("Activate_Loading_View");
            }
            else
            {
                GameExtension.BridgeExtension.SendEvent("Deactivate_Loading_View");
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
            
            _compositeDisposable?.Dispose();
        }
    }
}
