namespace JoyBrick.Walkio.Game
{
    using Unity.Burst;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;

    [DisableAutoCreation]
    public class NonTeamUnitMoveSystem : SystemBase
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
            var deltaTime = Time.DeltaTime;

            Entities
                .WithName("NonTeamUnitMoveSystem")
                .WithAll<Unit>()
                .WithNone<Team>()
                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
                .ForEach((Entity entity, int entityInQueryIndex, ref UnitMoveToTarget unitMoveToTarget, ref Translation translation) =>
                {
                    //
                    var position = translation.Value;
                    var nearTarget = math.distance(unitMoveToTarget.Target, position) < 0.1f;
                    if (nearTarget)
                    {
                        // Actually near target, ask to assign new target?
                        commandBuffer.AddComponent<UnitRequestNewTarget>(entityInQueryIndex, entity, new UnitRequestNewTarget());
                    }
                    else
                    {
                        var direction = math.normalize(unitMoveToTarget.Target - position);
                        translation.Value += direction * deltaTime * unitMoveToTarget.MoveSpeed;
                    }
                })
                .ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
