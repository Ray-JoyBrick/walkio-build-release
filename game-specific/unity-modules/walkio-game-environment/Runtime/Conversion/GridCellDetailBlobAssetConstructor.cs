namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine;

    [UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
    public class GridCellDetailBlobAssetConstructor : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            var authoring =
                GetEntityQuery(typeof(GridCellDetailBlobAssetAuthoring))
                    .ToComponentArray<GridCellDetailBlobAssetAuthoring>();

            if (authoring.Length == 0) return;
            Debug.Log($"GridCellDetailBlobAssetConstructor - OnUpdate");
            
            BlobAssetReference<GridCellDetailBlobAsset> gridCellDataBlobAssetReference;
            
            // I guess this update only perform at main thread
            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var gridCellDetailBlobAsset = ref blobBuilder.ConstructRoot<GridCellDetailBlobAsset>();
                
                // TileDataBlobAssetAuthoring itself is MonBehaviour + IConvertGameObjectToEntity and once being
                // converted into entity, that scriptable object which is ScriptableObject + IComponentData
                // becoming part of the entity.
                // The question is, can this conversion takes place only when ScriptableObject is loaded?
                var gridCellDetailAuthoring = GetEntityQuery(typeof(GridCellDetailBlobAssetAuthoring)).ToComponentArray<GridCellDetailBlobAssetAuthoring>()[0];
    
                // var tileDataArray = blobBuilder.Allocate(ref tileDataBlobAsset.TileDatas, 2);
                //
                // tileDataArray[0] = new TileData {Type = TileType.Floor, Cost = 0};
                // tileDataArray[1] = new TileData {Type = TileType.Wall, Cost = 0};
    
                var count = gridCellDetailAuthoring.gridCellDetails.Count;
                var gridCellDataArray = blobBuilder.Allocate(ref gridCellDetailBlobAsset.GridCellDetails, count);
                for (var i = 0; i < count; ++i)
                {
                    var td = gridCellDetailAuthoring.gridCellDetails[i];
                    gridCellDataArray[i] = new GridCellDetail {Kind = td.Kind, Cost = td.Cost};
                }
    
                gridCellDataBlobAssetReference = blobBuilder.CreateBlobAssetReference<GridCellDetailBlobAsset>(Allocator.Persistent);
            }
    
            var environmentQuery = DstEntityManager.CreateEntityQuery(typeof(TheEnvironment));
            var environmentEntity = environmentQuery.GetSingletonEntity();
            
            DstEntityManager.AddComponentData(environmentEntity, new ZoneGridCellDetailLookup
            {
                GridCellDetailBlobAssetRef = gridCellDataBlobAssetReference
            });
        }
    }    
}
