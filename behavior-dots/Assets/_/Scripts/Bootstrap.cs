namespace JoyBrick.Walkio.Game.Main
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;

    public class CustomBootstrap : ICustomBootstrap
    {
        public bool Initialize(string defaultWorldName)
        {
            Debug.Log($"CustomBootstrap - Initialize: {defaultWorldName}");
            var world = new World(defaultWorldName);
            World.DefaultGameObjectInjectionWorld = world;
            var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
 
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systems);
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);

            return true;
        }
    }

    public class Bootstrap :
        MonoBehaviour//,
        // ICustomBootstrap
    {
        [Header("Players")]
        public GameObject PlayerCharacterPrefab;
    
        // Two transforms to make a rectangle and spawn pickups inside it
        public UnityEngine.BoxCollider PickUpsBounds;
        public float HealthPickUpsNumber;
        public GameObject[] PickupPrefabs;
        
        public Camera Camera;

        //
        private EntityManager entityManager;
        private Entity characterPrefabEntity;
        private List<Entity> prefabEntityPickUps = new List<Entity>();
    
        private EntityQuery playersQuery;

        void Start()
        {
            // Initialize("Client");
            
            Debug.Log($"Bootstrap - Start");

            var customBootstrap = new CustomBootstrap();
            customBootstrap.Initialize("Client");
            
            var clientWorlds =
                World.AllWorlds
                    .Where(w => string.Compare(w.Name, "Client", StringComparison.Ordinal) == 0)
                    .ToList();
            if (clientWorlds.Any())
            {
                var clientWorld = clientWorlds.First();
                
                clientWorld.GetOrCreateSystem<CharacterMoveSystem>().CameraTransform = Camera.transform;
                
                var settings = GameObjectConversionSettings.FromWorld(clientWorld,  new BlobAssetStore());
                characterPrefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(PlayerCharacterPrefab, settings);
                
                entityManager = clientWorld.EntityManager;
                
                EntityQueryDesc queryDesc = new EntityQueryDesc
                {
                    All = new ComponentType[] { ComponentType.ReadOnly<Character>(), ComponentType.ReadOnly<PlayerCharacter>() }
                };
                playersQuery = clientWorld.EntityManager.CreateEntityQuery(queryDesc);
                
                foreach(GameObject prefab in PickupPrefabs)
                {
                    prefabEntityPickUps.Add(GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings));
                }
                
                List<int> deviceIds = new List<int>();
                deviceIds.Add(0);
                
                var playerInputSystem = clientWorld.GetOrCreateSystem<PlayerInputSystem>();
                var pickupSystem = clientWorld.GetOrCreateSystem<PickupSystem>();
                var characterMoveSystem = clientWorld.GetOrCreateSystem<CharacterMoveSystem>();
                
                clientWorld.GetExistingSystem<SimulationSystemGroup>().AddSystemToUpdateList(playerInputSystem);
                clientWorld.GetExistingSystem<SimulationSystemGroup>().AddSystemToUpdateList(pickupSystem);
                clientWorld.GetExistingSystem<SimulationSystemGroup>().AddSystemToUpdateList(characterMoveSystem);
                
                Entity newPlayeEntity = playerInputSystem.CreatePlayer(deviceIds);
                // SpawnCharacterForPlayer(entityManager, characterPrefabEntity, startingGunPrefabEntity, playerStartingMeleePrefabEntity, PlayerSpawnPoints[playersSpawned].position, Quaternion.identity, newPlayeEntity);
                SpawnCharacterForPlayer(entityManager, characterPrefabEntity, new Vector3(0, 0.0f, 0), Quaternion.identity, newPlayeEntity);
                
                //
                Bounds bounds = PickUpsBounds.bounds;
                for(int i = 0; i < HealthPickUpsNumber; i++)
                {
                    float x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
                    float z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);
                    Vector3 spawnPointhealthPickUp = new Vector3(x, 0.5f, z);
                    SpawnHealthPickUp(entityManager, prefabEntityPickUps[0], spawnPointhealthPickUp);
                } 
            }
        }
        
        public void SpawnCharacterForPlayer(
            EntityManager entityManager,
            Entity characterPrefabEntity,
            // Entity startingGunPrefabEntity,
            // Entity startingMeleePrefabEntity,
            Vector3 atPoint,
            Quaternion atRotation,
            Entity owningEntity)
        {
            Entity charInstanceEntity = entityManager.Instantiate(characterPrefabEntity);
            entityManager.SetComponentData(charInstanceEntity, new Translation { Value = atPoint });
            entityManager.SetComponentData(charInstanceEntity, new Rotation { Value = atRotation });
            entityManager.AddComponentData(charInstanceEntity, new OwningPlayer { PlayerEntity = owningEntity });
            // entityManager.AddComponentData(charInstanceEntity, new CameraFocus());
        
            // Character spawnedCharacter = entityManager.GetComponentData<Character>(charInstanceEntity);
            Character charData = entityManager.GetComponentData<Character>(charInstanceEntity);
            entityManager.SetComponentData(charInstanceEntity, charData);
        }
    
        public void SpawnHealthPickUp(
            EntityManager entityManager,
            Entity healthPickupPrefabEntity,
            Vector3 atPoint)
        {
            Entity HealPickUpInstance = entityManager.Instantiate(healthPickupPrefabEntity);
            entityManager.SetComponentData(HealPickUpInstance, new Translation { Value = atPoint });
        }

        // public bool Initialize(string defaultWorldName)
        // {
        //     //throw new System.NotImplementedException();
        //
        //     Debug.Log($"Bootstrap - Initialize: {defaultWorldName}");
        //     var world = new World("Client");
        //     World.DefaultGameObjectInjectionWorld = world;
        //     var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
        //
        //     DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systems);
        //     ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
        //
        //     return true;
        //     
        //     // var clientWorlds =
        //     //     World.AllWorlds
        //     //         .Where(w => string.Compare(w.Name, "Client", StringComparison.Ordinal) == 0)
        //     //         .ToList();
        //     // if (clientWorlds.Any())
        //     // {
        //     //     var clientWorld = clientWorlds.First();
        //     //     var playerInputSystem = clientWorld.GetOrCreateSystem<PlayerInputSystem>();
        //     // }
        //
        //     return true;
        // }
    }
}
