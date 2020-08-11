namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Mathematics;

    [RequireComponentTag(typeof(PathPointFound))]
    public struct PathPointFoundProperty : IComponentData
    {
        public int GroupId;

        // public Entity ForWhichLeader;

        public int2 AtTileIndex;
        public Entity AtTileEntity;
    }
}
