namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Entities;
    using Unity.Rendering;
    using Unity.Transforms;
    using UnityEngine;
    
    using AppUtility = JoyBrick.Walkio.Game.Utility;

    [DisableAutoCreation]
    public abstract class GenerateVisualWorldMapSystem_Base : SystemBase
    {
        // protected EntityQuery _eventQuery;
        //
        // protected override void OnCreate()
        // {
        //     base.OnCreate();
        //     
        //     //
        //     _eventQuery = GetEntityQuery(new EntityQueryDesc
        //     {
        //         All = new ComponentType[] { typeof(GenerateVisualWorldMap) }
        //     });
        //     
        //     RequireForUpdate(_eventQuery);
        // }        
    }

    [DisableAutoCreation]
    public class GenerateDiagnosticWorldMapSystem_Empty : GenerateVisualWorldMapSystem_Base
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
    public class GenerateDiagnosticWorldMapSystem : GenerateVisualWorldMapSystem_Base
    {
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        protected EntityQuery _eventQuery;
        private EntityArchetype _visualMapArchetype;
        private EntityQuery _mapQuery;
        private EntityQuery _diagnosticWorldMapQuery;

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

            // _visualMapArchetype = EntityManager.CreateArchetype(
            //     typeof(RenderMesh),
            //     typeof(RenderBounds),
            //     typeof(LocalToWorld),
            //     typeof(Translation),
            //     typeof(DiagnosticWorldMap));
            
            _diagnosticWorldMapQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<DiagnosticWorldMap>(), 
                    ComponentType.ReadWrite<RenderMesh>(),
                }
            });
            
            RequireForUpdate(_eventQuery);
            RequireForUpdate(_diagnosticWorldMapQuery);
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
            
            // var visualMapEntity = commandBuffer.CreateEntity(_visualMapArchetype);
            var diagnosticWorldMapEntity = _diagnosticWorldMapQuery.GetSingletonEntity();

            var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(diagnosticWorldMapEntity);

            var mesh = AppUtility.Geometry.CreatePlane(width, height);
            commandBuffer.SetSharedComponent(diagnosticWorldMapEntity, new RenderMesh
            {
                mesh = mesh,
                material = renderMesh.material
            });
            
            commandBuffer.DestroyEntity(entity);
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}