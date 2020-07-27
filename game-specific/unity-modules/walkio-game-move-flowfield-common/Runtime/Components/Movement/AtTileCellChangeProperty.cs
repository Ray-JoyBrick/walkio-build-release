namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Mathematics;

    [RequireComponentTag(typeof(AtTileChange))]
    public struct AtTileCellChangeProperty : IComponentData
    {
        public int GroupId;
        public float3 ChangeToPosition;
        public int2 ChangeToTileIndex;

        public Entity LeadingToSetEntity;
    }
}
