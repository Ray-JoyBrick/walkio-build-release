namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    [RequireComponentTag(typeof(MonitorTileChange))]
    public struct MonitorTileChangeProperty : IComponentData
    {
        public bool CanMonitor;
    }
}
