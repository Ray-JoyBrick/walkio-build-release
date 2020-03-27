namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Entities;

    [DisableAutoCreation]
    public class GenerateWorldMapSystem : SystemBase
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
            
            var worldMapArchetype = EntityManager.CreateArchetype(
                typeof(WorldMap),
                typeof(WorldMapTileBuffer));
            
            var generateDiagnosticEventArchetype = EntityManager.CreateArchetype(
                typeof(GenerateDiagnosticWorldMap));

            var generateVisualEventArchetype = EntityManager.CreateArchetype(
                typeof(GenerateVisualWorldMap));

            Entities
                .WithAll<GenerateWorldMap>()
                .ForEach((Entity entity, int entityInQueryIndex, GenerateWorldMapProperty generateWorldMapProperty) =>
                {
                    // Need to get map data to assign further

                    var width = generateWorldMapProperty.Width;
                    var height = generateWorldMapProperty.Height;
                    var total = width * height;
                    
                    //
                    var mapEntity = concurrentCommandBuffer.CreateEntity(entityInQueryIndex, worldMapArchetype);
                    concurrentCommandBuffer.AddComponent<WorldMap>(entityInQueryIndex, mapEntity, new WorldMap
                    {
                        Width = width,
                        Height = height
                    });
                    var buffer = concurrentCommandBuffer.AddBuffer<WorldMapTileBuffer>(entityInQueryIndex, mapEntity);
                    
                    buffer.ResizeUninitialized(0);
                    // buffer.

                    var diagnosticEventEntity = concurrentCommandBuffer.CreateEntity(entityInQueryIndex, generateDiagnosticEventArchetype);
                    concurrentCommandBuffer.AddComponent<GenerateDiagnosticWorldMap>(entityInQueryIndex, diagnosticEventEntity);

                    var visualEventEntity =
                        concurrentCommandBuffer.CreateEntity(entityInQueryIndex, generateVisualEventArchetype);
                    concurrentCommandBuffer.AddComponent<GenerateVisualWorldMap>(entityInQueryIndex, visualEventEntity);
                    
                    //
                    concurrentCommandBuffer.DestroyEntity(entityInQueryIndex, entity);
                    
                })
                .Schedule();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
