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
            // var gridPrefabTask = _assetLoadingSystem.LoadFromAddressable<GameObject>("Test");

            // Only wait a bit here
            // gridPrefabTask.Wait(System.TimeSpan.FromMilliseconds(1000));
            // Just get result directly
            // gridPrefabTask.RunSynchronously(TaskScheduler.Current);
            // var gridPrefab = gridPrefabTask.Result;
            // var gridInstance = GameObject.Instantiate(gridPrefab);

            Addressables.LoadAssetAsync<GameObject>("Test").Completed += OnCompleted;

            
            // // Setup ECS systems here
            // Utility.Bootstrap.AddInitializationSystem<SpawnTeamUnitSystem>();
            // Utility.Bootstrap.AddInitializationSystem<SpawnUnitSystem>();
            //
            // Utility.Bootstrap.AddSimulationSystem<PlayerInputSystem>();
            // Utility.Bootstrap.AddSimulationSystem<DecideTargetSystem>();
            // Utility.Bootstrap.AddSimulationSystem<AssignNewTargetToFreeUnitSystem>();
            // Utility.Bootstrap.AddSimulationSystem<NonTeamUnitMoveSystem>();
            //
            // Utility.Bootstrap.AddPresentationSystem<PlayerMoveRangeSystem>();
        }

        private void OnCompleted(AsyncOperationHandle<GameObject> handle)
        {
            var prefab = handle.Result;

            var gridInstance = GameObject.Instantiate(prefab);

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
