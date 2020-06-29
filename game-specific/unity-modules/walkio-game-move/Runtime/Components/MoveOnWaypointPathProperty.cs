namespace JoyBrick.Walkio.Game.Move
{
    using Unity.Entities;

    public struct MoveOnWaypointPathProperty : IComponentData
    {
        public int StartPathIndex;
        public int EndPathIndex;

        public int AtIndex;
    }
}
