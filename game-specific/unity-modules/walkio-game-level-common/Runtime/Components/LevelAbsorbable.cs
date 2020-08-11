namespace JoyBrick.Walkio.Game.Level
{
    using Unity.Entities;

    public struct LevelAbsorbable : IComponentData
    {
        //
        public int Kind;

        //
        public bool TriggerCooldown;
        public float IntervalMax;
        public float Countdown;

        //
        public Entity AttachedEntity;
    }
}
