namespace JoyBrick.Walkio.Game.Move.CrowdSim
{
    using Unity.Entities;

    [DisableAutoCreation]
    [UpdateBefore(typeof(CrowdSimSystem))]
    public class RadixSortSystem : SystemBase
    {
        protected override void OnUpdate()
        {
        }
    }
}
