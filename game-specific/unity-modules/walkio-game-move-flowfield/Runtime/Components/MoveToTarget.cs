namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    public struct MoveToTarget : IComponentData
    {
        public Entity AtTile;
    }
}
