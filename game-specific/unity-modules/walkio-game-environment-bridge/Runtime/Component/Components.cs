namespace JoyBrick.Walkio.Game.Environment.Creature
{
    using Unity.Entities;

    //
    public struct TeamForce : IComponentData
    {
    }

    public struct NeutralForce : IComponentData
    {
    }

    public struct Unit : IComponentData
    {
    }

    public struct Leader : IComponentData
    {
    }

    // Movement use
    
    // Waypoint
    public struct MoveOnWaypointPath : IComponentData
    {
        public int StartIndex;
        public int EndIndex;

        public int AtIndex;
    }

    // Flow Field
    public struct MonitorFlowFieldTileChange : IComponentData
    {
    }

    public struct FlowFieldGoal : IComponentData
    {
    }

    public struct MoveOnFlowFieldTile : IComponentData
    {
    }

    public struct MoveOnFlowFieldTileInfo : IComponentData
    {
        public Entity OnTile;
    }
}
