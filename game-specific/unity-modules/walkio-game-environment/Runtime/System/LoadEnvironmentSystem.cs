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
        private ScriptableObject _levelSettingAsset;
        private GameObject _levelSettingBlobAssetAuthoringPrefab;
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
                    (_levelSettingAsset, _levelSettingBlobAssetAuthoringPrefab, _waypointDataAsset, _waypointPathBlobAssetAuthoringPrefab) = result;
                            
                    // //
                    // var wpbaaPrefab = _waypointPathBlobAssetAuthoringPrefab.GetComponent<WaypointPathBlobAssetAuthoring>();
                    // wpbaaPrefab.waypointDataAsset = _waypointDataAsset as WaypointData;
                    GameObject.Instantiate(_waypointPathBlobAssetAuthoringPrefab);
                    // GameObject.Instantiate(wpbaaPrefab);
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
        
        private async Task<(ScriptableObject, GameObject, ScriptableObject, GameObject)> Load()
        {
            var levelSettingAssetTask = GetAsset<ScriptableObject>($"Level Setting.asset");
            var levelSettingBlobAssetAuthoringTask = GetAsset<GameObject>($"Level Setting BlobAsset Authoring");
            var waypointDataAssetTask = GetAsset<ScriptableObject>($"Waypoint Data.asset");
            var waypointPathBlobAssetAuthoringTask = GetAsset<GameObject>($"Waypoint Path BlobAsset Authoring");

            var (levelSettingAsset, levelSettingBlobAssetAuthoring, waypointDataAsset, waypointPathBlobAssetAuthoring) =
                (await levelSettingAssetTask, await levelSettingBlobAssetAuthoringTask, await waypointDataAssetTask, await waypointPathBlobAssetAuthoringTask);

            return (levelSettingAsset, levelSettingBlobAssetAuthoring, waypointDataAsset, waypointPathBlobAssetAuthoring);
        }

        protected override void OnUpdate() {}

        public void RemovingAssets()
        {
            //
            if (_levelSettingAsset != null)
            {
                Addressables.Release(_levelSettingAsset);
            }
            
            if (_levelSettingBlobAssetAuthoringPrefab != null)
            {
                Addressables.ReleaseInstance(_levelSettingBlobAssetAuthoringPrefab);
            }

            if (_waypointDataAsset != null)
            {
                Addressables.Release(_waypointDataAsset);
            }
            
            if (_waypointPathBlobAssetAuthoringPrefab != null)
            {
                Addressables.ReleaseInstance(_waypointPathBlobAssetAuthoringPrefab);
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
