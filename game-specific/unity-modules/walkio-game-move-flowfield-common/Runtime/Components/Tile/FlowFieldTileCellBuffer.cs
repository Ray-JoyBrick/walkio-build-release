namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    public struct TileCellContent
    {
        public int CellIndex;
        public int BaseCost;
        public int Direction;
    }
    
    public struct FlowFieldTileCellBuffer : IBufferElementData
    {
        // This Value has overload meaning in different context
        public TileCellContent Value;
        public static implicit operator TileCellContent(FlowFieldTileCellBuffer b) => b.Value;
        public static implicit operator FlowFieldTileCellBuffer(TileCellContent v) => new FlowFieldTileCellBuffer { Value = v };
    }
}
