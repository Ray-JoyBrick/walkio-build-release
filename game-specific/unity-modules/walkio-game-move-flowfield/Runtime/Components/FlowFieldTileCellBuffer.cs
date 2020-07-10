namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    public struct FlowFieldTileCellBuffer : IBufferElementData
    {
        // The index is the world cell index 
        public int Index;
        public static implicit operator int(FlowFieldTileCellBuffer b) => b.Index;
        public static implicit operator FlowFieldTileCellBuffer(int v) => new FlowFieldTileCellBuffer { Index = v };
    }
}
