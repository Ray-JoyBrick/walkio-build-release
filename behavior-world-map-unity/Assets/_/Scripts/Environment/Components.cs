namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Entities;

    public struct LoadWorldMapRequest : IComponentData
    {
        public int WorldMapIndex;
    }

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
        Wall = 1
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
}