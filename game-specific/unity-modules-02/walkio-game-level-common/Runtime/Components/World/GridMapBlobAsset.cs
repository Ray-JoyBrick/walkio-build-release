namespace JoyBrick.Walkio.Game.Level
{
    using Unity.Entities;

    public struct GridMapContext
    {
    }
    
    public struct GridMapBlobAsset
    {
        public BlobArray<GridMapContext> FlowFieldTileContextArray;
        // public BlobArray<FlowFieldTile> FlowFieldTiles;
    }
}
