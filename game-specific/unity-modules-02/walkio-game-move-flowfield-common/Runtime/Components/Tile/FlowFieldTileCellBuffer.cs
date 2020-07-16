namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    public struct FlowFieldTileCellBuffer : IBufferElementData
    {
        // This Value has overload meaning in different context
        public int Value;
        public static implicit operator int(FlowFieldTileCellBuffer b) => b.Value;
        public static implicit operator FlowFieldTileCellBuffer(int v) => new FlowFieldTileCellBuffer { Value = v };
    }
}
