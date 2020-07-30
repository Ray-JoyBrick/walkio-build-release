namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using Unity.Entities;
    using Unity.Mathematics;

    public struct NearbyParticleDetail
    {
        public float3 Position;
        public float3 Velocity;

        public float Radius;
    }
    
    public struct NearbyParticleBuffer : IBufferElementData
    {
        // This Value has overload meaning in different context
        public NearbyParticleDetail Value;
        public static implicit operator NearbyParticleDetail(NearbyParticleBuffer b) => b.Value;
        public static implicit operator NearbyParticleBuffer(NearbyParticleDetail v) => new NearbyParticleBuffer { Value = v };
    }
}
