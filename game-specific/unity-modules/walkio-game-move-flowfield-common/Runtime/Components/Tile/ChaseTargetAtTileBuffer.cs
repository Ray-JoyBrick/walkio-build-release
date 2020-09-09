namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

   
    public struct ChaseTargetAtTileBuffer : IBufferElementData
    {
        // This Value has overload meaning in different context
        public Entity Value;
        public static implicit operator Entity(ChaseTargetAtTileBuffer b) => b.Value;
        public static implicit operator ChaseTargetAtTileBuffer(Entity v) => new ChaseTargetAtTileBuffer { Value = v };
    }
}
