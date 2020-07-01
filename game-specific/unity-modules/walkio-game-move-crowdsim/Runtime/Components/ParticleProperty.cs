namespace JoyBrick.Walkio.Game.Move.CrowdSim
{
    using Unity.Entities;
    using Unity.Mathematics;

    public struct ParticleProperty : IComponentData
    {
        public float Mass;
        public float InverseDensity;

        public float3 Position;
        public float3 Velocity;

        public float Pressure;
        
        public ParticleProperty(float mass, float inverseDensity)
        {
            //
            Mass = mass;
            InverseDensity = inverseDensity;

            Position = float3.zero;
            Velocity = float3.zero;
            Pressure = 0;
        }

        public static int stride = sizeof(float) * 27 + sizeof(int) * 2;
    }
}
