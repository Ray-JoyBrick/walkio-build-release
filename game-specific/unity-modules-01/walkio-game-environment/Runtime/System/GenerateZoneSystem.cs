namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Entities;

    [DisableAutoCreation]
    [UpdateAfter(typeof(LoadZoneTemplateSystem))]
    public class GenerateZoneSystem :
        SystemBase
    {
        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            //
            _entityCommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();            
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            // var zoneArchetype = EntityManager.CreateArchetype(
            //     typeof(Zone),
            //     typeof(ZoneGridCellBuffer));
            var zoneEntityQuery = EntityManager.CreateEntityQuery(typeof(Zone));
            var zoneEntity = zoneEntityQuery.GetSingletonEntity();

            var genratePathfindEventArchetype = EntityManager.CreateArchetype(
                typeof(GeneratePathfind));
            
            Entities
                .WithAll<GenerateZone>()
                .ForEach((Entity entity, int entityInQueryIndex, GenerateZoneProperty generateZoneProperty) =>
                {
                    var width = generateZoneProperty.Width;
                    var height = generateZoneProperty.Height;
                    var total = width * height;
                    
                    //
                    // var zoneEntity = concurrentCommandBuffer.CreateEntity(entityInQueryIndex, zoneArchetype);
                    concurrentCommandBuffer.SetComponent(entityInQueryIndex, zoneEntity, new Zone
                    {
                        Width = width,
                        Height = height
                    });
                    
                    // // This buffer can be replaced by asset blob
                    // var buffer = concurrentCommandBuffer.AddBuffer<ZoneGridCellBuffer>(entityInQueryIndex, zoneEntity);
                    //
                    // buffer.ResizeUninitialized(total);

                    concurrentCommandBuffer.CreateEntity(entityInQueryIndex, genratePathfindEventArchetype);
                    
                    //
                    concurrentCommandBuffer.DestroyEntity(entityInQueryIndex, entity);
                })
                .Schedule();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);                
        }
    }
}
