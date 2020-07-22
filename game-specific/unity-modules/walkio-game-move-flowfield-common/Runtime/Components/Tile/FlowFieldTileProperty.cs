namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Mathematics;

    [RequireComponentTag(typeof(FlowFieldTile))]
    public struct FlowFieldTileProperty : IComponentData
    {
        //
        public int WorldId;
        public bool UsedAsBase;
        public int Index;
        public int GroupId;
        public int2 TileIndex;
        public int TimeTick;

        //
        public Entity NextFlowFieldTile;

        //
        public override string ToString()
        {
            var desc = $"WorldId: {WorldId} UsedAsBase: {UsedAsBase} Index: {Index} TimeTick: {TimeTick}";
            return desc;
        }
    }
}
