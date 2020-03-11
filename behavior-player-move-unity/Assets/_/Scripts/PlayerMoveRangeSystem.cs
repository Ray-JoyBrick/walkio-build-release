namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;
    using Unity.Rendering;
    using UnityEngine;

    [DisableAutoCreation]
    [AlwaysSynchronizeSystem]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class PlayerMoveRangeSystem : SystemBase
    {
        private EntityQuery _playerQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            _playerQuery = GetEntityQuery(
                ComponentType.ReadOnly<Player>(),
                ComponentType.ReadOnly<MovementRequest>(),
                ComponentType.ReadOnly<PlayerMoveRange>(),
                typeof(RenderMesh));
            
            // var mesh = new Mesh();
            //
            // var vertices = new Vector3[4];
            // var uvs = new Vector2[4];
            // var triangles = new int[6];
            //
            // vertices[0] = new Vector3(0, 0, 0);
            // vertices[1] = new Vector3(0, 100, 0);
            // vertices[2] = new Vector3(100, 100, 0);
            // vertices[3] = new Vector3(100, 0, 0);
            //
            // uvs[0] = new Vector2(0, 0);
            // uvs[1] = new Vector2(0, 1);
            // uvs[2] = new Vector2(1, 1);
            // uvs[2] = new Vector2(1, 0);
            //
            // triangles[0] = 0;
            // triangles[1] = 1;
            // triangles[2] = 2;
            //
            // triangles[3] = 0;
            // triangles[4] = 2;
            // triangles[5] = 3;
            //
            // mesh.vertices = vertices;
            // mesh.uv = uvs;
            // mesh.triangles = triangles;
        }

        protected override void OnUpdate()
        {
            var playerEntity = _playerQuery.GetSingletonEntity();
            var movementRequest = EntityManager.GetComponentData<MovementRequest>(playerEntity);
            var playerMoveRange = EntityManager.GetComponentData<PlayerMoveRange>(playerEntity);
            var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(playerEntity);

            var mesh = renderMesh.mesh;

            var vertices = new Vector3[4];
            var uvs = new Vector2[4];
            var triangles = new int[6];

            var size = movementRequest.Strength * 300.0f;
            var halfSize = size * 0.5f;
            var center = halfSize;

            vertices[0] = new Vector3(-halfSize, 0, -halfSize);
            vertices[1] = new Vector3(-halfSize, 0, halfSize);
            vertices[2] = new Vector3(halfSize, 0, halfSize);
            vertices[3] = new Vector3(halfSize, 0, -halfSize);

            uvs[0] = new Vector2(0, 0);
            uvs[1] = new Vector2(0, 1);
            uvs[2] = new Vector2(1, 1);
            uvs[2] = new Vector2(1, 0);
            
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            
            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            // var mesh = new Mesh();
            //
            // var vertices = new Vector3[4];
            // var uvs = new Vector2[4];
            // var triangles = new int[6];
            //
            // vertices[0] = new Vector3(0, 0, 0);
            // vertices[1] = new Vector3(0, 100, 0);
            // vertices[2] = new Vector3(100, 100, 0);
            // vertices[3] = new Vector3(100, 0, 0);
            //
            // uvs[0] = new Vector2(0, 0);
            // uvs[1] = new Vector2(0, 1);
            // uvs[2] = new Vector2(1, 1);
            // uvs[2] = new Vector2(1, 0);
            //
            // triangles[0] = 0;
            // triangles[1] = 1;
            // triangles[2] = 2;
            //
            // triangles[3] = 0;
            // triangles[4] = 2;
            // triangles[5] = 3;
            //
            // mesh.vertices = vertices;
            // mesh.uv = uvs;
            // mesh.triangles = triangles;
            //
            // renderMesh.mesh = mesh;
        }
    }
}
