namespace JoyBrick.Walkio.Game.Environment
{
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.SceneManagement;
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameTemplate = JoyBrick.Walkio.Game.Template;
    
    // Meant to be in used in Main thread
    [DisableAutoCreation]
    public class LoadEnvironmentTemplateSystem :
        SystemBase
    {
        //
        public GameCommon.IEnvironmentSetupRequester EnvironmentSetupRequester { get; set; }
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        public void Setup()
        {
            EnvironmentSetupRequester?.InitializingEnvironment
                .Subscribe(x =>
                {
                    //
                    Debug.Log($"LoadEnvironmentSettingSystem - InitializingEnvironment received");
                    Load().ToObservable()
                        .ObserveOnMainThread()
                        .SubscribeOnMainThread()
                        .Subscribe(result =>
                        {
                            var scriptableObject = result;

                            var castedSO = scriptableObject as GameTemplate.EnvironmentData;
                            
                            var theEnvironmentQuery = EntityManager.CreateEntityQuery(typeof(TheEnvironment));
                            var theEnvironmentEntity = theEnvironmentQuery.GetSingletonEntity();
                            
                            var gridCellSize = new float2(castedSO.gridCellSize.x, castedSO.gridCellSize.y);
                            var tileCellSize = new float2(castedSO.tileCellSize.x, castedSO.tileCellSize.y);
                            var tileCellCount = new int2((int) castedSO.tileCellCount.x, (int) castedSO.tileCellCount.y);
                            
                            EntityManager.SetComponentData(theEnvironmentEntity, new TheEnvironment
                            {
                                GridCellSize = gridCellSize,
                                TileCellSize = tileCellSize,
                                TileCellCount = tileCellCount
                            });

                            var blobAssetAuthoring = castedSO.prefab.GetComponent<GridCellDetailBlobAssetAuthoring>();
                            blobAssetAuthoring.gridCellDetails = castedSO.gridCellDetails;

                            GameObject.Instantiate(castedSO.prefab);
                            
                            EnvironmentSetupRequester.SetEnvironmentData(scriptableObject);
                        })
                        .AddTo(_compositeDisposable);
                })
                .AddTo(_compositeDisposable);
        }
        
        private async Task<ScriptableObject> Load()
        {
            var environmentDataAddress = $"Environment Data";
            var handle1 = Addressables.LoadAssetAsync<ScriptableObject>(environmentDataAddress);

            var t1 = await handle1.Task;

            return t1;
        }
        
        protected override void OnCreate()
        {
            base.OnCreate();
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
