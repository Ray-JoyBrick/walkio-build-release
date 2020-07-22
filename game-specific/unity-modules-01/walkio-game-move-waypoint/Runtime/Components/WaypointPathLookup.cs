namespace JoyBrick.Walkio.Game.Move.Waypoint
{
    using Unity.Entities;

    public struct WaypointPathLookup : IComponentData
    {
        public BlobAssetReference<WaypointPathBlobAsset> WaypointPathBlobAssetRef;
    }
}
