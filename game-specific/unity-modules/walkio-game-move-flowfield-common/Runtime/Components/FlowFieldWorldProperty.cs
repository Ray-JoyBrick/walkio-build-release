namespace JoyBrick.Walkio.Game.Move.FlowField.Common
{
    using Unity.Entities;
    using Unity.Mathematics;

    [RequireComponentTag(typeof(FlowFieldWorld))]
    public struct FlowFieldWorldProperty : IComponentData
    {
        public int2 TileCount;
        public int2 TileCellCount;
        
        public BlobAssetReference<FlowFieldTileBlobAsset> FlowFieldTileBlobAssetRef;

        public override string ToString()
        {
            var desc = $"TileCount: {TileCount} TileCellCount: {TileCellCount}";

            return desc;
        }
    }
}
