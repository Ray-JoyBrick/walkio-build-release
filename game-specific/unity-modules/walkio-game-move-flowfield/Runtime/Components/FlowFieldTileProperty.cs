namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    [RequireComponentTag(typeof(FlowFieldTile))]
    public struct FlowFieldTileProperty : IComponentData
    {
        public int Index;
        
        public int HorizontalCount;
        public int VerticalCount;

        public int TimeTick;

        public Entity NextFlowFieldTile;

        public override string ToString()
        {
            var desc = $"Index: {Index} TimeTick: {TimeTick}";
            return desc;
        }
    }
}
