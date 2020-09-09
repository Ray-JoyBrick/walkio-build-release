namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    // This is used so that other flow field move unit can follow to move to this target.
    public struct MoveToTarget : IComponentData
    {
        // This is served as cache so the tile entity can be used quickly
        public Entity AtTile;
    }
}
