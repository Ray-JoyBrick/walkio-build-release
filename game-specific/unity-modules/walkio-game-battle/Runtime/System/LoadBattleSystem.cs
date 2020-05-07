namespace JoyBrick.Walkio.Game.Battle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameExtension = JoyBrick.Walkio.Game.Extension;
    
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;

    [DisableAutoCreation]
    public class LoadBattleSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadBattleSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private GameObject _battleUsePoolPrefab;
        private GameObject _teamForceSetPrefab;
        private GameObject _teamForceUnitPrefab;
        private GameObject _neutralForceUnitPrefab;

        //
        private GameObject _battleUsePool;
        private EntityQuery _theEnvironmentQuery;

        //
        public GameObject RefBootstrap { get; set; }
        public GameCommand.ICommandService CommandService { get; set; }
        // public GameCommand.IInfoPresenter InfoPresenter { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"LoadBattleSystem - Construct");
            
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

            // //
            // CommandService.CommandStream
            //     .Do(x => _logger.Debug($"Receive Command Stream: {x}"))
            //     .Where(x => (x as GameCommand.ActivateLoadingViewCommand) != null)
            //     .Subscribe(x =>
            //     {
            //         _logger.Debug($"LoadAppHudSystem - Construct - Receive ActivateLoadingViewCommand");
            //         var activateLoadingViewCommand = (x as GameCommand.ActivateLoadingViewCommand);
            //         //
            //         ActivateLoadingView(activateLoadingViewCommand.Flag);
            //     })
            //     .AddTo(_compositeDisposable);
            
            CommandService.CommandStream
                .Where(x => (x as GameCommand.CreateNeutralForceUnit) != null)
                .Subscribe(x =>
                {
                    _logger.Debug($"LoadBattleSystem - Construct - Receive CreateNeutralForceUnit");
                    CreateNeutralForceUnit(_neutralForceUnitPrefab);

                })
                .AddTo(_compositeDisposable);

            // Might be better to create another system and leave this system simply responsible for creation
            CommandService.CommandStream
                .Where(x => (x as GameCommand.PlaceTeamForceLeader) != null)
                .Subscribe(x =>
                {
                    _logger.Debug($"LoadBattleSystem - Construct - Receive PlaceTeamForceLeader");
                    PlaceTeamForceLeader();
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
                    (_battleUsePoolPrefab, _teamForceSetPrefab, _teamForceUnitPrefab, _neutralForceUnitPrefab) = result;
                            
                    //
                    _battleUsePool = GameObject.Instantiate(_battleUsePoolPrefab);
                    var scene = SceneManager.GetSceneByName("Entry");
                    if (scene.IsValid())
                    {
                        SceneManager.MoveGameObjectToScene(_battleUsePool, scene);
                    }
                    
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
        
        private async Task<(GameObject, GameObject, GameObject, GameObject)> Load()
        {
            var battleUsePoolPrefabTask = GetAsset<GameObject>($"Battle Use Pool");
            var teamForceSetPrefabTask = GetAsset<GameObject>($"Team Force Set");
            var teamForceUnitPrefabTask = GetAsset<GameObject>($"Team Force Unit");
            var neutralForceUnitPrefabTask = GetAsset<GameObject>($"Neutral Force Unit");

            var (battleUsePoolPrefab, teamForceSetPrefab, teamForceUnitPrefab, neutralForceUnitPrefab) =
                (await battleUsePoolPrefabTask, await teamForceSetPrefabTask, await teamForceUnitPrefabTask, await neutralForceUnitPrefabTask);

            return (battleUsePoolPrefab, teamForceSetPrefab, teamForceUnitPrefab, neutralForceUnitPrefab);
        }

        private void CreateNeutralForceUnit(GameObject prefab)
        {
            // This should be converted to entity automatically
            
            var entity = _theEnvironmentQuery.GetSingletonEntity();
            var levelWaypointPathLookup = EntityManager.GetComponentData<GameEnvironment.LevelWaypointPathLookup>(entity);

            var pathCount = levelWaypointPathLookup.WaypointPathBlobAssetRef.Value.WaypointPaths.Length;
            var rnd = new Unity.Mathematics.Random((uint)System.DateTime.UtcNow.Ticks);

            var randomIndex = rnd.NextInt(0, pathCount);
            var waypointPath = levelWaypointPathLookup.WaypointPathBlobAssetRef.Value.WaypointPaths[randomIndex];

            var startingPosition =
                levelWaypointPathLookup.WaypointPathBlobAssetRef.Value.Waypoints[waypointPath.StartIndex];
            // var adjustedStartingPosition = new float3(startingPosition.x, 10.0f, startingPosition.z);
            // var adjustedStartingPosition = new float3(startingPosition.x, 0.0f, startingPosition.z);
            // var adjustedStartingPosition = new float3(5.0f, 2.0f, 5.0f);
            Debug.Log($"waypoint pos: {startingPosition} start: {waypointPath.StartIndex} end: {waypointPath.EndIndex}");

            var neutralForceAuthoring = prefab.GetComponent<NeutralForceAuthoring>();
            if (neutralForceAuthoring != null)
            {
                neutralForceAuthoring.startPathIndex = waypointPath.StartIndex;
                neutralForceAuthoring.endPathIndex = waypointPath.EndIndex;
                neutralForceAuthoring.startingPosition = startingPosition;
                // neutralForceAuthoring.startingPosition = adjustedStartingPosition;
            }
            
            GameObject.Instantiate(prefab);
        }

        private void PlaceTeamForceLeader()
        {
            _logger.Debug($"LoadBattleSystem - Construct - PlaceTeamForceLeader");

            // At this time, should safely assumed that there exists level settings to be used
            // _battleUsePool.GetComponent<Pool>()

            var loadEnvironmentSystem = World.GetExistingSystem<GameEnvironment.LoadEnvironmentSystem>();
            var levelSetting = loadEnvironmentSystem.LevelSettingAsset as GameEnvironment.LevelSetting;

            var aiControlCount = levelSetting.aiControlCount;
            for (var i = 0; i < aiControlCount; ++i)
            {
                _logger.Debug($"LoadBattleSystem - Construct - PlaceTeamForceLeader - Create ai: {i}");
                CommandService?.SendCommand("Create Team Leader From Pool");
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _theEnvironmentQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(GameEnvironment.TheEnvironment) }
            });
            
            RequireForUpdate(_theEnvironmentQuery);
        }

        protected override void OnUpdate() {}

        public void RemovingAssets()
        {
            //
            if (_battleUsePoolPrefab != null)
            {
                Addressables.ReleaseInstance(_battleUsePoolPrefab);
            }
            
            if (_neutralForceUnitPrefab != null)
            {
                Addressables.ReleaseInstance(_neutralForceUnitPrefab);
            }
            
            // if (_viewLoadingPrefab != null)
            // {
            //     Addressables.ReleaseInstance(_viewLoadingPrefab);
            // }
            //
            // if (_timelineAsset != null)
            // {
            //     Addressables.Release(_timelineAsset);
            // }
            //
            // if (_i2Asset != null)
            // {
            //     Addressables.Release(_i2Asset);
            // }
            
            //
            if (_battleUsePool != null)
            {
                GameObject.Destroy(_battleUsePool);
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
