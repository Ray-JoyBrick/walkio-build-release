namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Mathematics;

    public struct LeadingToTileContent
    {
        public int2 TileIndex;
        public Entity Tile;
    }

    public struct LeadingToTileBuffer : IBufferElementData
    {
        public LeadingToTileContent Value;

        public static implicit operator LeadingToTileContent(LeadingToTileBuffer b) => b.Value;
        public static implicit operator LeadingToTileBuffer(LeadingToTileContent v) => new LeadingToTileBuffer { Value = v };

        // public Entity Value;
        //
        // public static implicit operator Entity(LeadingToTileBuffer b) => b.Value;
        // public static implicit operator LeadingToTileBuffer(Entity v) => new LeadingToTileBuffer { Value = v };
    }
}
