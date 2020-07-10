namespace JoyBrick.Walkio.Game.Common
{
    using Unity.Entities;

    public struct MakeMoveSpecificSetupProperty : IComponentData
    {
        public bool FlowFieldMoveSetup;

        public int TeamId;
    }
}
