namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Mathematics;

    [RequireComponentTag(typeof(MoveOnFlowFieldTile))]
    public struct MoveOnFlowFieldTileProperty : IComponentData
    {
        public Entity OnTile;
        
        //
        public float Speed;
        public float3 Direction;
    }
}
