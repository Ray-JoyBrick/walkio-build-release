namespace JoyBrick.Walkio.Game.Environment
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
        private EntityArchetype _entityArchetype;

        //
        public GameCommand.ICommandService CommandService { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }

        // Although Construct called before OnCreate, waiting for observable stream will occur way
        // after OnCreate being called. And OnCreate should be called just once, observable stream
        // might be triggered many times during gameplay.
        public void Construct()
        {
            _logger.Debug($"LoadEnvironmentSystem - Construct");
            
            base.OnCreate();
            
            //
            FlowControl.LoadingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    // Create Singleton Entity first
                    var theEnvironmentEntity = EntityManager.CreateEntity(_entityArchetype);
                    
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

                    var levelSetting = _levelSettingAsset as LevelSetting;
                    if (levelSetting != null)
                    {
                        // var desc =
                        //     levelSetting.gridTextures.Aggregate("", (acc, next) => $"{acc} {next}");
                        // _logger.Debug($"LoadEnvironmentSystem - LoadingAsset\n{desc}");
                        levelSetting.gridTextures.ForEach(gt =>
                        {
                            AssignDataFromTexture(gt);
                        });
                    }
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

        private void AssignDataFromTexture(Texture2D texture2D)
        {
            var width = texture2D.width;
            var height = texture2D.height;
            var array = texture2D.GetRawTextureData<Color32>();
            // var array = texture2D.GetRawTextureData<byte>();
            _logger.Debug($"texture length: {array.Length} format: {texture2D.format} width: {width} height: {height}");
            
            for (var i = 0; i < array.Length; ++i)
            {
                var color = array[i];
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _entityArchetype = EntityManager.CreateArchetype(
                 typeof(TheEnvironment),
                 typeof(GameCommon.StageUse));
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
