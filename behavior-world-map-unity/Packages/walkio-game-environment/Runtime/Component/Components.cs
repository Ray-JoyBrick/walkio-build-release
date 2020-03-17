namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Collections;
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

    public class TileDataBlobAssetConstructor : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            BlobAssetReference<TileDataBlobAsset> tileDataBlobAssetReference;
            
            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var tileDataBlobAsset = ref blobBuilder.ConstructRoot<TileDataBlobAsset>();
                var tileDataArray = blobBuilder.Allocate(ref tileDataBlobAsset.TileDatas, 3);

                tileDataArray[0] = new TileData {Type = TileType.Floor, Cost = 0};
                tileDataArray[1] = new TileData {Type = TileType.Wall, Cost = 0};

                tileDataBlobAssetReference = blobBuilder.CreateBlobAssetReference<TileDataBlobAsset>(Allocator.Persistent);
            }

            var environmentQuery = DstEntityManager.CreateEntityQuery(typeof(Environment));
            var environmentEntity = environmentQuery.GetSingletonEntity();

            DstEntityManager.AddComponentData(environmentEntity, new WorldMapTileLookup
            {
                TileDataBlobAssetRef = tileDataBlobAssetReference
            });
        }
    }
}