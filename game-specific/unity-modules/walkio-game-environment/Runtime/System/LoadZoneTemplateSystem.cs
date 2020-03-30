namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameTemplate = JoyBrick.Walkio.Game.Template;
    
    // Meant to be in used in Main thread
    [DisableAutoCreation]
    public class LoadZoneTemplateSystem :
        SystemBase
    {
        //
        public GameCommon.IWorldLoadingRequester WorldLoadingRequester { get; set; }
        
        //
        private EntityQuery _eventQuery;
        private EntityArchetype _generateEventArchetype;

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public void Setup()
        {
            WorldLoadingRequester?.LoadingWorld
                .Subscribe(x =>
                {
                    //
                    Debug.Log($"LoadZoneSystem - LoadingWorld received");          

                    var index = x;

                    Load(index).ToObservable()
                        .ObserveOnMainThread()
                        .SubscribeOnMainThread()
                        .Subscribe(result =>
                        {
                            var (scriptableObject, sceneInstance) = result;
                            // var castedSO = scriptableObject as Bridge.TileDetailAsset;
                            var castedSO = scriptableObject as GameTemplate.ZoneData;
                            var scene = sceneInstance.Scene;
                            
                            WorldLoadingRequester.SetZoneScene(scene);
                            WorldLoadingRequester.SetupPathfindingData(castedSO.pathfindingData);

                            var prefab = castedSO.prefab;

                            var (width, height) = AssignDataFromTexture(prefab, castedSO.gridImage);

                            // Need to instantiate the prefab to make it converted
                            GameObject.Instantiate(prefab);

                            //
                            var entity = EntityManager.CreateEntity(_generateEventArchetype);
                            EntityManager.SetName(entity, "Generate Zone Event");
                            EntityManager.SetComponentData(entity, new GenerateZoneProperty
                            {
                                Width = width,
                                Height = height
                            });

                        })
                        .AddTo(_compositeDisposable);                    
                })
                .AddTo(_compositeDisposable);
        }
        
        private async Task<(ScriptableObject, SceneInstance)> Load(int index)
        {
            var zoneDataAddress = $"Zone Data - {index:00}";
            var zoneSceneAddress = $"Zone Scene - {index:00}";
            // TODO: Hide the use of Addressable into another system
            var handle1 = Addressables.LoadAssetAsync<ScriptableObject>(zoneDataAddress);
            // var handle2 = Addressables.LoadSceneAsync(zoneSceneAddress, LoadSceneMode.Additive, false, 100);
            var handle2 = Addressables.LoadSceneAsync(zoneSceneAddress, LoadSceneMode.Additive);

            var t1 = await handle1.Task;
            var t2 = await handle2.Task;

            return (t1, t2);
        }
        
        // TODO: Think about how the texture should be formatted to represent the obstacles
        private (int, int) AssignDataFromTexture(GameObject prefab, Texture2D texture2D)
        {
            var width = texture2D.width;
            var height = texture2D.height;
            var array = texture2D.GetRawTextureData<Color32>();
            // var array = texture2D.GetRawTextureData<byte>();
            Debug.Log($"texture length: {array.Length} format: {texture2D.format} width: {width} height: {height}");

            var blobAssetAuthoring = prefab.GetComponent<GridCellIndexBlobAssetAuthoring>();
            
            var indices = new List<int>();
            for (var i = 0; i < array.Length; ++i)
            {
                var color = array[i];
                // Debug.Log($"color: {array[i]}");
                var index = Utility.WorldMapHelper.GetTileTypeIndex(color.r, color.g, color.b);
                indices.Add(index);
            }

            blobAssetAuthoring.indices = indices;

            return (width, height);
        }
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            //
            _eventQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<GameCommon.LoadZoneRequest>() 
                }
            });
            
            _generateEventArchetype = EntityManager.CreateArchetype(
                typeof(GenerateZone),
                typeof(GenerateZoneProperty));
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
}
