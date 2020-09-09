namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    public class ParticleAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public float mass;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Particle());

            var onSphereDirection = UnityEngine.Random.onUnitSphere;
            var direction = new Vector3(onSphereDirection.x, 0, onSphereDirection.z);
            var adjustedDirection = (float3)direction.normalized;
            var force = UnityEngine.Random.Range(1.0f, 10.0f);
            dstManager.AddComponentData(entity, new ParticleProperty
            {
                Position = (float3)transform.position,
                Direction = adjustedDirection,
                Force = force,

                Mass = mass,
                
                ParticleRadius = 1.0f,
                SmoothRadius = 1.0f,
                SmoothRadiusSquare = 1.0f,
                RestDensity = 30,
                GravityMultiplier = 2000,
                ParticleMass = 1.0f,
                ParticleViscosity = 0.1f,
                ParticleDrag = 0.025f
            });

            var buffer = dstManager.AddBuffer<ParticleNearbyBuffer>(entity);
        }
    }
}
