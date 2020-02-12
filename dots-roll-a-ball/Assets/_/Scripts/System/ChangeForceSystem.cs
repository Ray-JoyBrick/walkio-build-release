using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public class ChangeForceSystem : JobComponentSystem
{
    [BurstCompile]
    private struct ChangeForceJob : IJobForEach<PhysicsVelocity, PhysicsMass, Ball, Force>
    {
        public float3 Direction;

        public void Execute(ref PhysicsVelocity physicsVelocity, 
            [ReadOnly] ref PhysicsMass physicsMass, 
            [ReadOnly] ref Ball ball, 
            ref Force force)
        {
            force.direction = Direction;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new ChangeForceJob();

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            job.Direction = math.float3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            job.Direction = math.float3(1, 0, 0);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            job.Direction = math.float3(0, 0, 1);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            job.Direction = math.float3(0, 0, -1);
        }

        return job.Schedule(this, inputDeps);
    }
}