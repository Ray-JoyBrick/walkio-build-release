namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.AddressableAssets.ResourceLocators;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public class Bootstrap : MonoBehaviour
    {
        public bool turnOnDiagnostic;

        public GameObject tileAuthoringPrefab;
        
        void Start()
        {
            var archetype = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(
                typeof(Environment.Environment));

            World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(archetype);
            
            Addressables.InitializeAsync().Completed += OnAddressableInitializeAsyncCompleted;
        }

        private void OnAddressableInitializeAsyncCompleted(AsyncOperationHandle<IResourceLocator> handle)
        {
            Debug.Log($"Bootstrap - OnAddressableInitializeAsyncCompleted");


            GameObject.Instantiate(tileAuthoringPrefab);

            //
            // Utility.Bootstrap.AddGameObjectBeforeConversionSystem<Environment.LoadSettingsSystem>();
            // Utility.Bootstrap.AddInitializationSystem<Environment.LoadTiledWorldMapSystem>();
            // Utility.Bootstrap.AddInitializationSystem<Environment.GenerateWorldMapSystem>();
            // if (turnOnDiagnostic)
            // {
            //     Utility.Bootstrap.AddInitializationSystem<Environment.GenerateDiagnosticWorldMapSystem>();
            // }
            // else
            // {
            //     Utility.Bootstrap.AddInitializationSystem<Environment.GenerateDiagnosticWorldMapSystem_Empty>();
            // }
            //
            // //
            // Utility.Bootstrap.AddSimulationSystem<Battle.UnitSpawnSystem>();
        }
    }
}
