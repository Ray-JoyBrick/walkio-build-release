namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;

    // public struct LoadWorldMapRequest : IComponentData
    // {
    //     public int WorldMapIndex;
    // }

    public struct GenerateWorldMap : IComponentData
    {
    }

    public struct GenerateWorldMapProperty : IComponentData
    {
        public int Width;
        public int Height;
    }

    public struct WorldMap : IComponentData
    {
        public int Width;
        public int Height;
    }

    public enum TileType : int
    {
        Floor = 0,
        Wall = 1,
        OutBoundary = 2,
        Mine = 3,
        Lave = 4
    }

    public struct WorldMapTileBuffer : IBufferElementData
    {
        public TileType Value;
        public static implicit operator TileType(WorldMapTileBuffer b) => b.Value;
        public static implicit operator WorldMapTileBuffer(TileType v) => new WorldMapTileBuffer { Value = v };
    }

    public struct TileDetailAdjustmentBuffer : IBufferElementData
    {
        public int2 Value;
        public static implicit operator int2(TileDetailAdjustmentBuffer b) => b.Value;
        public static implicit operator TileDetailAdjustmentBuffer(int2 v) => new TileDetailAdjustmentBuffer { Value = v };
    }

    public struct GenerateVisualWorldMap : IComponentData
    {
        public int Value;
    }
    
    public struct VisualWorldMap : IComponentData
    {
    }
    
    public struct GenerateDiagnosticWorldMap : IComponentData
    {
        public int Value;
    }

    public struct DiagnosticWorldMap : IComponentData
    {
    }
    
    //
    public struct Environment : IComponentData
    {
    }

    public struct WorldMapTileDetailLookup : IComponentData
    {
        public BlobAssetReference<TileDetailBlobAsset> TileDetailBlobAssetRef;
    }

    public struct WorldMapTileDetailIndexLookup : IComponentData
    {
        public BlobAssetReference<TileDetailIndexBlobAsset> TileDetailIndexBlobAssetRef;
    }
    
    //
    public struct TileDetail
    {
        public TileType Type;
        public int Cost;
    }

    public struct TileDetailBlobAsset
    {
        public BlobArray<TileDetail> TileDetails;
    }

    public struct TileDataPlaceholder : IComponentData
    {
    }

    public struct TileDetailIndexBlobAsset
    {
        public BlobArray<int> TileDetailIndices;
    }

    public struct RemoveAfterConversion : IComponentData
    {
        
    }
}