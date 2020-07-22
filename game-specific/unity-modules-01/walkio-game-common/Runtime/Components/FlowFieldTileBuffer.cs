namespace JoyBrick.Walkio.Game.Common
{
    using Unity.Entities;

    public struct FlowFieldTileBuffer : IBufferElementData
    {
        public Entity Value;

        public static implicit operator Entity(FlowFieldTileBuffer b) => b.Value;
        public static implicit operator FlowFieldTileBuffer(Entity v) => new FlowFieldTileBuffer { Value = v };
    }
}
