namespace JoyBrick.Walkio.Game.Level
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pathfinding;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    // using GameCommand = JoyBrick.Walkio.Game.Command;

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

#if WALKIO_FLOWCONTROL
    [GameFlowControl.DoneLoadingAssetWait("Stage")]
#endif
    [DisableAutoCreation]
    public class LoadAssetSystem :
        SystemBase
        // GameCommon.ISystemContext
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadAssetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private ScriptableObject _levelDataAsset;
        private SceneInstance _sceneInstance;

        //
        private EntityArchetype _entityArchetype;
        private EntityQuery _gridMapBlobAssetConstructedEventEntityQuery;

        private enum WaitingEvent
        {
            GridMapBlobAssetConstructed
        }

        //
        private readonly List<int> _waitingEventForLoadingDone = new List<int>
        {
            0,
            0
        };

        //
        // public GameCommand.ICommandService CommandService { get; set; }
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        public IGridWorldProvider GridWorldProvider { get; set; }

        //
        public string AtPart => "Stage";

        //
        public bool ProvideExternalAsset { get; set; }

        // public ScriptableObject LevelSettingDataAsset
        // {
        //     get => _levelSettingDataAsset;
        //     set => _levelSettingDataAsset = value;
        // }
        //
        // public SceneInstance SceneInstance
        // {
        //     get => _sceneInstance;
        //     set => _sceneInstance = value;
        // }

        private async Task<(ScriptableObject, SceneInstance)> Load(string levelAssetName, string specificLevelName)
        //private async Task<ScriptableObject> Load(string levelAssetName, string specificLevelName)
        {
            // What to load is defined below with async task
            var levelDataAssetName = levelAssetName;
            var levelDataAssetTask = GameCommon.Utility.AssetLoadingHelper.GetAsset<ScriptableObject>(levelDataAssetName);

            var levelMainSceneAssetName = specificLevelName;
            var levelMainSceneAssetTask = GameCommon.Utility.AssetLoadingHelper.GetScene(levelMainSceneAssetName);
            //
            var levelSettingAsset = await levelDataAssetTask;
            var sceneInstance = await levelMainSceneAssetTask;

            return (levelSettingAsset, sceneInstance);
            // return levelSettingAsset;
        }

        private void SetupGridMap(
            GameObject prefab,
            Dictionary<Color32, int> lookupTable,
            IList<Texture2D> texture2Ds, Vector2Int gridTileCount2D,
            Vector2Int gridTileCellCount2D, Vector2 gridWorldCellSize)
        {
            var gridCells =
                Utility.GridWorldHelper.GetGridObstacleIndicesFromTextures(lookupTable, texture2Ds, gridTileCount2D, gridTileCellCount2D);

            var authoring = prefab.GetComponent<GridMapBlobAssetAuthoring>();
            if (authoring != null)
            {
                authoring.gridCellCount = gridTileCount2D * gridTileCellCount2D;
                authoring.gridCellSize = gridWorldCellSize;
                authoring.gridCells = gridCells;

                 _logger.Debug($"Module - LoadAssetSystem - SetupGridMap - grid cells assigned to authoring prefab");

                 // This may takes several secs to get ready
                GameObject.Instantiate(authoring);
            }
        }

        // private void SetupAStarPathfinder(Scene scene, TextAsset textAsset)
        // {
        //     _logger.Debug($"LoadAssetSystem - SetupAStarPathfinder - {scene.name}, {textAsset.bytes.Length}");
        //
        //     AstarPath.active.data.DeserializeGraphs(textAsset.bytes);
        // }

        private void MakeWorld(ScriptableObject levelSettingDataAsset, SceneInstance sceneInstance)
        {
            //
            var gridWorldEntity = EntityManager.CreateEntity(_entityArchetype);

#if UNITY_EDITOR
            EntityManager.SetName(gridWorldEntity, $"Grid World");
#endif
            //
            _logger.Debug($"Module - LoadAssetSystem - MakeWorld - load level data");
            var levelData = levelSettingDataAsset as Template.LevelData;
            if (levelData != null)
            {
                // Should read the value from data asset instead of hard code
                var gridWorldTileCount = levelData.gridWorldTileCount;
                var gridWorldTileCellCount = levelData.gridWorldTielCellCount;
                var gridWorldCellSize = levelData.gridWorldCellSize;
                var gridWorldData = GridWorldProvider.GridWorldData as Template.GridWorldData;

                var lookupTable =
                    levelData.areaLookup
                        .ToDictionary(x => x.areaColor, x => x.index);

                var prefab = gridWorldData.gridMapBlobAssetAuthoringPrefab;

                SetupGridMap(
                    prefab,
                    lookupTable, levelData.subLevelImages,
                    gridWorldTileCount, gridWorldTileCellCount, gridWorldCellSize);
            }

            // var fakeTextAsset = new TextAsset();
            // // SetupGridMap(gridWorldTileCount, gridWorldTileCellCount);
            // SetupAStarPathfinder(sceneInstance.Scene, fakeTextAsset);
        }

        private void InternalLoadAsset(
            string levelAssetName, string specificLevelName,
            System.Action loadingDoneAction)
        {

            Load(levelAssetName, specificLevelName).ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    //
                    (_levelDataAsset, _sceneInstance) = result;
                    // _levelDataAsset = result;

                    MakeWorld(_levelDataAsset, _sceneInstance);

                    loadingDoneAction();
                })
                .AddTo(_compositeDisposable);
        }

        private void LoadingAsset(string levelAssetName, string specificLevelName)
        {
            if (ProvideExternalAsset)
            {
                // Asset is provided from somewhere else, just notify that the asset loading is done
                FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                {
                    Name = "Stage"
                });
            }
            else
            {
                InternalLoadAsset(
                    levelAssetName, specificLevelName,
                    () =>
                    {
                        // Since internal loading might be very time consuming, after it is finished, it will
                        // send an event entity. This event entity is caught in Update and process further.
                        
                        // FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                        // {
                        //     Name = "Stage"
                        // });
                    });
            }
        }

        //
        public void Construct()
        {
            _logger.Debug($"LoadAssetSystem - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.AssetLoadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - LoadAssetSystem - Construct - Receive AssetLoadingStarted");
                    
                    // Hard code here, should be given in event
                    // var levelAssetName = $"Level Setting.asset";
                    // var specificLevelName = $"Level 001";
                    var levelAssetName = $"Level 001/Level Data";
                    var specificLevelName = $"Level 001/Main";
                    LoadingAsset(levelAssetName, specificLevelName);
                })
                .AddTo(_compositeDisposable);
