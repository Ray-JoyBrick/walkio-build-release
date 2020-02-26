namespace JoyBrick.Walkio.Game.Main
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Physics.Systems;
    using Unity.Transforms;
    using UnityEngine;

    [System.Serializable]
    public struct Character : IComponentData
    {
        public float MoveSpeed;
        public float MoveSharpness;
        public float OrientSharpness;

        public float DecollisionDamping;

        [System.NonSerialized] public float3 StoredImpulse;

    }

    [System.Serializable]
    public struct PlayerCharacter : IComponentData
    {
    }

    [System.Serializable]
    public struct CharacterInputs : IComponentData
    {
        public float3 MoveVector;
        public float3 LookDirection;
    }

    [System.Serializable]
    public struct OwningPlayer : IComponentData
    {
        public Entity PlayerEntity;
    }

    [System.Serializable]
    public struct Health : IComponentData
    {
        public float MaxValue;
        [System.NonSerialized] public float Value;
    }

    [DisableAutoCreation]
    [UpdateAfter(typeof(PlayerInputSystem))]
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    // public class CharacterMoveSystem : JobComponentSystem
    public class CharacterMoveSystem : SystemBase
    {
        public Transform CameraTransform;

        [BurstCompile]
        struct PlayerInputsToCharacterInputsJob : IJobForEach<Character, CharacterInputs, OwningPlayer>
        {
            public float3 CameraPlanarForwardDirection;
            public float3 CameraPlanarRightDirection;

            public quaternion CameraPlanarRotation;

            [ReadOnly] public ComponentDataFromEntity<GameplayInputs> GameplayInputsFromEntity;

            public void Execute(
                [ReadOnly] ref Character character,
                ref CharacterInputs characterInputs,
                [ReadOnly] ref OwningPlayer owningPlayer)
            {
                GameplayInputs i = GameplayInputsFromEntity[owningPlayer.PlayerEntity];
                
                // Debug.Log($"CharacterMoveSystem - PlayerInputsToCharacterInputsJob - {i.Move}");

                characterInputs.MoveVector =
                    (CameraPlanarRightDirection * i.Move.x) + (CameraPlanarForwardDirection * i.Move.y);

                characterInputs.LookDirection = math.mul(CameraPlanarRotation, new float3(i.Look.x, 0f, i.Look.y));
            }
        }

        [BurstCompile]
        struct CharacterMoveJob : IJobForEach<Character, CharacterInputs, PhysicsVelocity, Rotation>
        {
            public float deltaTime;

            public unsafe void Execute(
                ref Character character,
                [ReadOnly] ref CharacterInputs characterInputs,
                ref PhysicsVelocity velocity,
                ref Rotation rotation)
            {
                // Velocity
                float3 targetPlanarVel = characterInputs.MoveVector * character.MoveSpeed;
                velocity.Linear = math.lerp(velocity.Linear, targetPlanarVel,
                    1f - math.exp(-character.MoveSharpness * deltaTime));
                velocity.Linear += character.StoredImpulse / character.DecollisionDamping;
                character.StoredImpulse = default;
                velocity.Linear.y = 0;

                if (math.lengthsq(characterInputs.LookDirection) > 0f)
                {
                    var smoothedRotation = math.slerp(
                        rotation.Value,
                        quaternion.LookRotationSafe(characterInputs.LookDirection, math.up()),
                        1f - math.exp(-character.OrientSharpness * deltaTime));
                    rotation.Value = smoothedRotation;
                }
            }
        }
        
        protected override void OnUpdate()
        {
            if (!CameraTransform) return;
        
            var cameraPlanarForward = Vector3.ProjectOnPlane(CameraTransform.forward, Vector3.up).normalized;
            var cameraPlanarRight = Vector3.ProjectOnPlane(CameraTransform.right, Vector3.up).normalized;
            var cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarForward, Vector3.up);
        
            var playerInputsToCharacterInputsJob = new PlayerInputsToCharacterInputsJob
            {
                CameraPlanarForwardDirection = cameraPlanarForward,
                CameraPlanarRightDirection = cameraPlanarRight,
                CameraPlanarRotation = cameraPlanarRotation,
                GameplayInputsFromEntity = GetComponentDataFromEntity<GameplayInputs>()
            };
            Dependency = playerInputsToCharacterInputsJob.Schedule(this, Dependency);
        
            var characterMoveSystemJob = new CharacterMoveJob
            {
                deltaTime = Time.DeltaTime
            };
        
            Dependency = characterMoveSystemJob.Schedule(this, Dependency);
        
            // return inputDeps;
        }

        // protected override JobHandle OnUpdate(JobHandle inputDeps)
        // {
        //     if (!CameraTransform) return inputDeps;
        //
        //     var cameraPlanarForward = Vector3.ProjectOnPlane(CameraTransform.forward, Vector3.up).normalized;
        //     var cameraPlanarRight = Vector3.ProjectOnPlane(CameraTransform.right, Vector3.up).normalized;
        //     var cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarForward, Vector3.up);
        //
        //     var playerInputsToCharacterInputsJob = new PlayerInputsToCharacterInputsJob
        //     {
        //         CameraPlanarForwardDirection = cameraPlanarForward,
        //         CameraPlanarRightDirection = cameraPlanarRight,
        //         CameraPlanarRotation = cameraPlanarRotation,
        //         GameplayInputsFromEntity = GetComponentDataFromEntity<GameplayInputs>()
        //     };
        //     inputDeps = playerInputsToCharacterInputsJob.Schedule(this, inputDeps);
        //
        //     var characterMoveSystemJob = new CharacterMoveJob
        //     {
        //         deltaTime = Time.DeltaTime
        //     };
        //
        //     inputDeps = characterMoveSystemJob.Schedule(this, inputDeps);
        //
        //     return inputDeps;
        // }
    }
}