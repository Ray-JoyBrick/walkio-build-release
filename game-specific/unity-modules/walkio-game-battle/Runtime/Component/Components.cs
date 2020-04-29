namespace JoyBrick.Walkio.Game.Battle
{
    using Unity.Entities;

    public struct UnitSpawner : IComponentData
    {
        public Entity Unit;
    }

    public struct UnitSpawnStyle : IComponentData
    {
        public float IntervalMax;
        public float CountDown;
    }
    
    public struct Unit : IComponentData
    {
    }

    public struct NeutralForce : IComponentData
    {
    }

    public struct TeamForce : IComponentData
    {
    }

    public struct MoveOnPath : IComponentData
    {
        public int StartIndex;
        public int EndIndex;

        public int AtIndex;
    }
    
    //
    public struct UnitBuffer : IBufferElementData
    {
        public Entity Value;
        
        public static implicit operator Entity(UnitBuffer b) => b.Value;
        public static implicit operator UnitBuffer(Entity v) => new UnitBuffer { Value = v };
    }
}
