namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    [RequireComponentTag(typeof(ChaseTarget))]
    public struct ChaseTargetProperty : IComponentData
    {
        public int BelongToGroup;
    }
}
