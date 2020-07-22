namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    [RequireComponentTag(typeof(PathPointFound))]
    public struct PathPointFoundProperty : IComponentData
    {
        public int GroupId;
    }
}
