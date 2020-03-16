namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Entities;
    using Unity.Rendering;
    using Unity.Transforms;
    using UnityEngine;

    // [DisableAutoCreation]
    // public abstract class GenerateVisualWorldMapSystem_Base : SystemBase
    // {
    //     protected EntityQuery _eventQuery;
    //
    //     protected override void OnCreate()
    //     {
    //         base.OnCreate();
    //         
    //         //
    //         _eventQuery = GetEntityQuery(new EntityQueryDesc
    //         {
    //             All = new ComponentType[] { typeof(GenerateVisualWorldMap) }
    //         });
    //         
    //         RequireForUpdate(_eventQuery);
    //     }        
    // }

    [DisableAutoCreation]
    public class GenerateDiagnosticWorldMapSystem_Empty : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        protected EntityQuery _eventQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            //
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _eventQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(GenerateDiagnosticWorldMap) }
            });
            
            RequireForUpdate(_eventQuery);
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();

            var entity = _eventQuery.GetSingletonEntity();
            var generateVisualWorldMap = EntityManager.GetComponentData<GenerateDiagnosticWorldMap>(entity);
            
            Debug.Log($"GenerateVisualWorldMapSystem_Empty - OnUpdate");
            
            commandBuffer.DestroyEntity(entity);
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
    
    [DisableAutoCreation]
    public class GenerateDiagnosticWorldMapSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        protected EntityQuery _eventQuery;
        private EntityArchetype _visualMapArchetype;
        private EntityQuery _mapQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            //
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _eventQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<GenerateDiagnosticWorldMap>() 
                }
            });

            _mapQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<WorldMap>(),
                    ComponentType.ReadOnly<WorldMapTileBuffer>()
                }
            });

            _visualMapArchetype = EntityManager.CreateArchetype(
                typeof(RenderMesh),
                typeof(RenderBounds),
                typeof(LocalToWorld),
                typeof(Translation),
                typeof(DiagnosticWorldMap));
            
            RequireForUpdate(_eventQuery);
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            
            //
            var entity = _eventQuery.GetSingletonEntity();
            var generateDiagnosticWorldMap = EntityManager.GetComponentData<GenerateDiagnosticWorldMap>(entity);

            //
            var mapEntity = _mapQuery.GetSingletonEntity();
            var worldMap = EntityManager.GetComponentData<WorldMap>(mapEntity);
            var worldMapTileBuffer = EntityManager.GetBuffer<WorldMapTileBuffer>(mapEntity);

            var width = worldMap.Width;
            var height = worldMap.Height;
            
            var visualMapEntity = commandBuffer.CreateEntity(_visualMapArchetype);
            var mesh = Utility.Geometry.CreatePlane(width, height);
            commandBuffer.AddSharedComponent(entity, new RenderMesh
            {
                mesh = mesh
            });
            
            commandBuffer.DestroyEntity(entity);
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}