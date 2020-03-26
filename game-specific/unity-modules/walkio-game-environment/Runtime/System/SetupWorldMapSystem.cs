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
        private EntityArchetype _generateEventArchetype;

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
            
             _generateEventArchetype = EntityManager.CreateArchetype(
                 typeof(GenerateWorldMap),
                 typeof(GenerateWorldMapProperty));

            RequireForUpdate(_eventQuery);
        }

        protected override void OnUpdate()
        {
            // Use SingletonEntity query to guard it here
            var entity = _eventQuery.GetSingletonEntity();
            var loadWorldMapRequest = EntityManager.GetComponentData<Common.LoadWorldMapRequest>(entity);

            // Debug.Log($"SetupWorldMapSystem - OnUpdate");

            if (_loading) return;

            if (loadWorldMapRequest.WorldMapIndex == 0)
            {
                _loading = true;
                
                // Can actually combined using UniRx but will require it to be used in the project
                _assetLoadingService.LoadAsset<ScriptableObject>("Environment Settings 1", (scriptableObject) =>
                {
                    _assetLoadingService.LoadAsset<GameObject>("Tile Authoring 1", (prefab) =>
                    {
                        _assetLoadingService.LoadAsset<GameObject>("Tile Detail Index Authoring Prefab",
                            (indexPrefab) =>
                            {
                                _assetLoadingService.LoadAsset<Texture2D>("World Map Image", (image) =>
                                {
                                    var tdba = prefab.GetComponent<TileDetailBlobAssetAuthoring>();
                                    var castedSO = scriptableObject as Bridge.TileDetailAsset;
                                    tdba.tileDetailAsset = castedSO;
                                    // tdba.tileDataAsset = tileDataAsset;
                            
                                    GameObject.Instantiate(prefab);

                                    var (width, height) = AssignDataFromTexture(indexPrefab, image);

                                    GameObject.Instantiate(indexPrefab);
                    
                                    _loading = false;
                    
                                    var eventEntity = EntityManager.CreateEntity(_generateEventArchetype);
                                    EntityManager.SetComponentData(eventEntity, new GenerateWorldMapProperty
                                    {
                                        Width = width,
                                        Height = height
                                    });
                                    
                                    EntityManager.DestroyEntity(entity);
                                });
                            });
                    });
                });
            }
        }
        
        private (int, int) AssignDataFromTexture(GameObject prefab, Texture2D texture2D)
        {
            var width = texture2D.width;
            var height = texture2D.height;
            var array = texture2D.GetRawTextureData<Color32>();
            // var array = texture2D.GetRawTextureData<byte>();
            Debug.Log($"texture length: {array.Length} format: {texture2D.format} width: {width} height: {height}");

            var tdiba = prefab.GetComponent<TileDetailIndexBlobAssetAuthoring>();

            var indices = new List<int>();
            for (var i = 0; i < array.Length; ++i)
            {
                var color = array[i];
                // Debug.Log($"color: {array[i]}");
                var index = Utility.WorldMapHelper.GetTileTypeIndex(color.r, color.g, color.b);
                indices.Add(index);
            }

            tdiba.indices = indices;

            return (width, height);
        }
    }
}