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

        public float Radius;
        public float3 Velocity;

        public float3 PrefVelocity;

        public float3 Force;
    }
}
