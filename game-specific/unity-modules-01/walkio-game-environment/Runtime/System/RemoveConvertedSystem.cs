namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Entities;
    using UnityEngine;

    // TODO: Move this to common module since it may apply to systems in any module
    [DisableAutoCreation]
    public class RemoveConvertedSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            Entities
                .WithAll<RemoveAfterConversion>()
                // .ForEach((Entity entity, int entityInQueryIndex) =>
                .ForEach((Entity entity) =>
                {
                    // concurrentCommandBuffer.DestroyEntity(entityInQueryIndex, entity);
                    Debug.Log($"destroy entity");
                    commandBuffer.DestroyEntity(entity);
                })
                // .Schedule();
                .WithoutBurst()
                .Run();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
