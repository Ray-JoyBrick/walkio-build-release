namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    public struct FlowFieldTile : IComponentData
    {
        public int Index;
        
        public int HorizontalCount;
        public int VerticalCount;

        public int TimeTick;

        public Entity NextFlowFieldTile;
    }
}
