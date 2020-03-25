using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class SpawnCubeSystem : JobComponentSystem
{
    private EntityCommandBufferSystem _bufferSystem;

    protected override void OnCreate()
    {
        _bufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    private struct SpawnCubeJob : IJobForEachWithEntity<CubeSpawnerData, LocalToWorld>
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref CubeSpawnerData cubeSpawnerData, [ReadOnly] ref LocalToWorld localToWorld)
        {
            for (var i = 0; i < cubeSpawnerData.number; i++)
            {
                var instance = CommandBuffer.Instantiate(index, cubeSpawnerData.cubePrefabEntity);
                var posX = cubeSpawnerData.radius * math.cos(2 * math.PI / cubeSpawnerData.number * i);
                var posZ = cubeSpawnerData.radius * math.sin(2 * math.PI / cubeSpawnerData.number * i);

                CommandBuffer.SetComponent(index, instance, new Translation {Value = math.float3(posX, 1, posZ)});
            }

            // 1回Executeを実行したらCubeSpawnEntityを削除する
            CommandBuffer.DestroyEntity(index, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new SpawnCubeJob
        {
            CommandBuffer = _bufferSystem.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDeps);

        _bufferSystem.AddJobHandleForProducer(job);
        return job;
    }
}