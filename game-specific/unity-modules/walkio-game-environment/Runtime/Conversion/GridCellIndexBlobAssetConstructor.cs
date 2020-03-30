namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine;

    [UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
    public class GridCellIndexBlobAssetConstructor : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Debug.Log($"GridCellIndexBlobAssetConstructor - OnUpdate");
            
            var authoring =
                GetEntityQuery(typeof(GridCellIndexBlobAssetAuthoring))
                    .ToComponentArray<GridCellIndexBlobAssetAuthoring>();

            if (authoring.Length == 0) return;

            BlobAssetReference<GridCellDetailIndexBlobAsset> gridCellDetailIndexBlobAssetReference;
            
            // I guess this update only perform at main thread
            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var gridCellDetailIndexBlobAsset = ref blobBuilder.ConstructRoot<GridCellDetailIndexBlobAsset>();
                
                var gridCellDetailIndexAuthoring = GetEntityQuery(typeof(GridCellIndexBlobAssetAuthoring)).ToComponentArray<GridCellIndexBlobAssetAuthoring>()[0];
    
                var count = gridCellDetailIndexAuthoring.indices.Count;
                var gridCellDataArray = blobBuilder.Allocate(ref gridCellDetailIndexBlobAsset.GridCellDetailIndices, count);
                for (var i = 0; i < count; ++i)
                {
                    var td = gridCellDetailIndexAuthoring.indices[i];
                    gridCellDataArray[i] = td;
                }
    
                gridCellDetailIndexBlobAssetReference = blobBuilder.CreateBlobAssetReference<GridCellDetailIndexBlobAsset>(Allocator.Persistent);
            }
    
            var zoneQuery = DstEntityManager.CreateEntityQuery(typeof(Zone));
            var zoneEntity = zoneQuery.GetSingletonEntity();
            
            DstEntityManager.AddComponentData(zoneEntity, new ZoneGridCellDetailIndexLookup
            {
                GridCellDetailIndexBlobAssetRef = gridCellDetailIndexBlobAssetReference
            });
        }
    }
}
