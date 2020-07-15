namespace JoyBrick.Walkio.Game.Move.FlowField.Common
{
    using Unity.Entities;

    public interface IFlowFieldWorldProvider
    {
        // This entity should have the following components
        // - FlowFieldWorld
        // - FlowFieldWorldProperty
        Entity FlowFieldWorldEntity { get; }
    }
}
