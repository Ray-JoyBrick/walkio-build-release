namespace JoyBrick.Walkio.Game.Hud.Stage
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
    
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameExtension = JoyBrick.Walkio.Game.Extension;
    
    [DisableAutoCreation]
    public class LoadStageHudSystem : SystemBase
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
        public GameObject RefBootstrap { get; set; }
        public GameCommand.ICommandService CommandService { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            base.OnCreate();
            
            //
            FlowControl.LoadingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    LoadingAsset();
                })
                .AddTo(_compositeDisposable);
            
            FlowControl.CleaningAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    RemovingAssets();
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
                    // _canvasPrefab = result;
                    (_canvasPrefab, _viewLoadingPrefab, _timelineAsset, _i2Asset) = result;
                            
                    //
                    _canvas = GameObject.Instantiate(_canvasPrefab);
                    AddCommandStreamAndInfoStream(_canvas);
                    SetReferenceToExtension(_canvas);
                            
                    //
                    FlowControl.FinishLoadingAsset(new GameCommon.FlowControlContext
                    {
                        Name = "Stage"
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
        
        private async Task<(GameObject, GameObject, ScriptableObject, ScriptableObject)> Load()
        {
            var canvasPrefabTask = GetAsset<GameObject>($"Hud - Canvas - Stage");
            var viewLoadingPrefabTask = GetAsset<GameObject>($"Hud - Stage - View - Base Prefab");
            var timelineAssetTask = GetAsset<ScriptableObject>($"Hud - Stage - View - Base Timeline");
            var i2AssetTask = GetAsset<ScriptableObject>($"Hud - Stage - I2");

            var (canvasPrefab, viewLoadingPrefab, timelineAsset, i2Asset) =
                (await canvasPrefabTask, await viewLoadingPrefabTask, await timelineAssetTask, await i2AssetTask);

            return (canvasPrefab, viewLoadingPrefab, timelineAsset, i2Asset);
        }        
        
        // private async Task<GameObject> Load()
        // {
        //     var addressName = $"Hud - Stage";
        //     var handle = Addressables.LoadAssetAsync<GameObject>(addressName);
        //
        //     var r = await handle.Task;
        //
        //     return r;
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
                GameObject.Destroy(_canvas);
            }            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            //
            RemovingAssets();

            _compositeDisposable?.Dispose();
        }
    }
}
