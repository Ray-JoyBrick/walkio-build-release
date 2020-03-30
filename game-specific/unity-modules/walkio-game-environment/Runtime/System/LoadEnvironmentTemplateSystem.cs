namespace JoyBrick.Walkio.Game.Environment
{
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
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
