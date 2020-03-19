namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Collections;
    using Unity.Entities;

    // public struct LoadWorldMapRequest : IComponentData
    // {
    //     public int WorldMapIndex;
    // }

    public struct GenerateWorldMap : IComponentData
    {
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

    public struct WorldMapTileLookup : IComponentData
    {
        public BlobAssetReference<TileDataBlobAsset> TileDataBlobAssetRef;
    }
    
    //
    public struct TileData
    {
        public TileType Type;
        public int Cost;
    }

    public struct TileDataBlobAsset
    {
        public BlobArray<TileData> TileDatas;
    }

    public struct TileDataPlaceholder : IComponentData
    {
    }
}