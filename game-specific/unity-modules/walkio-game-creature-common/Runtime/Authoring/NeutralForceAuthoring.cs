namespace JoyBrick.Walkio.Game.Creature
{
    using Unity.Entities;
    using UnityEngine;

    public class NeutralForceAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new NeutralForce());
        }
    }
}
