namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Mathematics;

    [RequireComponentTag(typeof(ChaseTarget))]
    public struct ChaseTargetProperty : IComponentData
    {
        public int BelongToGroup;

        public int2 AtTileIndex;
        public int2 AtTileCellIndex;

        public Entity AtFlowFieldTile;
    }
}
