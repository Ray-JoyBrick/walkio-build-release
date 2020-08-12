namespace Game
{
    using global::Game;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Physics.Systems;
    using UnityEngine;

    // // [System.Serializable]
    // public struct Pickup : IComponentData
    // {
    //     public int Id;
    //     public bool Interacted;
    // }

    public struct NeutralAbsorberInteractWithNeutralAbsorbable : IComponentData
    {
    }

    public struct NeutralAbsorberInteractWithNeutralAbsorbableContext : IComponentData
    {
        public Entity Absorbable;
    }

    [DisableAutoCreation]
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    // [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class NeutralForceUnitHitCheckSystem : SystemBase
    {
        //
        private BuildPhysicsWorld _buildPhysicsWorldSystem;
        private StepPhysicsWorld _stepPhysicsWorldSystem;
        private EndFramePhysicsSystem _endFramePhysicsSystem;
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        private EntityArchetype _eventEntityArchetype;

        //
        private bool _canUpdate;
        
        // [BurstCompile]
        struct NeutralAbsorbableCheckSystemJob : ITriggerEventsJob
        {
            //
            public EntityArchetype eventEntityArchetype;
            public EntityCommandBuffer entityCommandBuffer;

            //
            public ComponentDataFromEntity<NeutralAbsorbable> neutralAbsorbableGroup;
            [ReadOnly] public ComponentDataFromEntity<NeutralAbsorber> neutralAbsorberGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                var entityA = triggerEvent.Entities.EntityA;
                var entityB = triggerEvent.Entities.EntityB;

                var entityAIsNeutralAbsorbable = neutralAbsorbableGroup.HasComponent(entityA);
                var entityBIsNeutralAbsorber = neutralAbsorberGroup.HasComponent(entityB);
                
                if (entityAIsNeutralAbsorbable && entityBIsNeutralAbsorber)
                {
                    Debug.Log($"entityAIsNeutralAbsorbable && entityBIsNeutralAbsorber");

                    var neutralAbsorbable = neutralAbsorbableGroup[entityA];

                    if (!neutralAbsorbable.Absorbed)
                    {
                        // var neutralAbsorber = neutralAbsorberGroup[entityB];

                        neutralAbsorbable.Absorbed = true;

                        var eventEntity = entityCommandBuffer.CreateEntity(eventEntityArchetype);
                        
                        entityCommandBuffer.SetComponent(eventEntity, new NeutralAbsorberInteractWithNeutralAbsorbableContext
                        {
                            Absorbable = entityA
                        });
                    }
                }
                else
                {
                    var entityAIsNeutralAbsorber = neutralAbsorberGroup.HasComponent(entityA);
                    var entityBIsNeutralAbsorbable = neutralAbsorbableGroup.HasComponent(entityB);
                    
                    if (entityAIsNeutralAbsorber && entityBIsNeutralAbsorbable)
                    {
                        // Debug.Log($"entityAIsNeutralAbsorber && entityBIsNeutralAbsorbable");

                        var neutralAbsorbable = neutralAbsorbableGroup[entityB];

                        if (!neutralAbsorbable.Absorbed)
                        {
                            // var neutralAbsorber = neutralAbsorberGroup[entityA];

                            neutralAbsorbable.Absorbed = true;

                            var eventEntity = entityCommandBuffer.CreateEntity(eventEntityArchetype);
                        
                            entityCommandBuffer.SetComponent(eventEntity, new NeutralAbsorberInteractWithNeutralAbsorbableContext
                            {
                                Absorbable = entityB
                            });
                        }
                    }
                }
            }
        }
        
        protected override void OnCreate()
        {
            base.OnCreate();

            _buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
            _endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            _eventEntityArchetype = EntityManager.CreateArchetype(
                typeof(NeutralAbsorberInteractWithNeutralAbsorbable),
                typeof(NeutralAbsorberInteractWithNeutralAbsorbableContext));
        }        

        protected override void OnUpdate()
        {
            // if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();
            
            var job = new NeutralAbsorbableCheckSystemJob
            {
                eventEntityArchetype = _eventEntityArchetype,
                entityCommandBuffer = commandBuffer,
                neutralAbsorbableGroup = GetComponentDataFromEntity<NeutralAbsorbable>(),
                neutralAbsorberGroup = GetComponentDataFromEntity<NeutralAbsorber>(true)
            };

            Dependency = job.Schedule(
                _stepPhysicsWorldSystem.Simulation,
                ref _buildPhysicsWorldSystem.PhysicsWorld,
                Dependency);

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
