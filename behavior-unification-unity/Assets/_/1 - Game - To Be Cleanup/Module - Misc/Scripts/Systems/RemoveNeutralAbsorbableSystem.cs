namespace Game
{
    using Unity.Entities;
    using UnityEngine;

    [DisableAutoCreation]
    [UpdateAfter(typeof(NeutralForceUnitHitCheckSystem))]
    public class RemoveNeutralAbsorbableSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            Entities
                .WithAll<NeutralAbsorberInteractWithNeutralAbsorbable>()
                // .ForEach((Entity entity, int entityInQueryIndex, NeutralAbsorberInteractWithNeutralAbsorbableContext context) =>
                .ForEach((Entity entity, NeutralAbsorberInteractWithNeutralAbsorbableContext context) =>
                {
                    // concurrentCommandBuffer.DestroyEntity(entityInQueryIndex, context.Absorbable);
                    
                    // Debug.Log($"EndSimulationEntityCommandBufferSystem - {context.Absorbable}");
                    commandBuffer.DestroyEntity(context.Absorbable);
                    
                    // concurrentCommandBuffer.DestroyEntity(entityInQueryIndex, entity);
                    commandBuffer.DestroyEntity(entity);
                })
                .WithoutBurst()
                .Run();
                // .Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency); 
        }
    }
}
