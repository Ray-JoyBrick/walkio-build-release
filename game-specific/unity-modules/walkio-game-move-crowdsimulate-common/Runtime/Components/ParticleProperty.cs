namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using Unity.Entities;
    using Unity.Mathematics;

    [RequireComponentTag(typeof(Particle))]
    public struct ParticleProperty : IComponentData
    {
        public float K;
        public float T0;
        public float M;
        public float Ksi;
        public float MaxAcceleration;
        public float PrefSpeed;

        // public float Radius;
        public float3 Velocity;
        
        public float3 ForcePhysic;

        public float3 PrefVelocity;

        //
        public float3 Direction;
        public float Force;

        //
        public float3 Position;
        public float Mass;
        public float InverseDensity;
        public float Pressure;
        public float3 ForcePressure;
        public float3 ForceViscosity;
        public float3 ForceTension;

        public float ParticleRadius;
        public float SmoothRadius;
        public float SmoothRadiusSquare;
        public float RestDensity;
        public float GravityMultiplier;
        public float ParticleMass;
        public float ParticleViscosity;
        public float ParticleDrag;
        
        public float Density;

        public static unsafe int Stride => sizeof(ParticleProperty);
    }
}
