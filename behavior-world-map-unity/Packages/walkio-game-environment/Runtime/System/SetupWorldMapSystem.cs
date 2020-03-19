namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;
    
    using Common = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class SetupWorldMapSystem :
        SystemBase
    {
        //
        private Common.IAssetLoadingService _assetLoadingService;
        
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
                    ComponentType.ReadOnly<Common.LoadWorldMapRequest>() 
                }
            });
            
            RequireForUpdate(_eventQuery);
        }

        protected override void OnUpdate()
        {
            // Use SingletonEntity query to guard it here
            var entity = _eventQuery.GetSingletonEntity();
            var loadWorldMapRequest = EntityManager.GetComponentData<Common.LoadWorldMapRequest>(entity);

            Debug.Log($"SetupWorldMapSystem - OnUpdate");

            if (_loading) return;

            if (loadWorldMapRequest.WorldMapIndex == 0)
            {
                _loading = true;
                
                _assetLoadingService.LoadAsset<Bridge.TileDataAsset>("Environment Settings 1", (tileDataAsset) =>
                {
                    _assetLoadingService.LoadAsset<GameObject>("Tile Authoring 1", (prefab) =>
                    {
                        _assetLoadingService.LoadAsset<Texture2D>("World Map Image", (image) =>
                        {
                            var tdba = prefab.GetComponent<TileDataBlobAssetAuthoring>();
                            // var castedSO = scriptableObject as Bridge.TileDataAsset;
                            //
                            // if (castedSO == null)
                            // {
                            //     Debug.LogWarning($"Should get no null");
                            // }
                            // else
                            // {
                            //     Debug.Log(castedSO);
                            // }
                            //
                            // tdba.tileDataAsset = castedSO;
                            tdba.tileDataAsset = tileDataAsset;
                            
                            GameObject.Instantiate(prefab);

                            AssignDataFromTexture(image);
                    
                            _loading = false;
                    
                            EntityManager.DestroyEntity(entity);
                        });
                    });
                });
            }
        }
        
        private void AssignDataFromTexture(Texture2D texture2D)
        {
            var width = texture2D.width;
            var height = texture2D.height;
            var array = texture2D.GetRawTextureData<Color32>();
            // var array = texture2D.GetRawTextureData<byte>();
            Debug.Log($"texture length: {array.Length} format: {texture2D.format} width: {width} height: {height}");
            for (var i = 0; i < array.Length; ++i)
            {
                // Debug.Log($"color: {array[i]}");
                
                // Utility.WorldMapHelper.
            }
        }
    }
}