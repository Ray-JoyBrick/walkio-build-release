namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    public struct DiscardedFlowFieldTileProperty : IComponentData
    {
        public float IntervalMax;
        public float CountDown;
    }
}
