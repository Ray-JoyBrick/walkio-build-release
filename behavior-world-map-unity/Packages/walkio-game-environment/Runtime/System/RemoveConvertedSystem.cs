namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Entities;
    using UnityEngine;

    [DisableAutoCreation]
    public class RemoveConvertedSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            Entities
                // .WithAll<TileDataBlobAssetAuthoring>()
                // .WithAll<TileDataAssetEx>()
                .WithAll<Bridge.TileDataAsset>()
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