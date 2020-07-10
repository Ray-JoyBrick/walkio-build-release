namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    // Group does not mean to be team.
    // It is possible that a team contains several groups
    public struct FlowFieldGroup : IComponentData
    {
        public int GroupId;
    }
}
