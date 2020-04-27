namespace JoyBrick.Walkio.Game.GameFlowControl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.SceneManagement;
    using GameCommon = Walkio.Game.Common;
    
    [DisableAutoCreation]
    public class LoadGameFlowSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadGameFlowSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        private GameObject _flowPrefab;

        private GameObject _flow;
        
        //
        public GameObject RefBootstrap { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"LoadGameFlowSystem - Construct");
            
            base.OnCreate();
            
            //
            FlowControl.LoadingAsset
                .Where(x => x.Name.Contains("App"))
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
                    _flowPrefab = result;
                            
                    //
                    _flow = GameObject.Instantiate(_flowPrefab);
                    var scene = SceneManager.GetSceneByName("Entry");
                    if (scene.IsValid())
                    {
                        SceneManager.MoveGameObjectToScene(_flow, scene);
                    }

                    SetReferenceToExtension(_flow);
                    // AddCommandStreamAndInfoStream(_canvas);
                    //
                    // ExtractView();
                            
                    //
                    FlowControl.FinishLoadingAsset(new GameCommon.FlowControlContext
                    {
                        Name = "App"
                    });
                })
                .AddTo(_compositeDisposable);            
        }

        private async Task<GameObject> Load()
        {
            var addressName = "Game Flow";
            var handle = Addressables.LoadAssetAsync<GameObject>(addressName);
            var r = await handle.Task;
        
            return r;
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
            pmfsms.ForEach(x => SetFsmVariableValue(x, "zz_Flow Service", RefBootstrap));
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
            if (_flowPrefab != null)
            {
                Addressables.ReleaseInstance(_flowPrefab);
            }

            //
            if (_flow != null)
            {
                GameObject.Destroy(_flow);
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
