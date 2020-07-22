namespace JoyBrick.Walkio.Game.Move.FlowField.Assist
{
    using Unity.Entities;
    using Unity.Mathematics;

    [RequireComponentTag(typeof(TemporaryPointIndication))]
    public struct TemporaryPointIndicationProperty : IComponentData
    {
        public int GroupId;
        public float3 Location;

        public float IntervalMax;
        public float CountDown;
    }
}
