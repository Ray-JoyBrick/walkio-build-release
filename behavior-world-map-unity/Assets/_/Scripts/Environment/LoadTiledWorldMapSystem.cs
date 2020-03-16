namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    [DisableAutoCreation]
    public class LoadTiledWorldMapSystem : SystemBase
    {
        private bool _loading = false;
        private bool _loaded = false;

        private float _startLoadingTime = 0;

        private AsyncOperationHandle<GameObject> _handle = default;
        
        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _eventQuery;
        private EntityArchetype _generateEventArchetype;

        protected override void OnCreate()
        {
            base.OnCreate();

            //
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            //
            _eventQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(LoadWorldMapRequest) }
            });

            _generateEventArchetype = EntityManager.CreateArchetype(
                typeof(GenerateWorldMap));
            
            //
            RequireForUpdate(_eventQuery);
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var entity = _eventQuery.GetSingletonEntity();
            var loadWorldMapRequest = EntityManager.GetComponentData<LoadWorldMapRequest>(entity);
            
            // Debug.Log("LoadTiledWorldMapSystem - OnUpdate");

            if (!_loading)
            {
                var worldMapAddressableName = "World Map";
                _handle = Addressables.LoadAssetAsync<GameObject>(worldMapAddressableName);
                _loading = true;
                _startLoadingTime = UnityEngine.Time.time;
            }

            if (_loading && _handle.IsDone)
            {
                var loadingTime = UnityEngine.Time.time - _startLoadingTime;
                Debug.Log($"LoadTiledWorldMapSystem - finish loading, took: {loadingTime} seconds");
                _loading = false;
                _loaded = true;

                //
                var prefab = _handle.Result;
                AssignDataFromGrid(prefab);

                //
                var mapEntity = commandBuffer.CreateEntity(_generateEventArchetype);
                
                commandBuffer.DestroyEntity(entity);
            }
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        private void AssignDataFromGrid(GameObject prefab)
        {
            var grid = prefab.GetComponent<Grid>();

            Debug.Log($"LoadTiledWorldMapSystem - grid: {grid}");

            foreach (Transform child in grid.transform)
            {
                Debug.Log($"LoadTiledWorldMapSystem - child: {child.name} under grid");
            }
        }
    }
}