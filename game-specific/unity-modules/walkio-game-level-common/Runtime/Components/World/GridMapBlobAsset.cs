namespace JoyBrick.Walkio.Game.Level
{
    using Unity.Entities;

    public struct GridMapContext
    {
        public int Index;
    }

    public struct GridMapBlobAsset
    {
        // public BlobArray<GridMapContext> GridMapContextArray;
        public BlobArray<int> GridMapContextArray;
        // public BlobArray<FlowFieldTile> FlowFieldTiles;
    }
}
