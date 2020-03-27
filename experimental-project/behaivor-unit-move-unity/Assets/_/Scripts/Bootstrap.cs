namespace JoyBrick.Walkio.Game
{
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public class Bootstrap : MonoBehaviour
    {
        private void Start()
        {
            // Setup ECS systems here
            Utility.Bootstrap.AddInitializationSystem<AssetLoadingSystem>();
            
            Utility.Bootstrap.AddInitializationSystem<SpawnTeamUnitSystem>();
            Utility.Bootstrap.AddInitializationSystem<SpawnUnitSystem>();

            // Better to chop the system into several parts
            Utility.Bootstrap.AddInitializationSystem<EnvironmentSystem>();
            Utility.Bootstrap.AddInitializationSystem<HudSystem>();
            
            Utility.Bootstrap.AddSimulationSystem<PlayerInputSystem>();
            Utility.Bootstrap.AddSimulationSystem<DecideTargetSystem>();
            Utility.Bootstrap.AddSimulationSystem<AssignNewTargetToFreeUnitSystem>();
            Utility.Bootstrap.AddSimulationSystem<NonTeamUnitMoveSystem>();
            
            Utility.Bootstrap.AddPresentationSystem<PresentGridWorldSystem>();
            Utility.Bootstrap.AddPresentationSystem<PlayerMoveRangeSystem>();
        }

        private void OnDestroy()
        {
        }
    }
}
