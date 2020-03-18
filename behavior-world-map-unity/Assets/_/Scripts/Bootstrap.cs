namespace JoyBrick.Walkio.Game
{
    using System.Collections;
    using Environment;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.AddressableAssets.ResourceLocators;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public class Bootstrap : MonoBehaviour
    {
        public bool turnOnDiagnostic;

        public GameObject tileAuthoringPrefab01;
        public GameObject tileAuthoringPrefab02;

        public Environment.Bridge.TileDataAsset tileDataAsset;
        
        private Entity _entity;
        
        void Start()
        {
            var archetype = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(
                typeof(Environment.Environment));

            _entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(archetype);
            // World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(_entity, tileDataAsset);
            
            Addressables.InitializeAsync().Completed += OnAddressableInitializeAsyncCompleted;
        }

        private void OnAddressableInitializeAsyncCompleted(AsyncOperationHandle<IResourceLocator> handle)
        {
            Debug.Log($"Bootstrap - OnAddressableInitializeAsyncCompleted");

            // StartCoroutine(Spawn01());
            // StartCoroutine(Spawn02());

            StartCoroutine(SpawnPrefab(tileAuthoringPrefab01, 1.0f));
            StartCoroutine(SpawnPrefab(tileAuthoringPrefab02, 4.0f));
            StartCoroutine(SpawnPrefab(tileAuthoringPrefab01, 6.0f));
            StartCoroutine(SpawnPrefab(tileAuthoringPrefab02, 9.0f));

            // StartCoroutine(RemoveEntity01());
            // StartCoroutine(RemoveEntity02());

            // World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(_entity, tileDataAsset);

            // GameObject.Instantiate(tileAuthoringPrefab);

            //
            Utility.Bootstrap.AddInitializationSystem<Environment.RemoveConvertedSystem>();
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

        IEnumerator SpawnPrefab(GameObject prefab, float afterTime)
        {
            yield return new WaitForSeconds(afterTime);

            GameObject.Instantiate(prefab);
        }

        IEnumerator Spawn01()
        {
            yield return new WaitForSeconds(3.0f);

            GameObject.Instantiate(tileAuthoringPrefab01);
        }

        IEnumerator Spawn02()
        {
            yield return new WaitForSeconds(30.0f);

            GameObject.Instantiate(tileAuthoringPrefab02);
        }

        // IEnumerator RemoveEntity01()
        // {
        //     yield return new WaitForSeconds(10.0f);
        //     var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //     var query = entityManager.CreateEntityQuery(typeof(TileDataBlobAssetAuthoring));
        //     if (query == null)
        //     {
        //         Debug.Log($"can not get valid query");
        //     }
        //     else
        //     {
        //         Debug.Log($"going to destroy the entity 1");
        //         entityManager.RemoveComponent<TileDataAssetEx>(query);
        //         entityManager.DestroyEntity(query);
        //     }
        // }
        //
        // IEnumerator RemoveEntity02()
        // {
        //     yield return new WaitForSeconds(35.0f);
        //     var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //     var query = entityManager.CreateEntityQuery(typeof(TileDataBlobAssetAuthoring));
        //     if (query == null)
        //     {
        //         Debug.Log($"can not get valid query");
        //     }
        //     else
        //     {
        //         Debug.Log($"going to destroy the entity 2");
        //         entityManager.DestroyEntity(query);
        //     }
        // }
    }
}
