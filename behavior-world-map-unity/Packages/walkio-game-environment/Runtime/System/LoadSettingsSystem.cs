namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    // // [DisableAutoCreation]
    // [UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
    // public class LoadSettingsSystem : GameObjectConversionSystem
    // {
    //     private bool _loading = false;
    //     private bool _loaded = false;
    //
    //     private float _startLoadingTime = 0;
    //     
    //     //
    //     private AsyncOperationHandle<ScriptableObject> _handle = default;
    //     
    //     //
    //     // private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
    //
    //     protected override void OnCreate()
    //     {
    //         Debug.Log($"LoadSettingsSystem - OnCreate");
    //         base.OnCreate();
    //         
    //         //
    //         // _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    //
    //     }
    //
    //     protected override void OnUpdate()
    //     {
    //         Debug.Log($"LoadSettingsSystem - OnUpdate");
    //         // var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
    //         if (!_loading && !_loaded)
    //         {
    //             var addressableName = "Environment Settings";
    //             _handle = Addressables.LoadAssetAsync<ScriptableObject>(addressableName);
    //             _loading = true;
    //             _startLoadingTime = UnityEngine.Time.time;
    //         }
    //         
    //         if (_loading && _handle.IsDone)
    //         {
    //             var loadingTime = UnityEngine.Time.time - _startLoadingTime;
    //             Debug.Log($"LoadSettingsSystem - finish loading, took: {loadingTime} seconds");
    //             _loading = false;
    //             _loaded = true;
    //
    //             //
    //             var settings = _handle.Result;
    //             AssignDataFromSettings(settings);
    //
    //             // //
    //             // var mapEntity = commandBuffer.CreateEntity(_generateEventArchetype);
    //             //
    //             // commandBuffer.DestroyEntity(entity);
    //         }
    //         
    //         // _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);            
    //     }
    //
    //     private void AssignDataFromSettings(ScriptableObject settings)
    //     {
    //         Debug.Log($"LoadSettingsSystem - AssignDataFromSettings - settings: {settings}");
    //         BlobAssetReference<TileDataBlobAsset> tileDataBlobAssetReference;
    //         
    //         using (var blobBuilder = new BlobBuilder(Allocator.Temp))
    //         {
    //             ref var tileDataBlobAsset = ref blobBuilder.ConstructRoot<TileDataBlobAsset>();
    //             var tileDataArray = blobBuilder.Allocate(ref tileDataBlobAsset.TileDatas, 3);
    //
    //             tileDataArray[0] = new TileData {Type = TileType.Floor, Cost = 0};
    //             tileDataArray[1] = new TileData {Type = TileType.Wall, Cost = 0};
    //
    //             tileDataBlobAssetReference = blobBuilder.CreateBlobAssetReference<TileDataBlobAsset>(Allocator.Persistent);
    //         }
    //
    //         var environmentQuery = DstEntityManager.CreateEntityQuery(typeof(Environment));
    //         // var environmentEntity = environmentQuery.GetSingletonEntity();
    //         //
    //         // DstEntityManager.AddComponentData(environmentEntity, new WorldMapTileLookup
    //         // {
    //         //     TileDataBlobAssetRef = tileDataBlobAssetReference
    //         // });
    //         
    //     }
    // }
    
    // //TODO: Delete after merge
    // [UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
    // public class TileDataBlobAssetConstructor : GameObjectConversionSystem
    // {
    //     public BlobAssetReference<TileDataBlobAsset> tileDataBlobAssetReference;
    //
    //     protected override void OnUpdate()
    //     {
    //         Debug.Log($"TileDataBlobAssetConstructor - OnUpdate");
    //         
    //         // BlobAssetReference<TileDataBlobAsset> tileDataBlobAssetReference;
    //         
    //         // I guess this update only perform at main thread
    //         using (var blobBuilder = new BlobBuilder(Allocator.Temp))
    //         {
    //             ref var tileDataBlobAsset = ref blobBuilder.ConstructRoot<TileDataBlobAsset>();
    //             
    //             // var tileDataAuthoring = GetEntityQuery(typeof(TileDataBlobAssetAuthoring)).ToComponentArray<TileDataBlobAssetAuthoring>()[0];
    //             // var tileDataAuthoring = GetEntityQuery(typeof(TileDataPlaceholder)).ToComponentArray<TileDataPlaceholder>()[0];
    //             var query = GetEntityQuery(typeof(TileDataPlaceholder));
    //             var btda = query.ToComponentDataArray<Bridge.TileDataAsset>()[0];
    //
    //             var count = btda.tileDatas.Count;
    //             var tileDataArray = blobBuilder.Allocate(ref tileDataBlobAsset.TileDatas, count);
    //             for (var i = 0; i < count; ++i)
    //             {
    //                 var td = btda.tileDatas[i];
    //                 tileDataArray[i] = new TileData {Type = (TileType) td.kind, Cost = td.cost};
    //             }
    //
    //             tileDataBlobAssetReference = blobBuilder.CreateBlobAssetReference<TileDataBlobAsset>(Allocator.Persistent);
    //         }
    //
    //         var environmentQuery = DstEntityManager.CreateEntityQuery(typeof(Environment));
    //         var environmentEntity = environmentQuery.GetSingletonEntity();
    //         
    //         DstEntityManager.AddComponentData(environmentEntity, new WorldMapTileLookup
    //         {
    //             TileDataBlobAssetRef = tileDataBlobAssetReference
    //         });
    //     }
    // }
    
    //TODO: Delete after merge
    [UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
    public class TileDataBlobAssetConstructor : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Debug.Log($"TileDataBlobAssetConstructor - OnUpdate");
            
            BlobAssetReference<TileDataBlobAsset> tileDataBlobAssetReference;
            
            // I guess this update only perform at main thread
            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var tileDataBlobAsset = ref blobBuilder.ConstructRoot<TileDataBlobAsset>();
                
                // TileDataBlobAssetAuthoring itself is MonBehaviour + IConvertGameObjectToEntity and once being
                // converted into entity, that scriptable object which is ScriptableObject + IComponentData
                // becoming part of the entity.
                // The question is, can this conversion takes place only when ScriptableObject is loaded? 
                var tileDataAuthoring = GetEntityQuery(typeof(TileDataBlobAssetAuthoring)).ToComponentArray<TileDataBlobAssetAuthoring>()[0];
    
                // var tileDataArray = blobBuilder.Allocate(ref tileDataBlobAsset.TileDatas, 2);
                //
                // tileDataArray[0] = new TileData {Type = TileType.Floor, Cost = 0};
                // tileDataArray[1] = new TileData {Type = TileType.Wall, Cost = 0};
    
                var count = tileDataAuthoring.tileDataAsset.tileDatas.Count;
                var tileDataArray = blobBuilder.Allocate(ref tileDataBlobAsset.TileDatas, count);
                for (var i = 0; i < count; ++i)
                {
                    var td = tileDataAuthoring.tileDataAsset.tileDatas[i];
                    tileDataArray[i] = new TileData {Type = (TileType) td.kind, Cost = td.cost};
                }
    
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


    // //TODO: Delete after merge
    // [UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
    // public class TileDataBlobAssetConstructor : GameObjectConversionSystem
    // {
    //     protected override void OnUpdate()
    //     {
    //         Debug.Log($"TileDataBlobAssetConstructor - OnUpdate");
    //         
    //         BlobAssetReference<TileDataBlobAsset> tileDataBlobAssetReference;
    //         
    //         // I guess this update only perform at main thread
    //         using (var blobBuilder = new BlobBuilder(Allocator.Temp))
    //         {
    //             ref var tileDataBlobAsset = ref blobBuilder.ConstructRoot<TileDataBlobAsset>();
    //             
    //             // TileDataBlobAssetAuthoring itself is MonBehaviour + IConvertGameObjectToEntity and once being
    //             // converted into entity, that scriptable object which is ScriptableObject + IComponentData
    //             // becoming part of the entity.
    //             // The question is, can this conversion takes place only when ScriptableObject is loaded? 
    //             var tileDataAuthoring = GetEntityQuery(typeof(TileDataBlobAssetAuthoring)).ToComponentArray<TileDataBlobAssetAuthoring>()[0];
    //
    //             // var tileDataArray = blobBuilder.Allocate(ref tileDataBlobAsset.TileDatas, 2);
    //             //
    //             // tileDataArray[0] = new TileData {Type = TileType.Floor, Cost = 0};
    //             // tileDataArray[1] = new TileData {Type = TileType.Wall, Cost = 0};
    //
    //             var count = tileDataAuthoring.tileDataAsset.tileDatas.Count;
    //             var tileDataArray = blobBuilder.Allocate(ref tileDataBlobAsset.TileDatas, count);
    //             for (var i = 0; i < count; ++i)
    //             {
    //                 var td = tileDataAuthoring.tileDataAsset.tileDatas[i];
    //                 tileDataArray[i] = new TileData {Type = (TileType) td.kind, Cost = td.cost};
    //             }
    //
    //             tileDataBlobAssetReference = blobBuilder.CreateBlobAssetReference<TileDataBlobAsset>(Allocator.Persistent);
    //         }
    //
    //         var environmentQuery = DstEntityManager.CreateEntityQuery(typeof(Environment));
    //         var environmentEntity = environmentQuery.GetSingletonEntity();
    //         
    //         DstEntityManager.AddComponentData(environmentEntity, new WorldMapTileLookup
    //         {
    //             TileDataBlobAssetRef = tileDataBlobAssetReference
    //         });
    //     }
    // }    
}