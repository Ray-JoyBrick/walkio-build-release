namespace JoyBrick.Walkio.Game
{
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.AddressableAssets.ResourceLocators;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public class Bootstrap : MonoBehaviour
    {
        public bool turnOnDiagnostic;
        
        void Start()
        {
            Addressables.InitializeAsync().Completed += OnAddressableInitializeAsyncCompleted;
        }

        private void OnAddressableInitializeAsyncCompleted(AsyncOperationHandle<IResourceLocator> handle)
        {
            Debug.Log($"Bootstrap - OnAddressableInitializeAsyncCompleted");
            
            //
            Utility.Bootstrap.AddInitializationSystem<Environment.LoadTiledWorldMapSystem>();
            Utility.Bootstrap.AddInitializationSystem<Environment.GenerateWorldMapSystem>();
            if (turnOnDiagnostic)
            {
                Utility.Bootstrap.AddInitializationSystem<Environment.GenerateDiagnosticWorldMapSystem>();
            }
            else
            {
                Utility.Bootstrap.AddInitializationSystem<Environment.GenerateDiagnosticWorldMapSystem_Empty>();
            }
            
            //
            Utility.Bootstrap.AddSimulationSystem<Battle.UnitSpawnSystem>();
        }
    }
}
