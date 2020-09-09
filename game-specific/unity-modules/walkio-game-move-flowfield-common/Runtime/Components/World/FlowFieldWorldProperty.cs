namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Mathematics;

    [RequireComponentTag(typeof(FlowFieldWorld))]
    public struct FlowFieldWorldProperty : IComponentData
    {
        //
        public int Id;
        public int2 TileCount;
        public int2 TileCellCount;
        public float2 TileCellSize;

        //
        public float3 OriginPosition;
        public float3 PositionOffset;

        //
        public BlobAssetReference<FlowFieldTileBlobAsset> FlowFieldTileBlobAssetRef;

        //
        public override string ToString()
        {
            var desc = $"TileCount: {TileCount} TileCellCount: {TileCellCount} TileCellSize: {TileCellSize}";

            return desc;
        }
    }
}
