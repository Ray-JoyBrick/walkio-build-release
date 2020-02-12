using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using UnityEngine;

public class MoveBallSystem : JobComponentSystem
{
    [BurstCompile]
    struct MoveBallJob : IJobForEach<PhysicsVelocity, PhysicsMass, Ball, Force>
    {
        public float DeltaTime;

        public void Execute(ref PhysicsVelocity physicsVelocity, 
            [ReadOnly] ref PhysicsMass physicsMass, 
            [ReadOnly] ref Ball ball, 
            ref Force force)
        {
            physicsVelocity.Linear += physicsMass.InverseMass * force.direction * force.magnitude * DeltaTime;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new MoveBallJob
        {
            DeltaTime = Time.DeltaTime,
        };

        return job.Schedule(this, inputDeps);
    }
}