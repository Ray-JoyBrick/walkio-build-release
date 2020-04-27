namespace JoyBrick.Walkio.Game.Battle
{
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;

    public class TeamLeaderAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new CopyTransformFromGameObject());
        }
    }
}
