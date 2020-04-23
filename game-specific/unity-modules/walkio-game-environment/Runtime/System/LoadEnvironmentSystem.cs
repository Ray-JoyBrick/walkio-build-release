namespace JoyBrick.Walkio.Game.Environment
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
    // using GameExtension = JoyBrick.Walkio.Game.Extension;

    [DisableAutoCreation]
    public class LoadEnvironmentSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadEnvironmentSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private ScriptableObject _waypointDataAsset;
        private GameObject _waypointPathBlobAssetAuthoringPrefab;

        //
        public GameCommand.ICommandService CommandService { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"LoadEnvironmentSystem - Construct");
            
            base.OnCreate();
            
            //
            FlowControl.LoadingAsset
                .Where(x => x.Name.Contains("Stage"))
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
                    (_waypointDataAsset, _waypointPathBlobAssetAuthoringPrefab) = result;
                            
                    // //
                    GameObject.Instantiate(_waypointPathBlobAssetAuthoringPrefab);
                    // _canvas = GameObject.Instantiate(_canvasPrefab);
                    // AddCommandStreamAndInfoStream(_canvas);

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
        
        private async Task<(ScriptableObject, GameObject)> Load()
        {
            var waypointDataAssetTask = GetAsset<ScriptableObject>($"Waypoint Data");
            var waypointPathBlobAssetAuthoringTask = GetAsset<GameObject>($"Waypoint Path BlobAsset Authoring");

            var (waypointDataAsset, waypointPathBlobAssetAuthoring) =
                (await waypointDataAssetTask, await waypointPathBlobAssetAuthoringTask);

            return (waypointDataAsset, waypointPathBlobAssetAuthoring);
        }

        protected override void OnUpdate() {}

        public void RemovingAssets()
        {
            //
            if (_waypointDataAsset != null)
            {
                Addressables.Release(_waypointDataAsset);
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
