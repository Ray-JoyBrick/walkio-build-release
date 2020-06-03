namespace Game
{
    using Unity.Entities;
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

        public GameObject neutralUnitPrefab { get; set; }
        
        public void Construct()
        {
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            _theEnvironmentQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(TheEnvironment) }
            });
            
            RequireForUpdate(_theEnvironmentQuery);
        }
        
        private void CreateNeutralForceUnit(GameObject prefab)
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

            var neutralForceAuthoring = prefab.GetComponent<UnitAuthoring>();
            if (neutralForceAuthoring != null)
            {
                neutralForceAuthoring.startPathIndex = waypointPathIndexPair.StartIndex;
                neutralForceAuthoring.endPathIndex = waypointPathIndexPair.EndIndex;
                neutralForceAuthoring.startingPosition = startingPosition;
            }
            
            GameObject.Instantiate(prefab);
        }

        protected override void OnUpdate()
        {
            //
            // var environmentEntity = _theEnvironmentQuery.GetSingletonEntity();
            // var levelWaypointPathLookup = EntityManager.GetComponentData<LevelWaypointPathLookup>(environmentEntity);
            // var neutralUnitSpawn = EntityManager.GetComponentData<NeutralUnitSpawn>(environmentEntity);

            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            //
            var deltaTime = Time.DeltaTime;

            //
            Entities
                .ForEach((Entity entity, NeutralUnitSpawn neutralUnitSpawn) =>
                {
                    var elapsedTime = neutralUnitSpawn.CountDown + deltaTime;

                    neutralUnitSpawn.CountDown = elapsedTime;

                    if (elapsedTime >= neutralUnitSpawn.IntervalMax)
                    {
                        neutralUnitSpawn.CountDown = 0;

                        CreateNeutralForceUnit(neutralUnitPrefab);
                    }
                    
                    commandBuffer.SetComponent(entity, neutralUnitSpawn);
                })
                .WithoutBurst()
                .Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency); 
        }
    }
}
