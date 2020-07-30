namespace JoyBrick.Walkio.Game.Move.Waypoint
{
    using Unity.Entities;
    using Unity.Mathematics;

    public struct WaypointPathBlobAsset
    {
        public BlobArray<WaypointPathIndexPair> WaypointPathIndexPairs;
        public BlobArray<float3> Waypoints;
    }
}
