namespace Game
{
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Physics.Systems;
    using Unity.Transforms;
    using UnityEngine;

    [DisableAutoCreation]
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class TeamUnitMoveSystem : SystemBase
    {
        private float _updateTime;
        
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities
                .WithAll<Unit, TeamForce>()
                .ForEach((Entity entity, ref UnitMovement unitMovement, ref PhysicsMass physicsMass, ref PhysicsVelocity physicsVelocity, ref Rotation rotation) =>
                {
                    // translation.Value = new float3(translation.Value.x, 0, translation.Value.z);
                    // rotation.Value = quaternion.identity;

                    
                    var smoothedRotation = math.slerp(
                        rotation.Value,
                        // quaternion.LookRotationSafe(physicsVelocity.Linear, math.up()), 1f - math.exp(-deltaTime));
                        quaternion.LookRotationSafe(physicsVelocity.Linear, math.up()), deltaTime * 3.0f);
                        // quaternion.LookRotationSafe(unitMovement.Direction, math.up()), 1f - math.exp(-deltaTime));
                        // quaternion.LookRotationSafe(unitMovement.Direction, math.up()), deltaTime);
                    rotation.Value = smoothedRotation;
                    // rotation.Value = quaternion.LookRotationSafe(unitMovement.Direction, math.up());
                    // unitMovement.Direction = smoothedRotation;
                    // rotation.Value = quaternion.identity;
                    // physicsMass.InverseInertia[0] = 0;
                    // physicsMass.InverseInertia[2] = 0;
                })
                .WithoutBurst()
                .Run();

            _updateTime += deltaTime;

            if (_updateTime >= 5.0f)
            {
                _updateTime = 0;
            }
            else
            {
                return;
            }
            
            Entities
                .WithAll<Unit, TeamForce>()
                .ForEach((Entity entity, ref UnitMovement unitMovement, ref PhysicsVelocity physicsVelocity, ref Rotation rotation) =>
                {
                    var x = UnityEngine.Random.Range(-10.0f, 10.0f);
                    var z = UnityEngine.Random.Range(-10.0f, 10.0f);
                    
                    var v = new Vector3(x, 0, z);
                    var velocity = (float3) Vector3.Normalize(v);

                    physicsVelocity.Linear = velocity * 3.0f;
                    
                    // rotation.Value = quaternion.identity;

                    unitMovement.Direction = velocity;
                    // unitMovement.Direction = velocity * 3.0f;
                    unitMovement.Rot = rotation.Value;
                    
                    // var smoothedRotation = math.slerp(
                    //     rotation.Value,
                    //     quaternion.LookRotationSafe(velocity, math.up()), 1f - math.exp(-deltaTime));
                    // rotation.Value = smoothedRotation;
                    // rotation.Value = quaternion.LookRotationSafe(velocity, math.up());

                })
                .WithoutBurst()
                .Run();
        }
    }
}
