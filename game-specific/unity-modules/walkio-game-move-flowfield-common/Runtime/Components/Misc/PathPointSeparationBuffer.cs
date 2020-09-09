namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Mathematics;

    public struct PathPointSeparationBuffer : IBufferElementData
    {
        public int2 Value;

        public static implicit operator int2(PathPointSeparationBuffer b) => b.Value;
        public static implicit operator PathPointSeparationBuffer(int2 v) => new PathPointSeparationBuffer { Value = v };
    }
}
