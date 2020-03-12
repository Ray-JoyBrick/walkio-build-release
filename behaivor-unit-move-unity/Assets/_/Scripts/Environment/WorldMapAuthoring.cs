namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;
    using Unity.Rendering;
    using Unity.Transforms;
    using UnityEngine;

    public class WorldMapAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public Mesh mesh;
        public Material material;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddSharedComponentData(entity, new RenderMesh
            {
                mesh = mesh,
                material = material
            });
            dstManager.AddComponentData(entity, new RenderBounds());
            dstManager.AddComponentData(entity, new LocalToWorld());
            dstManager.AddComponentData(entity, new Translation());

            dstManager.AddComponentData(entity, new WorldMap
            {
                Width = 0,
                Height = 0
            });
        }
    }
}
