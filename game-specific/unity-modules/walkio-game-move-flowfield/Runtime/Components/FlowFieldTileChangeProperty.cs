namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Mathematics;

    public struct FlowFieldTileChangeProperty : IComponentData
    {
        public int TeamId;
        public int ToTileIndex;
        public Entity ToTileEntity;
        public int TimeTick;
        public float3 TargetPosition;
    }
}
