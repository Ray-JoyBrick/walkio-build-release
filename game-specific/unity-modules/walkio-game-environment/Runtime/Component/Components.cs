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
    public struct TheEnvironment : IComponentData
    {
        public float2 GridCellSize;
        
        public float2 TileCellSize;
        public int2 TileCellCount;
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
    
    //
    public struct GenerateZone : IComponentData
    {
    }
    
    public struct GenerateZoneProperty : IComponentData
    {
        public int Width;
        public int Height;
    }
    
    public struct Zone : IComponentData
    {
        public int Width;
        public int Height;
    }

    public struct ZoneGridCellBuffer : IBufferElementData
    {
        public int Index;
        public static implicit operator int(ZoneGridCellBuffer b) => b.Index;
        public static implicit operator ZoneGridCellBuffer(int v) => new ZoneGridCellBuffer { Index = v };
    }
    
    //
    public struct WaypointPath
    {
        public int StartIndex;
        public int EndIndex;
    }

    public struct WaypointPathBlobAsset
    {
        public BlobArray<WaypointPath> WaypointPaths;
        public BlobArray<float3> Waypoints;
    }
    
    // public struct WaypointPath
    // {
    //     public int StartIndex;
    //     public int EndIndex;
    // }

    public struct GridMapBlobAsset
    {
        // public BlobArray<WaypointPath> WaypointPaths;
        public BlobArray<int> Indices;
    }

    public struct LevelWaypointPathLookup : IComponentData
    {
        public BlobAssetReference<WaypointPathBlobAsset> WaypointPathBlobAssetRef;
    }

    public struct LevelGridMapLookup : IComponentData
    {
        public BlobAssetReference<GridMapBlobAsset> GridMapBlobAssetRef;
    }
    
    

    //
    public struct GridCellDetail
    {
        public int Kind;
        public int Cost;
    }

    public struct GridCellDetailBlobAsset
    {
        public BlobArray<GridCellDetail> GridCellDetails;
    }

    // public struct TileDataPlaceholder : IComponentData
    // {
    // }

    public struct GridCellDetailIndexBlobAsset
    {
        public BlobArray<int> GridCellDetailIndices;
    }
    
    public struct ZoneGridCellDetailLookup : IComponentData
    {
        public BlobAssetReference<GridCellDetailBlobAsset> GridCellDetailBlobAssetRef;
    }
    
    public struct ZoneGridCellDetailIndexLookup : IComponentData
    {
        public BlobAssetReference<GridCellDetailIndexBlobAsset> GridCellDetailIndexBlobAssetRef;
    }

    public struct GeneratePathfind : IComponentData
    {
    }

    public struct PathfindBoard : IComponentData
    {
        public int HorizontalCount;
        public int VerticalCount;
    }

    public struct PathfindBoardBuffer : IBufferElementData
    {
        public Entity Value;
     
        public static implicit operator Entity(PathfindBoardBuffer b) => b.Value;
        public static implicit operator PathfindBoardBuffer(Entity v) => new PathfindBoardBuffer { Value = v };
    }

    public struct PathfindTile : IComponentData
    {
        public int Index;
        public int2 Index2d;
        
        public int HorizontalCount;
        public int VerticalCount;
    }

    public struct PathfindTileBuffer : IBufferElementData
    {
        public int Value;

        public static implicit operator int(PathfindTileBuffer b) => b.Value;
        public static implicit operator PathfindTileBuffer(int v) => new PathfindTileBuffer { Value = v };
    }

    // TODO: Move this to other module, Battle?
    public struct Team : IComponentData
    {
        public int Id;
    }

    public struct TeamLeader : IComponentData
    {
        public int Id;
    }

    public struct Unit : IComponentData
    {
        public int Index;
    }

    public struct PathfindGroup : IComponentData
    {
        public int Index;
    }

    // Entity of FlowFieldTile
    public struct FlowFieldTile : IComponentData
    {
        public int HorizontalCount;
        public int VerticalCount;        
    }

    public struct FlowFieldTileCellBuffer : IBufferElementData
    {
        public int Index;
        public static implicit operator int(FlowFieldTileCellBuffer b) => b.Index;
        public static implicit operator FlowFieldTileCellBuffer(int v) => new FlowFieldTileCellBuffer { Index = v };
    }

    public struct FlowFieldTileInCellBuffer : IBufferElementData
    {
        public int Index;
        public static implicit operator int(FlowFieldTileInCellBuffer b) => b.Index;
        public static implicit operator FlowFieldTileInCellBuffer(int v) => new FlowFieldTileInCellBuffer { Index = v };
    }

    public struct FlowFieldTileOutCellBuffer : IBufferElementData
    {
        public int Index;
        public static implicit operator int(FlowFieldTileOutCellBuffer b) => b.Index;
        public static implicit operator FlowFieldTileOutCellBuffer(int v) => new FlowFieldTileOutCellBuffer { Index = v };
    }

    // Entity of FlowFieldTilePath
    public struct FlowFieldTilePath : IComponentData
    {
        public int GoalIndex;
    }

    public struct FlowFieldTileBuffer : IBufferElementData
    {
        public Entity Value;

        public static implicit operator Entity(FlowFieldTileBuffer b) => b.Value;
        public static implicit operator FlowFieldTileBuffer(Entity v) => new FlowFieldTileBuffer { Value = v };
    }
}
