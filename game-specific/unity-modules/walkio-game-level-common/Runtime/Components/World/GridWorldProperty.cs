namespace JoyBrick.Walkio.Game.Level
{
    using Unity.Entities;
    using Unity.Mathematics;

    [RequireComponentTag(typeof(GridWorld))]
    public struct GridWorldProperty : IComponentData
    {
        public int2 CellCount;
        public float2 CellSize;

        public int2 GridTileCount;
        public int2 GridTileCellCount;
        
        public BlobAssetReference<GridMapBlobAsset> GridMapBlobAssetRef;
    }
}
