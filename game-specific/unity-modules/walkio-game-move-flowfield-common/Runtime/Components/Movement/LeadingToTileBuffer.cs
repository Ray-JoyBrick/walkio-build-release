namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    public struct LeadingToTileBuffer : IBufferElementData
    {
        public Entity Value;

        public static implicit operator Entity(LeadingToTileBuffer b) => b.Value;
        public static implicit operator LeadingToTileBuffer(Entity v) => new LeadingToTileBuffer { Value = v };
    }
}
