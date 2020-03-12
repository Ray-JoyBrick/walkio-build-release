namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;
    using UnityEngine;

    [DisableAutoCreation]
    public class EnvironmentSystem : SystemBase
    {
        private IAssetLoadingService _assetLoadingService;
        
        private GameObject _worldMapInstance;
        private GameObject _gridInstance;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            var als = World.GetOrCreateSystem<AssetLoadingSystem>();
            _assetLoadingService = als as IAssetLoadingService;

            _assetLoadingService.LoadAssetAsync<GameObject>("Test Map", (mapPrefab) =>
            {
                //
                _assetLoadingService.LoadAssetAsync<GameObject>("Test Grid", (gridPrefab) =>
                {
                    //
                    _worldMapInstance = GameObject.Instantiate(mapPrefab);
                    _gridInstance = GameObject.Instantiate(gridPrefab);
                });
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
        }
    }    
}
