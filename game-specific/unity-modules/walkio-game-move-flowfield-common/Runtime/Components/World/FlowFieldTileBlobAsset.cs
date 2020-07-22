namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;

    public struct FlowFieldTileContext
    {
        
    }

    // public struct FlowFieldTile : IComponentData
    // {

    // }
    
    // Buffer might be easier
    public struct FlowFieldTileBlobAsset
    {
        public BlobArray<FlowFieldTileContext> FlowFieldTileContextArray;
        // public BlobArray<FlowFieldTile> FlowFieldTiles;
    }
}
