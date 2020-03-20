namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    [DisableAutoCreation]
    public class GenerateVisualWorldMapSystem : SystemBase
    {
        private bool _loading = false;
        private bool _loaded = false;

        private float _startLoadingTime = 0;

        private AsyncOperationHandle<GameObject> _handle = default;

        // private GameObject _visualWorldMap;
        
        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        protected EntityQuery _eventQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            //
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _eventQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<GenerateVisualWorldMap>() 
                }
            });

            RequireForUpdate(_eventQuery);
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            
            //
            var entity = _eventQuery.GetSingletonEntity();
            var generateVisualWorldMap = EntityManager.GetComponentData<GenerateVisualWorldMap>(entity);

            //
            
            if (!_loading)
            {
                var worldMapAddressableName = "Visual World Map";
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

                // _visualWorldMap =
                GameObject.Instantiate(prefab);

                commandBuffer.DestroyEntity(entity);
            }            
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}