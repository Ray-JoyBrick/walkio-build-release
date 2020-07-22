namespace JoyBrick.Walkio.Game.Environment
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pathfinding;
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

        private SceneInstance _sceneInstance;

        //
        private EntityArchetype _entityArchetype;

        //
        public GameCommand.ICommandService CommandService { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }
        
        //
        public ScriptableObject LevelSettingAsset => _levelSettingAsset;

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
                    (_levelSettingAsset, _sceneInstance) = result;

                    //
                    var levelSetting = _levelSettingAsset as LevelSetting;
                    if (levelSetting != null)
                    {
                        // var desc =
                        //     levelSetting.gridTextures.Aggregate("", (acc, next) => $"{acc} {next}");
                        // _logger.Debug($"LoadEnvironmentSystem - LoadingAsset\n{desc}");
                        // levelSetting.gridTextures.ForEach(gt =>
                        // {
                        //     AssignDataFromTexture(gt);
                        // });

                        var gridTileCount2D = new Vector2Int(levelSetting.hGridCount, levelSetting.vGridCount);
                        var gridTileCellCount2D = new Vector2Int(levelSetting.gridCellCount, levelSetting.gridCellCount);
                        SetupGridMap(levelSetting.gridMapAuthoringPrefab, levelSetting.gridTextures, gridTileCount2D, gridTileCellCount2D);

                        SetupPathfinder(_sceneInstance.Scene, levelSetting.astarGraphDatas.First());
                    }
                    // //
                    // var wpbaaPrefab = _waypointPathBlobAssetAuthoringPrefab.GetComponent<WaypointPathBlobAssetAuthoring>();
                    // wpbaaPrefab.waypointDataAsset = _waypointDataAsset as WaypointData;
                    // GameObject.Instantiate(_waypointPathBlobAssetAuthoringPrefab);
                    GameObject.Instantiate(levelSetting.waypointPathAuthoringPrefab);
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

        private async Task<SceneInstance> GetScene(string addressName)
        {
            var handle = Addressables.LoadSceneAsync(addressName, LoadSceneMode.Additive);
            var r = await handle.Task;

            return r;
        }

        private async Task<(ScriptableObject, SceneInstance)> Load()
        {
            var levelSettingAssetTask = GetAsset<ScriptableObject>($"Level Setting.asset");
            var levelMainSceneAssetTask = GetScene("Level 001");

            var levelSettingAsset = await levelSettingAssetTask;
            var sceneInstance = await levelMainSceneAssetTask;

            return (levelSettingAsset, sceneInstance);
        }

        // private async Task<(ScriptableObject, GameObject, ScriptableObject, GameObject, SceneInstance)> Load()
        // {
        //     var levelSettingAssetTask = GetAsset<ScriptableObject>($"Level Setting.asset");
        //     var levelSettingBlobAssetAuthoringTask = GetAsset<GameObject>($"Level Setting BlobAsset Authoring");
        //     var waypointDataAssetTask = GetAsset<ScriptableObject>($"Waypoint Data.asset");
        //     var waypointPathBlobAssetAuthoringTask = GetAsset<GameObject>($"Waypoint Path BlobAsset Authoring");
        //
        //     var levelMainSceneAssetTask = GetScene("Level 001");
        //
        //     var (levelSettingAsset, levelSettingBlobAssetAuthoring, waypointDataAsset, waypointPathBlobAssetAuthoring) =
        //         (await levelSettingAssetTask, await levelSettingBlobAssetAuthoringTask, await waypointDataAssetTask, await waypointPathBlobAssetAuthoringTask);
        //
        //     var sceneInstance = await levelMainSceneAssetTask;
        //
        //     return (levelSettingAsset, levelSettingBlobAssetAuthoring, waypointDataAsset, waypointPathBlobAssetAuthoring, sceneInstance);
        // }

        private void SetupGridMap(GameObject prefab, IList<Texture2D> texture2Ds, Vector2Int gridTileCount2D, Vector2Int gridTileCellCount2D)
        {
            var gridCells =
                Utility.GridHelper.GetGridObstacleIndicesFromTextures(texture2Ds, gridTileCount2D, gridTileCellCount2D);

            var authoring = prefab.GetComponent<GridMapBlobAssetAuthoring>();
            if (authoring != null)
            {
                authoring.gridCells = gridCells;

                GameObject.Instantiate(authoring);
            }
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

        private void SetupPathfinder(Scene scene, TextAsset textAsset)
        {
            _logger.Debug($"LoadEnvironmentSystem - SetupPathfinder - {scene.name}, {textAsset.bytes.Length}");
            
            // astarPath.data.SetData(textAsset.bytes);
            // var astarPath = GetComponentAtScene<AstarPath>(scene);
            // if (astarPath != null)
            // {
            //     _logger.Debug($"LoadEnvironmentSystem - SetupPathfinder - Found {astarPath}");
            //     // astarPath.data.SetData(textAsset.bytes);
            //     astarPath.ac.data.SetData(textAsset.bytes);
            // }
            AstarPath.active.data.DeserializeGraphs(textAsset.bytes);
        }
        
        // // TODO: This is the third time this function is being copy/paste, create utility to merge them
        // private static T GetComponentAtScene<T>(Scene scene) where T : VersionedMonoBehaviour
        // {
        //     T comp = default;

        //     if (!scene.IsValid()) return comp;

        //     var foundGOs =
        //         scene.GetRootGameObjects()
        //             .Where(x => x.GetComponent<T>() != null)
        //             .ToList();

        //     if (foundGOs != null && foundGOs.Any())
        //     {
        //         var foundGO = foundGOs.First();
        //         comp = foundGO.GetComponent<T>();
        //     }

        //     return comp;
        // }

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
            _logger.Debug($"LoadEnvironmentSystem - RemovingAssets");

            //
            if (_levelSettingAsset != null)
            {
                Addressables.Release(_levelSettingAsset);
            }
            
            // if (_levelSettingBlobAssetAuthoringPrefab != null)
            // {
            //     Addressables.ReleaseInstance(_levelSettingBlobAssetAuthoringPrefab);
            // }

            // if (_waypointDataAsset != null)
            // {
            //     Addressables.Release(_waypointDataAsset);
            // }
            
            // if (_waypointPathBlobAssetAuthoringPrefab != null)
            // {
            //     Addressables.ReleaseInstance(_waypointPathBlobAssetAuthoringPrefab);
            // }

            // This async process flow won't start
            var asyncOperation =
                SceneManager.UnloadSceneAsync(_sceneInstance.Scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

            asyncOperation.AsObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    _logger.Debug($"LoadEnvironmentSystem - RemovingAssets - Scene is unloaded");

                    var asyncOperation2 = Addressables.UnloadSceneAsync(_sceneInstance);
                    asyncOperation2.ToObservable()
                        .ObserveOnMainThread()
                        .SubscribeOnMainThread()
                        .Subscribe(x =>
                        {
                            //
                            _logger.Debug($"LoadEnvironmentSystem - RemovingAssets - Scene instance is unloaded");

                        })
                        .AddTo(_compositeDisposable);
                })
                .AddTo(_compositeDisposable);
        }

        // private async Task<(ScriptableObject, GameObject, ScriptableObject, GameObject, SceneInstance)> Unload()
        // {
        //     var asyncOperation =
        //         SceneManager.UnloadSceneAsync(_sceneInstance.Scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        //
        //     asyncOperation.AsObservable()
        // }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            RemovingAssets();
            
            _compositeDisposable?.Dispose();
        }
    }
}
