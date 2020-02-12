using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeForceSystem : 
    JobComponentSystem
{
    public Controls controls;
    
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
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new ChangeForceJob();
        var v = (float2) controls.Gameplay.Move.ReadValue<Vector2>();
        job.Direction = math.float3(v.x, 0, v.y);
        
        return job.Schedule(this, inputDependencies);
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        //
        controls = new Controls();
    }

    // protected override void OnDestroy()
    // {
    //     base.OnDestroy();
    // }
    
    //
    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        controls.Enable();
    }

    protected override void OnStopRunning()
    {
        base.OnStartRunning();

        controls.Disable();
    }
}
