namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Mathematics;

    public struct LeadingToSetPlaceholderProperty : IComponentData
    {
        public int GroupId;
        public float3 ChangeToPosition;
        public int2 TileIndex;
    }
}
