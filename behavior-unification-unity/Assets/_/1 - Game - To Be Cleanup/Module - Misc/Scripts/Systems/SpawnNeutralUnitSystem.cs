namespace Game
{
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;
    using UnityEngine;

    [DisableAutoCreation]
    public class SpawnNeutralUnitSystem : SystemBase
    {
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _theEnvironmentQuery;
        
        //
        private float _countDown = 5.0f;

        private Entity _prefabEntity;

        public GameObject neutralUnitPrefab { get; set; }
        
        public void Construct()
        {
            // var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            // _prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(neutralUnitPrefab, settings);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            _theEnvironmentQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(TheEnvironment) }
            });

            // var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            // GameObjectConversionUtility.ConvertGameObjectHierarchy(neutralUnitPrefab, settings);

            RequireForUpdate(_theEnvironmentQuery);
        }
        
        // private void CreateNeutralForceUnit(
            //GameObject prefab)
        private void CreateNeutralForceUnit(
            EntityCommandBuffer entityCommandBuffer,
            Entity prefab)
        {
            // This should be converted to entity automatically
            
            var entity = _theEnvironmentQuery.GetSingletonEntity();
            var levelWaypointPathLookup = EntityManager.GetComponentData<LevelWaypointPathLookup>(entity);

            var pathCount = levelWaypointPathLookup.WaypointPathBlobAssetRef.Value.WaypointPathIndexPairs.Length;
            var rnd = new Unity.Mathematics.Random((uint)System.DateTime.UtcNow.Ticks);

            var randomIndex = rnd.NextInt(0, pathCount);
            var waypointPathIndexPair = levelWaypointPathLookup.WaypointPathBlobAssetRef.Value.WaypointPathIndexPairs[randomIndex];

            var startingPosition =
                levelWaypointPathLookup.WaypointPathBlobAssetRef.Value.Waypoints[waypointPathIndexPair.StartIndex];
            // Debug.Log($"waypoint pos: {startingPosition} start: {waypointPathIndexPair.StartIndex} end: {waypointPathIndexPair.EndIndex}");

            // var neutralForceAuthoring = prefab.GetComponent<UnitAuthoring>();
            // if (neutralForceAuthoring != null)
            // {
            //     neutralForceAuthoring.startPathIndex = waypointPathIndexPair.StartIndex;
            //     neutralForceAuthoring.endPathIndex = waypointPathIndexPair.EndIndex;
            //     neutralForceAuthoring.startingPosition = startingPosition;
            // }
            //
            // GameObject.Instantiate(prefab);
            
            // var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            // GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
            
            var ent = entityCommandBuffer.Instantiate(prefab);
            // var startingPosition = new float3(
            //     UnityEngine.Random.Range(0, 20.0f),
            //     0,
            //     UnityEngine.Random.Range(0, 20.0f));
            //
            entityCommandBuffer.SetComponent(ent, new MoveOnWaypointPath
            {
                StartPathIndex = waypointPathIndexPair.StartIndex,
                EndPathIndex = waypointPathIndexPair.EndIndex,
                    
                AtIndex = waypointPathIndexPair.StartIndex
            });
            entityCommandBuffer.SetComponent(ent, new Translation
            {
                Value = (float3)startingPosition
            });
        }

        protected override void OnUpdate()
        {
            //
            // var environmentEntity = _theEnvironmentQuery.GetSingletonEntity();
            // var levelWaypointPathLookup = EntityManager.GetComponentData<LevelWaypointPathLookup>(environmentEntity);
            // var neutralUnitSpawn = EntityManager.GetComponentData<NeutralUnitSpawn>(environmentEntity);

            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            //
            var deltaTime = Time.DeltaTime;

            if (_prefabEntity == Entity.Null)
            {
                var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, new BlobAssetStore());
                _prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(neutralUnitPrefab, settings);
            }

            var prefabEntity = _prefabEntity;

            //
            Entities
                .ForEach((Entity entity, NeutralUnitSpawn neutralUnitSpawn) =>
                {
                    var elapsedTime = neutralUnitSpawn.CountDown + deltaTime;

                    neutralUnitSpawn.CountDown = elapsedTime;

                    if (elapsedTime >= neutralUnitSpawn.IntervalMax)
                    {
                        neutralUnitSpawn.CountDown = 0;

                        // CreateNeutralForceUnit(neutralUnitPrefab);
                        CreateNeutralForceUnit(commandBuffer, prefabEntity);
                    }
                    
                    commandBuffer.SetComponent(entity, neutralUnitSpawn);
                })
                .WithoutBurst()
                .Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency); 
        }
    }
}
