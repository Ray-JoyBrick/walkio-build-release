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
    using UnityEngine;

    [System.Serializable]
    public struct HealthPickup : IComponentData
    {
        public float restoredAmount;
    }

    [DisableAutoCreation]
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class PickupSystem : SystemBase
    {
        private BuildPhysicsWorld buildPhysicsWorldSystem;
        private StepPhysicsWorld stepPhysicsWorldSystem;
        private EntityCommandBufferSystem barrier;

        // [BurstCompile]
        struct HealthPickupSystemJob : ITriggerEventsJob
        {
            public EntityCommandBuffer entityCommandBuffer;

            public ComponentDataFromEntity<HealthPickup> healthPickupGroup;
            public ComponentDataFromEntity<Health> healthGroup;
            [ReadOnly] public ComponentDataFromEntity<PlayerCharacter> playerCharacterGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity entityA = triggerEvent.Entities.EntityA;
                Entity entityB = triggerEvent.Entities.EntityB;
                bool entityAIsHealthPickup = healthPickupGroup.Exists(entityA);
                bool entityBIsPlayerCharacter = playerCharacterGroup.Exists(entityB);
                if (entityAIsHealthPickup && entityBIsPlayerCharacter)
                {
                    Debug.Log($"entityAIsHealthPickup && entityBIsPlayerCharacter");

                    HealthPickup healthPickUp = healthPickupGroup[entityA];
                    Health health = healthGroup[entityB];

                    health.Value += healthPickUp.restoredAmount;
                    healthGroup[entityB] = health;

                    entityCommandBuffer.DestroyEntity(entityA);
                }
                else
                {
                    // Debug.Log($"not entityAIsHealthPickup && entityBIsPlayerCharacter");

                    bool entityAIsPlayerCharacter = playerCharacterGroup.Exists(entityA);
                    bool entityBIsHealthPickUp = healthPickupGroup.Exists(entityB);
                    if (entityAIsPlayerCharacter && entityBIsHealthPickUp)
                    {
                        Debug.Log($"entityAIsPlayerCharacter && entityBIsHealthPickUp");

                        HealthPickup healthPickUp = healthPickupGroup[entityB];
                        Health health = healthGroup[entityA];

                        health.Value = math.clamp(health.Value + healthPickUp.restoredAmount, 0, health.MaxValue);
                        healthGroup[entityA] = health;

                        entityCommandBuffer.DestroyEntity(entityB);
                    }
                }
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            // barrier = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var job = new HealthPickupSystemJob
            {
                entityCommandBuffer = barrier.CreateCommandBuffer(),
                healthPickupGroup = GetComponentDataFromEntity<HealthPickup>(),
                healthGroup = GetComponentDataFromEntity<Health>(),
                playerCharacterGroup = GetComponentDataFromEntity<PlayerCharacter>(true)
            };
            Dependency = job.Schedule(stepPhysicsWorldSystem.Simulation, ref buildPhysicsWorldSystem.PhysicsWorld,
                Dependency);
            barrier.AddJobHandleForProducer(Dependency);
        }        
        
        // protected override JobHandle OnUpdate(JobHandle inputDeps)
        // {
        //     var job = new HealthPickupSystemJob
        //     {
        //         entityCommandBuffer = barrier.CreateCommandBuffer(),
        //         healthPickupGroup = GetComponentDataFromEntity<HealthPickup>(),
        //         healthGroup = GetComponentDataFromEntity<Health>(),
        //         playerCharacterGroup = GetComponentDataFromEntity<PlayerCharacter>(true)
        //     };
        //     inputDeps = job.Schedule(stepPhysicsWorldSystem.Simulation, ref buildPhysicsWorldSystem.PhysicsWorld,
        //         inputDeps);
        //     barrier.AddJobHandleForProducer(inputDeps);
        //
        //     return inputDeps;
        // }
    }
}