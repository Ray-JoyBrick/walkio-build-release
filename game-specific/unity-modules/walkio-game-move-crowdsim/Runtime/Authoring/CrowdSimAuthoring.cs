namespace JoyBrick.Walkio.Game.Move.CrowdSim
{
    using Unity.Entities;
    using UnityEngine;

    public class CrowdSimAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Particle());
            dstManager.AddComponentData(entity, new ParticleProperty());
        }
    }
}
