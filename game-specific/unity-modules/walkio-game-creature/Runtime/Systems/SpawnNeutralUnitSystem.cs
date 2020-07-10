namespace JoyBrick.Walkio.Game.Creature
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class SpawnNeutralUnitSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SpawnNeutralUnitSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _theEnvironmentQuery;

        //
        private float _countDown = 5.0f;

        private Entity _prefabEntity;

        //
        private bool _canUpdate;
        
        //
        public GameCommon.IFlowControl FlowControl { get; set; }
        public GameCommon.IEcsSettingProvider EcsSettingProvider { get; set; }
        public GameObject NeutralUnitPrefab { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"SpawnNeutralUnitSystem - Construct");
            // var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            // _prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(neutralUnitPrefab, settings);
            
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"SpawnNeutralUnitSystem - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);

            // Observable.Timer(System.TimeSpan.FromSeconds(3))
            //     .Subscribe(x =>
            //     {
            //         _logger.Debug($"SpawnNeutralUnitSystem - Construct - Receive AllDoneSettingAsset");
            //         _canUpdate = true;
            //     })
            //     .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            //
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            
            // _theEnvironmentQuery = GetEntityQuery(new EntityQueryDesc
            // {
            //     All = new ComponentType[] { typeof(TheEnvironment) }
            // });
            //
            // // var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            // // GameObjectConversionUtility.ConvertGameObjectHierarchy(neutralUnitPrefab, settings);
            //
            // RequireForUpdate(_theEnvironmentQuery);
        }
        
        // private void CreateNeutralForceUnit(
        //     EntityCommandBuffer entityCommandBuffer,
        //     Entity prefab)
        // {
        //     // This should be converted to entity automatically
        //     
        //     var entity = _theEnvironmentQuery.GetSingletonEntity();
        //     var levelWaypointPathLookup = EntityManager.GetComponentData<LevelWaypointPathLookup>(entity);
        //
        //     var pathCount = levelWaypointPathLookup.WaypointPathBlobAssetRef.Value.WaypointPathIndexPairs.Length;
        //     var rnd = new Unity.Mathematics.Random((uint)System.DateTime.UtcNow.Ticks);
        //
        //     var randomIndex = rnd.NextInt(0, pathCount);
        //     var waypointPathIndexPair = levelWaypointPathLookup.WaypointPathBlobAssetRef.Value.WaypointPathIndexPairs[randomIndex];
        //
        //     var startingPosition =
        //         levelWaypointPathLookup.WaypointPathBlobAssetRef.Value.Waypoints[waypointPathIndexPair.StartIndex];
        //     // Debug.Log($"waypoint pos: {startingPosition} start: {waypointPathIndexPair.StartIndex} end: {waypointPathIndexPair.EndIndex}");
        //
        //     // var neutralForceAuthoring = prefab.GetComponent<UnitAuthoring>();
        //     // if (neutralForceAuthoring != null)
        //     // {
        //     //     neutralForceAuthoring.startPathIndex = waypointPathIndexPair.StartIndex;
        //     //     neutralForceAuthoring.endPathIndex = waypointPathIndexPair.EndIndex;
        //     //     neutralForceAuthoring.startingPosition = startingPosition;
        //     // }
        //     //
        //     // GameObject.Instantiate(prefab);
        //     
        //     // var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        //     // GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
        //     
        //     var ent = entityCommandBuffer.Instantiate(prefab);
        //     // var startingPosition = new float3(
        //     //     UnityEngine.Random.Range(0, 20.0f),
        //     //     0,
        //     //     UnityEngine.Random.Range(0, 20.0f));
        //     //
        //     entityCommandBuffer.SetComponent(ent, new MoveOnWaypointPath
        //     {
        //         StartPathIndex = waypointPathIndexPair.StartIndex,
        //         EndPathIndex = waypointPathIndexPair.EndIndex,
        //             
        //         AtIndex = waypointPathIndexPair.StartIndex
        //     });
        //     entityCommandBuffer.SetComponent(ent, new Translation
        //     {
        //         Value = (float3)startingPosition
        //     });
        // }
        private void CreateNeutralForceUnit(
            EntityCommandBuffer entityCommandBuffer,
            Translation translation,
            Entity prefab)
        {
            var createdEntity = entityCommandBuffer.Instantiate(prefab);
            var startingPosition = new float3(
                UnityEngine.Random.Range(-14.0f, 14.0f),
                0,
                UnityEngine.Random.Range(-14.0f, 14.0f));
            // entityCommandBuffer.SetComponent(createdEntity, new Translation
            // {
            //     Value = (float3)startingPosition
            // });
            entityCommandBuffer.SetComponent(createdEntity, new Translation
            {
                Value = translation.Value
            });
            
            entityCommandBuffer.AddComponent(createdEntity, new MakeEntityPlaceholder());
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            //
            // var environmentEntity = _theEnvironmentQuery.GetSingletonEntity();
            // var levelWaypointPathLookup = EntityManager.GetComponentData<LevelWaypointPathLookup>(environmentEntity);
            // var neutralUnitSpawn = EntityManager.GetComponentData<NeutralUnitSpawn>(environmentEntity);

            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            //
            var deltaTime = Time.DeltaTime;

            if (_prefabEntity == Entity.Null)
            {
                _prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(NeutralUnitPrefab, EcsSettingProvider.GameObjectConversionSettings);
            }

            var prefabEntity = _prefabEntity;
            
            // _logger.Debug($"SpawnNeutralUnitSystem - OnUpdate - prefab entity assigned");

            //
            Entities
                .WithAll<NeutralUnitSpawn>()
                .ForEach((Entity entity, Translation translation, ref NeutralUnitSpawnProperty neutralUnitSpawnProperty) =>
                {
                    var elapsedTime = neutralUnitSpawnProperty.CountDown + deltaTime;

                    neutralUnitSpawnProperty.CountDown = elapsedTime;

                    if (elapsedTime >= neutralUnitSpawnProperty.IntervalMax)
                    {
                        neutralUnitSpawnProperty.CountDown = 0;

                        // CreateNeutralForceUnit(neutralUnitPrefab);
                        CreateNeutralForceUnit(commandBuffer, translation, prefabEntity);
                    }
                    
                    commandBuffer.SetComponent(entity, neutralUnitSpawnProperty);
                })
                .WithoutBurst()
                .Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
