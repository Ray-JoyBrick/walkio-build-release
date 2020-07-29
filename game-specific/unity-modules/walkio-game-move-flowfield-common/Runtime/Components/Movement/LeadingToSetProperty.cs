namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Mathematics;

    [RequireComponentTag(typeof(LeadingToSet))]
    public struct LeadingToSetProperty : IComponentData
    {
        public int GroupId;

        public int2 TileIndex;
        public Entity LeadingToTile;
    }
}
