namespace JoyBrick.Walkio.Game.Hud.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameExtension = JoyBrick.Walkio.Game.Extension;

    //
    [GameCommon.DoneLoadingAssetWait("App")]
    //
    [DisableAutoCreation]
    public class LoadAppHudSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadAppHudSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private GameObject _canvasPrefab;
        private GameObject _viewLoadingPrefab;
        private ScriptableObject _timelineAsset;
        private ScriptableObject _i2Asset;

        private HudData _hudData;

        //
        private GameObject _canvas;

        // private View _loadView;

        //
        public GameObject RefBootstrap { get; set; }
        public GameCommand.ICommandService CommandService { get; set; }
        // public GameCommand.IInfoPresenter InfoPresenter { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"LoadAppHudSystem - Construct");
            
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
                // .Do(x => _logger.Debug($"Receive Command Stream: {x}"))
                .Where(x => (x as GameCommand.ActivateLoadingViewCommand) != null)
                .Subscribe(x =>
                {
                    _logger.Debug($"LoadAppHudSystem - Construct - Receive ActivateLoadingViewCommand");
                    var activateLoadingViewCommand = (x as GameCommand.ActivateLoadingViewCommand);
                    //
                    ActivateLoadingView(activateLoadingViewCommand.Flag);
                })
                .AddTo(_compositeDisposable);
        }

        private void LoadingAsset()
        {
            _logger.Debug($"LoadAppHudSystem - LoadingAsset");

            //
            Load().ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    //
                    var dataAsset = result;
                    
                    _hudData = (dataAsset as HudData);

                    _canvasPrefab = _hudData.canvasPrefab;
                    
                    //
                    _canvas = GameObject.Instantiate(_canvasPrefab);
                    var scene = SceneManager.GetSceneByName("Entry");
                    if (scene.IsValid())
                    {
                        SceneManager.MoveGameObjectToScene(_canvas, scene);
                    }
                    
                    AddCommandStreamAndInfoStream(_canvas);
                    SetReferenceToExtension(_canvas);

                    ExtractView();
                            
                    //
                    FlowControl.FinishLoadingAsset(new GameCommon.FlowControlContext
                    {
                        Name = "App"
                    });
                })
                .AddTo(_compositeDisposable);            
        }

        private async Task<T> GetAsset<T>(string addressName)
        {
            var handle = Addressables.LoadAssetAsync<T>(addressName);
            var r = await handle.Task;
        
            return r;
        }
        
        private async Task<ScriptableObject> Load()
        {
            var hudDataAssetTask = GetAsset<ScriptableObject>($"Hud - App - Hud Data");

            var hudDataAsset = await hudDataAssetTask;

            return hudDataAsset;
        }
        
        // private async Task<(GameObject, GameObject, ScriptableObject, ScriptableObject)> Load()
        // {
        //     var canvasPrefabTask = GetAsset<GameObject>($"Hud - Canvas - App");
        //     var viewLoadingPrefabTask = GetAsset<GameObject>($"Hud - App - View - Loading Prefab");
        //     var timelineAssetTask = GetAsset<ScriptableObject>($"Hud - App - View - Loading Timeline");
        //     var i2AssetTask = GetAsset<ScriptableObject>($"Hud - App - I2");
        //
        //     var (canvasPrefab, viewLoadingPrefab, timelineAsset, i2Asset) =
        //         (await canvasPrefabTask, await viewLoadingPrefabTask, await timelineAssetTask, await i2AssetTask);
        //
        //     return (canvasPrefab, viewLoadingPrefab, timelineAsset, i2Asset);
        // }

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
            _logger.Debug($"LoadAppHudSystem - ActivateLoadingView - flag: {flag}");
            
            // TODO: Rename to follow the system use contract. Starting as "zz_"
            if (flag)
            {
                GameExtension.BridgeExtension.SendEvent("Activate_Loading_View");
                // GameExtension.BridgeExtension.SendEvent("zz_Activate_Loading_View");
            }
            else
            {
                GameExtension.BridgeExtension.SendEvent("Deactivate_Loading_View");
                // GameExtension.BridgeExtension.SendEvent("zz_Deactivate_Loading_View");
            }
        }
        
        // TODO: Move hard reference to PlayMakerFSM to somewhere else
        // TODO: Assign reference to FSM may need a better approach
        private void SetReferenceToExtension(GameObject inGO)
        {
            var pmfsms = new List<PlayMakerFSM>();

            // Canvas itself
            var comps = inGO.GetComponents<PlayMakerFSM>();
            if (comps.Length > 0)
            {
                pmfsms.AddRange(comps);
            }
            
            // Views under Canvas
            foreach (Transform child in inGO.transform)
            {
                comps = child.GetComponents<PlayMakerFSM>();
                if (comps.Length > 0)
                {
                    pmfsms.AddRange(comps);
                }
            }

            pmfsms.ForEach(x => SetFsmVariableValue(x, "zz_Command Service", RefBootstrap));
            pmfsms.Clear();
        }

        // TODO: Make this in some static class so that other class can access as well
        private static void SetFsmVariableValue(PlayMakerFSM pmfsm, string variableName, GameObject inValue)
        {
            var commandServiceVariables =
                pmfsm.FsmVariables.GameObjectVariables.Where(x => string.CompareOrdinal(x.Name, variableName) == 0);
                
            commandServiceVariables.ToList()
                .ForEach(x =>
                {
                    x.Value = inValue;
                });
        }        

        protected override void OnUpdate() {}

        public void RemovingAssets()
        {
            //
            if (_canvasPrefab != null)
            {
                Addressables.ReleaseInstance(_canvasPrefab);
            }

            if (_viewLoadingPrefab != null)
            {
                Addressables.ReleaseInstance(_viewLoadingPrefab);
            }

            if (_timelineAsset != null)
            {
                Addressables.Release(_timelineAsset);
            }

            if (_i2Asset != null)
            {
                Addressables.Release(_i2Asset);
            }

            //
            if (_canvas != null)
            {
                RemoveCommandStreamAndInfoStream(_canvas);
                GameObject.Destroy(_canvas);
            }            
        }
        
        private void RemoveCommandStreamAndInfoStream(GameObject inGO)
        {
            var commandStreamProducer = inGO.GetComponent<GameCommand.ICommandStreamProducer>();
            if (commandStreamProducer != null)
            {
                CommandService.RemoveCommandStreamProducer(commandStreamProducer);
            }

            var infoPresenter = inGO.GetComponent<GameCommand.IInfoPresenter>();
            if (infoPresenter != null)
            {
                CommandService.RemoveInfoStreamPresenter(infoPresenter);
            }            
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            RemovingAssets();
            
            _compositeDisposable?.Dispose();
        }
    }
}
