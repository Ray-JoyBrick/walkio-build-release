namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;
    using Unity.Rendering;
    using Unity.Transforms;
    using UnityEngine;

    [DisableAutoCreation]
    [AlwaysSynchronizeSystem]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class PresentGridWorldSystem : SystemBase
    {
        private EntityQuery _mapQuery;
        private EntityQuery _visualMapQuery;
        
        private BeginPresentationEntityCommandBufferSystem _entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            base.OnCreate();

            _mapQuery = GetEntityQuery(
                typeof(WorldMap),
                typeof(MapCell));

            _visualMapQuery = GetEntityQuery(
                typeof(RenderMesh),
                typeof(RenderBounds),
                typeof(LocalToWorld),
                typeof(Translation),
                typeof(WorldMap));

            _entityCommandBufferSystem = World.GetExistingSystem<BeginPresentationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            // var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();

            var mapEntity = _mapQuery.GetSingletonEntity();

            // Map entity has not been created yet
            if (mapEntity == Entity.Null)
            {
                return;
            }
            
            // var mapData = _mapQuery.GetSingleton<MapCell>();
            var buffer = EntityManager.GetBuffer<MapCell>(mapEntity);

            var visualMapEntity = _visualMapQuery.GetSingletonEntity();
            var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(visualMapEntity);

            Job
                .WithCode(() =>
                {
                    var width = 20;
                    var height = 20;
                    var tileSize = 1.0f;

                    var count = buffer.Length;

                    // Debug.Log($"count of buffer: {count}");
                    
                    var vertices = new Vector3[4 * count];
                    var uvs = new Vector2[4 * count];
                    var triangles = new int[6 * count];
                    
                    for (var i = 0; i < width; ++i)
                    {
                        for (var j = 0; j < height; ++j)
                        {
                            var index = i * height + j;
                            vertices[index * 4 + 0] = new Vector3(tileSize * i, tileSize * j);
                            vertices[index * 4 + 1] = new Vector3(tileSize * i, tileSize * (j + 1));
                            vertices[index * 4 + 2] = new Vector3(tileSize * (i + 1), tileSize * (j + 1));
                            vertices[index * 4 + 3] = new Vector3(tileSize * (i + 1), tileSize * j);
                            
                            uvs[index * 4 + 0] = new Vector2(0, 0);
                            uvs[index * 4 + 1] = new Vector2(0, 1);
                            uvs[index * 4 + 2] = new Vector2(1, 1);
                            uvs[index * 4 + 3] = new Vector2(1, 0);
                    
                            triangles[index * 6 + 0] = index * 4 + 0;
                            triangles[index * 6 + 1] = index * 4 + 1;
                            triangles[index * 6 + 2] = index * 4 + 2;
                            
                            triangles[index * 6 + 3] = index * 4 + 0;
                            triangles[index * 6 + 4] = index * 4 + 2;
                            triangles[index * 6 + 5] = index * 4 + 3;
                            
                        }
                    }
                    
                    // Debug.Log($"Finish making vertices");

                    var mesh = new Mesh
                    {
                        vertices = vertices,
                        uv = uvs,
                        triangles = triangles
                    };
                    
                    renderMesh.mesh = mesh;
                    
                    // EntityManager.SetSharedComponentData(visualMapEntity, renderMesh);
                    //
                    commandBuffer.SetSharedComponent<RenderMesh>(visualMapEntity, renderMesh);

                    //
                    //
                    // var meshFilter = GetComponent<MeshFilter>();
                    // meshFilter.mesh = mesh;
                })
                .WithoutBurst()
                .Run();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
