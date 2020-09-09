namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    public struct LeadingToSetPlaceholderTileBuffer : IBufferElementData
    {
        public Entity Value;

        public static implicit operator Entity(LeadingToSetPlaceholderTileBuffer b) => b.Value;
        public static implicit operator LeadingToSetPlaceholderTileBuffer(Entity v) => new LeadingToSetPlaceholderTileBuffer { Value = v };
    }
}
