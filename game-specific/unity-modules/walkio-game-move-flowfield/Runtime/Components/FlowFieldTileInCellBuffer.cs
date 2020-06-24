namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    public struct FlowFieldTileInCellBuffer : IBufferElementData
    {
        public int Index;
        public static implicit operator int(FlowFieldTileInCellBuffer b) => b.Index;
        public static implicit operator FlowFieldTileInCellBuffer(int v) => new FlowFieldTileInCellBuffer { Index = v };
    }
}
