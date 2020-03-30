// namespace JoyBrick.Walkio.Game.Environment
// {
//     using Unity.Collections;
//     using Unity.Entities;
//     using UnityEngine;
//
//     [UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
//     public class TileDetailBlobAssetConstructor : GameObjectConversionSystem
//     {
//         protected override void OnUpdate()
//         {
//             Debug.Log($"TileDataBlobAssetConstructor - OnUpdate");
//             
//             BlobAssetReference<TileDetailBlobAsset> tileDataBlobAssetReference;
//             
//             // I guess this update only perform at main thread
//             using (var blobBuilder = new BlobBuilder(Allocator.Temp))
//             {
//                 ref var tileDetailBlobAsset = ref blobBuilder.ConstructRoot<TileDetailBlobAsset>();
//                 
//                 // TileDataBlobAssetAuthoring itself is MonBehaviour + IConvertGameObjectToEntity and once being
//                 // converted into entity, that scriptable object which is ScriptableObject + IComponentData
//                 // becoming part of the entity.
//                 // The question is, can this conversion takes place only when ScriptableObject is loaded? 
//                 var tileDetailAuthoring = GetEntityQuery(typeof(TileDetailBlobAssetAuthoring)).ToComponentArray<TileDetailBlobAssetAuthoring>()[0];
//     
//                 // var tileDataArray = blobBuilder.Allocate(ref tileDataBlobAsset.TileDatas, 2);
//                 //
//                 // tileDataArray[0] = new TileData {Type = TileType.Floor, Cost = 0};
//                 // tileDataArray[1] = new TileData {Type = TileType.Wall, Cost = 0};
//     
//                 var count = tileDetailAuthoring.tileDetailAsset.tileDetails.Count;
//                 var tileDataArray = blobBuilder.Allocate(ref tileDetailBlobAsset.TileDetails, count);
//                 for (var i = 0; i < count; ++i)
//                 {
//                     var td = tileDetailAuthoring.tileDetailAsset.tileDetails[i];
//                     tileDataArray[i] = new TileDetail {Type = (TileType) td.kind, Cost = td.cost};
//                 }
//     
//                 tileDataBlobAssetReference = blobBuilder.CreateBlobAssetReference<TileDetailBlobAsset>(Allocator.Persistent);
//             }
//     
//             var environmentQuery = DstEntityManager.CreateEntityQuery(typeof(TheEnvironment));
//             var environmentEntity = environmentQuery.GetSingletonEntity();
//             
//             DstEntityManager.AddComponentData(environmentEntity, new WorldMapTileDetailLookup
//             {
//                 TileDetailBlobAssetRef = tileDataBlobAssetReference
//             });
//         }
//     }    
// }
