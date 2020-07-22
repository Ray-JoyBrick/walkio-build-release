namespace JoyBrick.Walkio.Game.Creature
{
    using Unity.Entities;

    public struct NeutralUnitSpawnProperty : IComponentData
    {
        public float IntervalMax;
        public float CountDown;
    }
}
