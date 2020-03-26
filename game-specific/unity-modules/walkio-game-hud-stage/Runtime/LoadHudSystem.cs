namespace JoyBrick.Walkio.Game.Hud.Stage
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;

    [DisableAutoCreation]
    public class LoadHudSystem : SystemBase
    {
        //
        private Common.IAssetLoadingService _assetLoadingService;
        
        //
        private EntityQuery _eventQuery;
        
        //
        private bool _loading;
        
        //
        public void SetAssetLoadingService(Common.IAssetLoadingService assetLoadingService)
        {
            _assetLoadingService = assetLoadingService;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _eventQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<LoadHudStage>() 
                }
            });
            
            RequireForUpdate(_eventQuery);
        }        
        
        protected override void OnUpdate()
        {
            // Use SingletonEntity query to guard it here
            var entity = _eventQuery.GetSingletonEntity();
            var loadWorldMapRequest = EntityManager.GetComponentData<LoadHudStage>(entity);

            if (_loading) return;

            _loading = true;
            _assetLoadingService.LoadAsset<GameObject>("Hud - Stage - Graph Controller", (graphControllerPrefab) =>
            {
                // Debug.Log($"LoadHudSystem - Callback is called");
                _assetLoadingService.LoadAsset<GameObject>("Hud - Stage - Canvas", (canvasPrefab) =>
                {
                    GameObject.Instantiate(graphControllerPrefab);
                    GameObject.Instantiate(canvasPrefab);
                    
                    _loading = false;
                
                    EntityManager.DestroyEntity(entity);
                });
            });
        }
    }
}