#else
            Observable.Timer(System.TimeSpan.FromMilliseconds(400))
                .Subscribe(x =>
                {
                    var levelAssetName = $"Level 001/Level Data";
                    var specificLevelName = $"Level 001/Main";
                    LoadingAsset(levelAssetName, specificLevelName);
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            _logger.Debug($"LoadAssetSystem - OnCreate");

            base.OnCreate();

            _entityArchetype = EntityManager.CreateArchetype(
                typeof(GridWorld),
                typeof(GridWorldProperty));

            _gridMapBlobAssetConstructedEventEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<GridMapBlobAssetConstructed>()
                }
            });

            RequireForUpdate(_gridMapBlobAssetConstructedEventEntityQuery);
        }

        private static bool CheckAllEventArrived(List<int> eventSlots)
        {
            var notSetEvents = eventSlots.Where(x => x == 0);

            return !notSetEvents.Any();
        }

        private static void ResetWaitingEventSlots(List<int> eventSlots)
        {
            for (var i = 0; i < eventSlots.Count; ++i)
            {
                eventSlots[i] = 0;
            }
        }

        protected override void OnUpdate()
        {
            var gridMapBlobAssetConstructedEventEntity =  _gridMapBlobAssetConstructedEventEntityQuery.GetSingletonEntity();

            _waitingEventForLoadingDone[(int)WaitingEvent.GridMapBlobAssetConstructed] = 1;

            //
            var allEventSet = CheckAllEventArrived(_waitingEventForLoadingDone);
            if (allEventSet)
            {
                // Delete event entity
                EntityManager.DestroyEntity(gridMapBlobAssetConstructedEventEntity);
                // Delete UniRx event

                // Reset list
                ResetWaitingEventSlots(_waitingEventForLoadingDone);

                // Notify
                FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                {
                    Name = "Stage"
                });
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
