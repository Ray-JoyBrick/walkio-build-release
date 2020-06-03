namespace Game
{
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;
    using UnityEngine;

    [DisableAutoCreation]
    public class SpawnTeamUnitSystem : SystemBase
    {
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _theEnvironmentQuery;
        
        //
        private float _countDown = 5.0f;

        public GameObject teamUnitPrefab { get; set; }
        
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
        
        private void CreateTeamForceUnit(GameObject prefab)
        {
            // This should be converted to entity automatically
            
            var teamForceAuthoring = prefab.GetComponent<UnitAuthoring>();
            if (teamForceAuthoring != null)
            {
                // neutralForceAuthoring.startPathIndex = waypointPathIndexPair.StartIndex;
                // neutralForceAuthoring.endPathIndex = waypointPathIndexPair.EndIndex;
                teamForceAuthoring.startingPosition = new float3(
                    UnityEngine.Random.Range(0, 20.0f),
                    0,
                    UnityEngine.Random.Range(0, 20.0f));
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
                .ForEach((Entity entity, TeamUnitSpawn teamUnitSpawn) =>
                {
                    var elapsedTime = teamUnitSpawn.CountDown + deltaTime;

                    teamUnitSpawn.CountDown = elapsedTime;

                    if (elapsedTime >= teamUnitSpawn.IntervalMax)
                    {
                        teamUnitSpawn.CountDown = 0;

                        CreateTeamForceUnit(teamUnitPrefab);
                    }
                    
                    commandBuffer.SetComponent(entity, teamUnitSpawn);
                })
                .WithoutBurst()
                .Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency); 
        }
    }
}
