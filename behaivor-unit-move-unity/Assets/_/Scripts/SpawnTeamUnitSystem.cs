namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;

    public class SpawnTeamUnitSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        private EntityArchetype _teamArchetype;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _teamArchetype = EntityManager.CreateArchetype(
                typeof(LocalToWorld),
                typeof(Translation),
                
                typeof(Team),
                typeof(Unit));

            _entityCommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var teamArchetype = EntityManager.CreateArchetype(
                typeof(LocalToWorld),
                typeof(Translation),
                
                typeof(Team),
                typeof(Unit),
                typeof(UnitMoveToTarget));

            Entities
                .WithAll<SpawnCorrespondingTeam>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    var teamEntity = commandBuffer.CreateEntity(entityInQueryIndex, teamArchetype);
                    commandBuffer.SetComponent<Translation>(entityInQueryIndex, teamEntity, new Translation
                    {
                        Value = float3.zero
                    });

                    commandBuffer.SetComponent<Team>(entityInQueryIndex, teamEntity, new Team
                    {
                        Id = 10
                    });
                    
                    commandBuffer.SetComponent<UnitMoveToTarget>(entityInQueryIndex, teamEntity, new UnitMoveToTarget
                    {
                        Target = float3.zero,
                        MoveSpeed = 2.0f
                    });

                    //
                    commandBuffer.RemoveComponent<SpawnCorrespondingTeam>(entityInQueryIndex, entity);
                })
                // .WithoutBurst()
                // .Run();
                .Schedule();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
