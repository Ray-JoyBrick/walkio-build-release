namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;
    using Unity.Mathematics;

    public struct Team : IComponentData
    {
        public int Id;
    }

    public struct Unit : IComponentData
    {
    }

    public struct UnitMoveToTarget : IComponentData
    {
        public float3 Target;
        public float MoveSpeed;
    }
    
    public struct UnitRequestNewTarget : IComponentData
    {
    }    
}
