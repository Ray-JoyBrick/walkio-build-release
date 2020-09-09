namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using Unity.Entities;
    using Unity.Mathematics;

    public struct NearbyParticleDetail
    {
        public Entity Entity;

        public float3 Position;
        public float3 Direction;
        public float Force;

        public float Mass;

        public float3 Velocity;

        public float Radius;

    }
    
    public struct ParticleNearbyBuffer : IBufferElementData
    {
        // This Value has overload meaning in different context
        public NearbyParticleDetail Value;
        public static implicit operator NearbyParticleDetail(ParticleNearbyBuffer b) => b.Value;
        public static implicit operator ParticleNearbyBuffer(NearbyParticleDetail v) => new ParticleNearbyBuffer { Value = v };
    }
}
