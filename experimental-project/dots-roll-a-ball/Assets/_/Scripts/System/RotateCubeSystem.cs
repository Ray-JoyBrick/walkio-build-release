using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RotateCubeSystem : JobComponentSystem
{
    [BurstCompile]
    private struct RotateCubeJob : IJobForEach<Rotation, Cube, CubeRotationSpeed>
    {
        public float DeltaTime;

        public void Execute(ref Rotation rotation, [ReadOnly] ref Cube cube, [ReadOnly] ref CubeRotationSpeed cubeRotationSpeed)
        {
            rotation.Value = math.mul(quaternion.AxisAngle(math.up(), cubeRotationSpeed.value * DeltaTime), math.normalize(rotation.Value));
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new RotateCubeJob
        {
            DeltaTime = Time.DeltaTime,
        };

        return job.Schedule(this, inputDeps);
    }
}