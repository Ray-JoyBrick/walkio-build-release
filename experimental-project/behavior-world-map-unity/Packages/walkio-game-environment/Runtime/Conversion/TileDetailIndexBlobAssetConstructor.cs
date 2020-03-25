namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine;

    [UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
    public class TileDetailIndexBlobAssetConstructor : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Debug.Log($"TileDetailIndexBlobAssetConstructor - OnUpdate");
            
            BlobAssetReference<TileDetailIndexBlobAsset> tileDataIndexBlobAssetReference;
            
            // I guess this update only perform at main thread
            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var tileDetailIndexBlobAsset = ref blobBuilder.ConstructRoot<TileDetailIndexBlobAsset>();
                
                var tileDetailIndexAuthoring = GetEntityQuery(typeof(TileDetailIndexBlobAssetAuthoring)).ToComponentArray<TileDetailIndexBlobAssetAuthoring>()[0];
    
                var count = tileDetailIndexAuthoring.indices.Count;
                var tileDataArray = blobBuilder.Allocate(ref tileDetailIndexBlobAsset.TileDetailIndices, count);
                for (var i = 0; i < count; ++i)
                {
                    var td = tileDetailIndexAuthoring.indices[i];
                    tileDataArray[i] = td;
                }
    
                tileDataIndexBlobAssetReference = blobBuilder.CreateBlobAssetReference<TileDetailIndexBlobAsset>(Allocator.Persistent);
            }
    
            var environmentQuery = DstEntityManager.CreateEntityQuery(typeof(Environment));
            var environmentEntity = environmentQuery.GetSingletonEntity();
            
            DstEntityManager.AddComponentData(environmentEntity, new WorldMapTileDetailIndexLookup
            {
                TileDetailIndexBlobAssetRef = tileDataIndexBlobAssetReference
            });
        }
    }
}