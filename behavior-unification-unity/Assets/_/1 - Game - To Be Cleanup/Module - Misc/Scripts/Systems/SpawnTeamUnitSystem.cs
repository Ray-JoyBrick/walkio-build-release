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

        private Entity _prefabEntity;
        
        public GameObject teamUnitPrefab { get; set; }
        
        public void Construct()
        {
            // var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            // _prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(teamUnitPrefab, settings);
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
        
        // private void CreateTeamForceUnit(GameObject prefab)
        private void CreateTeamForceUnit(
            EntityCommandBuffer entityCommandBuffer,
            Entity prefab)
        {
            // This should be converted to entity automatically
            
            // var teamForceAuthoring = prefab.GetComponent<UnitAuthoring>();
            // if (teamForceAuthoring != null)
            // {
            //     // neutralForceAuthoring.startPathIndex = waypointPathIndexPair.StartIndex;
            //     // neutralForceAuthoring.endPathIndex = waypointPathIndexPair.EndIndex;
            //     teamForceAuthoring.startingPosition = new float3(
            //         UnityEngine.Random.Range(0, 20.0f),
            //         0,
            //         UnityEngine.Random.Range(0, 20.0f));
            // }
            
            // GameObject.Instantiate(prefab);

            var ent = entityCommandBuffer.Instantiate(prefab);
            var startingPosition = new float3(
                UnityEngine.Random.Range(0, 20.0f),
                0,
                UnityEngine.Random.Range(0, 20.0f));
            
            entityCommandBuffer.SetComponent(ent, new Translation
            {
                Value = startingPosition
            });
            
            // entityCommandBuffer.SetComponent(ent, new PhysicsMass
            // {
            //     InverseMass = new float3(0, 0, 0)
            // });
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
                _prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(teamUnitPrefab, settings);
            }
            
            var prefabEntity = _prefabEntity;

            //
            Entities
                .ForEach((Entity entity, TeamUnitSpawn teamUnitSpawn) =>
                {
                    var elapsedTime = teamUnitSpawn.CountDown + deltaTime;

                    teamUnitSpawn.CountDown = elapsedTime;

                    if (elapsedTime >= teamUnitSpawn.IntervalMax)
                    {
                        teamUnitSpawn.CountDown = 0;

                        // CreateTeamForceUnit(commandBuffer, teamUnitPrefab);
                        CreateTeamForceUnit(commandBuffer, prefabEntity);
                    }
                    
                    commandBuffer.SetComponent(entity, teamUnitSpawn);
                })
                .WithoutBurst()
                .Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency); 
        }
    }
}
