namespace JoyBrick.Walkio.Game
{
    using UnityEngine;

    public class Bootstrap : MonoBehaviour
    {
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
