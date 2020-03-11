namespace JoyBrick.Walkio.Game
{
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public class Bootstrap : MonoBehaviour
    {
        // public AssetReferenceT<GameObject> gridPrefab;
        
        private AssetLoadingSystem _assetLoadingSystem = default;
        
        private void Start()
        {
            _assetLoadingSystem = new AssetLoadingSystem();
            _assetLoadingSystem.InitializationFinished += AssetLoadingSystemOnInitializationFinished;
            _assetLoadingSystem.Initialize();
        }

        private void AssetLoadingSystemOnInitializationFinished()
        {
            // Start the ECS world after initialization of normal world is finished.
            // Only take addressable system into account for now, expand later.
            
            Debug.Log($"Normal world systems are initialized");
            
            //
            var h1 = Addressables.LoadAssetAsync<GameObject>("Test Grid");
            var h2 = Addressables.LoadAssetAsync<GameObject>("Test Map");
            // //
            // var allHandles = Task.WhenAll(h1.Task, h2.Task);

            h1.Completed += handle1 =>
            {
                h2.Completed += handle2 =>
                {
                    //
                    OnCompleted(handle1.Result, handle2.Result);
                };
            };
        }

        private void OnCompleted(GameObject gridPrefab, GameObject mapPrefab)
        {
            var gridInstance = GameObject.Instantiate(gridPrefab);
            var mapInstance = GameObject.Instantiate(mapPrefab);

            // Setup ECS systems here
            Utility.Bootstrap.AddInitializationSystem<SpawnTeamUnitSystem>();
            Utility.Bootstrap.AddInitializationSystem<SpawnUnitSystem>();
            
            Utility.Bootstrap.AddSimulationSystem<PlayerInputSystem>();
            Utility.Bootstrap.AddSimulationSystem<DecideTargetSystem>();
            Utility.Bootstrap.AddSimulationSystem<AssignNewTargetToFreeUnitSystem>();
            Utility.Bootstrap.AddSimulationSystem<NonTeamUnitMoveSystem>();
            
            Utility.Bootstrap.AddPresentationSystem<PlayerMoveRangeSystem>();
        }

        private void OnDestroy()
        {
            _assetLoadingSystem?.Dispose();
        }
    }
}
