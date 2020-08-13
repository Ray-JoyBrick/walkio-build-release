namespace JoyBrick.Walkio.Game.Level
{
    using Unity.Entities;
    using Unity.Mathematics;

    public struct LevelAbsorbable : IComponentData
    {
        //
        public int Kind;

        public int GroupId;

        //
        public bool TriggerCooldown;
        public float IntervalMax;
        public float Countdown;
        
        //
        public float3 HitPosition; 

        //
        public Entity AttachedEntity;
    }
}
