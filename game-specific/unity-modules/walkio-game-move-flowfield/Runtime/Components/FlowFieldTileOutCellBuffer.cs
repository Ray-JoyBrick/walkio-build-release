namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    public struct FlowFieldTileOutCellBuffer : IBufferElementData
    {
        public int Index;
        public static implicit operator int(FlowFieldTileOutCellBuffer b) => b.Index;
        public static implicit operator FlowFieldTileOutCellBuffer(int v) => new FlowFieldTileOutCellBuffer { Index = v };
    }
}
