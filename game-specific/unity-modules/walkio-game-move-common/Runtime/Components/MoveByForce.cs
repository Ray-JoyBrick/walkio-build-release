namespace JoyBrick.Walkio.Game.Move
{
    using Unity.Entities;
    using Unity.Mathematics;

    public struct MoveByForce : IComponentData
    {
        public float3 Direction;
        public float Force;
    }
}
