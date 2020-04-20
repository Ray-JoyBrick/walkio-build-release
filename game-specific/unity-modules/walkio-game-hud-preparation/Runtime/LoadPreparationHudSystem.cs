namespace JoyBrick.Walkio.Game.Hud.Preparation
{
    using System;
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
        public GameCommon.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            base.OnCreate();

            //
            FlowControl.LoadingAsset
                .Where(x => x.Name.Contains("Preparation"))
                .Subscribe(x =>
                {
                    LoadingAsset();
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
                    SetReferenceToExtension(_canvas);
                            
                    //
                    FlowControl.FinishLoadingAsset(new GameCommon.FlowControlContext
                    {
                        Name = "Preparation"
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

        // TODO: Move hard reference to PlayMakerFSM to somewhere else
        // TODO: Assign reference to FSM may need a better approach
        private void SetReferenceToExtension(GameObject inGO)
        {
            var pmfsm = inGO.GetComponent<PlayMakerFSM>();
            if (pmfsm != null)
            {
                pmfsm.FsmVariables.GameObjectVariables.ToList()
                    .ForEach(x =>
                    {
                        //
                        if (string.CompareOrdinal(x.Name, "CommandService") == 0)
                        {
                            // x.Value = 
                        }
                    });
            }
        }

        protected override void OnUpdate() {}
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

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

            //
            _compositeDisposable?.Dispose();
        }
    }
}
