namespace JoyBrick.Walkio.Game.Creature.Assist
{
    using Unity.Entities;

    public class TeamUnitSpawnTimerProperty : IComponentData
    {
        public float IntervalMax;
        public float CountDown;
    }
}
