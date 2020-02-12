using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class TriggerSystem : JobComponentSystem
{
    private BuildPhysicsWorld _buildPhysicsWorldSystem;
    private StepPhysicsWorld _stepPhysicsWorldSystem;
    private EntityCommandBufferSystem _bufferSystem;

    // 追加
    private EntityQuery _entityQuery;

    protected override void OnCreate()
    {
        _buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
        _bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        // 追加
        _entityQuery = GetEntityQuery(ComponentType.ReadOnly<Count>());
    }

    private struct TriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<Cube> Cube;
        [ReadOnly] public ComponentDataFromEntity<Ball> Ball;
        public EntityCommandBuffer CommandBuffer;

        //　追加
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> CountArray;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.Entities.EntityA;
            var entityB = triggerEvent.Entities.EntityB;

            var isBodyACube = Cube.Exists(entityA);
            var isBodyBCube = Cube.Exists(entityB);

            var isBodyABall = Ball.Exists(entityA);
            var isBodyBBall = Ball.Exists(entityB);

            if (!isBodyACube && !isBodyBCube)
                return;

            if(!isBodyABall && !isBodyBBall)
                return;

            var cubeEntity = isBodyACube ? entityA : entityB;
            var ballEntity = isBodyABall ? entityA : entityB;

            CommandBuffer.DestroyEntity(cubeEntity);

            // 追加
            foreach (var entity in CountArray)
            {
                CommandBuffer.AddComponent(entity, new CountUp());
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var jobHandle = new TriggerJob
        {
            Cube = GetComponentDataFromEntity<Cube>(true),
            Ball = GetComponentDataFromEntity<Ball>(true),
            CommandBuffer = _bufferSystem.CreateCommandBuffer(),
            // 追加
            CountArray = _entityQuery.ToEntityArray(Allocator.TempJob)
        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _buildPhysicsWorldSystem.PhysicsWorld, inputDeps);

        _bufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}