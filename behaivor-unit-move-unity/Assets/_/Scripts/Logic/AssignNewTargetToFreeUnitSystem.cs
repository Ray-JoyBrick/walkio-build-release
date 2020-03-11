namespace JoyBrick.Walkio.Game
{
    using Unity.Burst;
    using Unity.Entities;
    using Unity.Mathematics;

    [DisableAutoCreation]
    [UpdateAfter(typeof(NonTeamUnitMoveSystem))]
    public class AssignNewTargetToFreeUnitSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var rnd = new Unity.Mathematics.Random((uint) System.DateTime.UtcNow.Ticks);

            Entities
                .WithName("AssignNewTargetToUnitSystem")
                .WithAll<Unit, UnitRequestNewTarget>()
                .WithNone<Team>()
                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
                .ForEach(
                    (Entity entity, int entityInQueryIndex, ref UnitMoveToTarget unitMoveToTarget) =>
                    {
                        var randomTarget = rnd.NextFloat3(new float3(-30.0f, 0, -30.0f), new float3(30.0f, 0, 30.0f));
                        unitMoveToTarget.Target = randomTarget;
                        commandBuffer.RemoveComponent<UnitRequestNewTarget>(entityInQueryIndex, entity);
                    }
                )
                .ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
