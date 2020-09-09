namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Mathematics;

    public struct PathPointBuffer : IBufferElementData
    {
        public float3 Value;

        public static implicit operator float3(PathPointBuffer b) => b.Value;
        public static implicit operator PathPointBuffer(float3 v) => new PathPointBuffer { Value = v };
    }
}
