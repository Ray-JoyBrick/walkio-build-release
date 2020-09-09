namespace JoyBrick.Walkio.Game.Move.Waypoint
{
    using Unity.Entities;

    public struct MoveOnWaypointPathProperty : IComponentData
    {
        public int StartPathIndex;
        public int EndPathIndex;

        public int AtIndex;
    }
}
