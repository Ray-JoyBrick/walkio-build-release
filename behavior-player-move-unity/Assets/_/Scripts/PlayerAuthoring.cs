namespace JoyBrick.Walkio.Game
{
    using System;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Rendering;
    using Unity.Transforms;
    using UnityEngine;

    public struct Player : IComponentData
    {
    }

    public struct PlayerMoveRange : IComponentData
    {
        public float Radius;
    }

    public class PlayerAuthoring : 
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public Mesh moveRangeMesh;
        public Material moveRangeMaterial;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<Player>(entity);
            dstManager.AddComponentData<MovementRequest>(entity, new MovementRequest
            {
                Direction = float2.zero,
                Strength = 0.0f
            });
            dstManager.AddComponentData<PlayerMoveRange>(entity, new PlayerMoveRange
            {
                Radius = 1.0f
            });
            dstManager.AddSharedComponentData<RenderMesh>(entity, new RenderMesh
            {
                mesh = moveRangeMesh,
                material = moveRangeMaterial
            });
            dstManager.AddComponentData<RenderBounds>(entity, new RenderBounds());
            dstManager.AddComponentData<LocalToWorld>(entity, new LocalToWorld());
            dstManager.AddComponentData<Translation>(entity, new Translation());
        }

        private void OnDrawGizmos()
        {
            var p = transform.position;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(p, Vector3.one);
        }
    }
}
