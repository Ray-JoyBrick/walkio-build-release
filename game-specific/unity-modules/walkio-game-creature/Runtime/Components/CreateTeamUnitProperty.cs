namespace JoyBrick.Walkio.Game.Creature
{
    using Unity.Entities;
    using Unity.Mathematics;

    public class CreateTeamUnitProperty : IComponentData
    {
        public int TeamId;
        public float3 AtPosition;
    }
}
