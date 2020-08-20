namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public class CrowdSimulateMoveAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public float k;
        public float t0;
        public float m;
        public float ksi;
        public float speed;
        public float radius;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Particle());
            dstManager.AddComponentData(entity, new ParticleProperty
            {
                K = k,
                T0 = t0,
                M = m,
                Ksi = ksi,
                PrefSpeed = speed,
                Radius = radius
            });
            dstManager.AddBuffer<ParticleNearbyBuffer>(entity);
        }
    }
}
