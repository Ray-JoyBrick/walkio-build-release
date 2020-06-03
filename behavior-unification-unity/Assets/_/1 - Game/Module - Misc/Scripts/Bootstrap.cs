namespace Game
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;

    public class Bootstrap : MonoBehaviour
    {
        public GameObject waypointPathBlobAssetAuthoringPrefab;

        public GameObject neutralUnitPrefab;
        public GameObject teamUnitPrefab;

        public Camera camera;

        public Mesh unitMesh;
        public Material unitMaterial;
        
        void Start()
        {
            //
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var archetype = entityManager.CreateArchetype(
                typeof(TheEnvironment),
                typeof(NeutralUnitSpawn),
                typeof(TeamUnitSpawn));
            var environmentEntity = entityManager.CreateEntity(archetype);
            
            entityManager.SetComponentData(environmentEntity, new NeutralUnitSpawn
            {
                IntervalMax = 0.3f,
                CountDown = 0.0f
            });

            entityManager.SetComponentData(environmentEntity, new TeamUnitSpawn
            {
                IntervalMax = 0.1f,
                CountDown = 0.0f
            });

            //
            GameObject.Instantiate(waypointPathBlobAssetAuthoringPrefab);
            
            //
            var initializationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();
            var simulationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>();
            var presentationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PresentationSystemGroup>();

            //
            var spawnNeutralUnitSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<SpawnNeutralUnitSystem>();
            var spawnTeamUnitSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<SpawnTeamUnitSystem>();

            var moveOnWaypointPathSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<MoveOnWaypointPathSystem>();
            
            var teamUnitMoveSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<TeamUnitMoveSystem>();
            
            var neutralForceUnitHitCheckSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<NeutralForceUnitHitCheckSystem>();
            var removeNeutralAbsorbableSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<RemoveNeutralAbsorbableSystem>();
            var neutralForceUnitRenderSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<NeutralForceUnitRenderSystem>();

            //
            spawnNeutralUnitSystem.neutralUnitPrefab = neutralUnitPrefab;
            spawnTeamUnitSystem.teamUnitPrefab = teamUnitPrefab;
            
            moveOnWaypointPathSystem.CanUpdate = true;
            //teamUnitMoveSystem.CanUpdate = true;

            neutralForceUnitRenderSystem.SceneCamera = camera;
            neutralForceUnitRenderSystem.UnitMesh = unitMesh;
            neutralForceUnitRenderSystem.UnitMaterial = unitMaterial;

            //
            spawnNeutralUnitSystem.Construct();

            //
            initializationSystemGroup.AddSystemToUpdateList(spawnNeutralUnitSystem);
            initializationSystemGroup.AddSystemToUpdateList(spawnTeamUnitSystem);
            simulationSystemGroup.AddSystemToUpdateList(moveOnWaypointPathSystem);
            simulationSystemGroup.AddSystemToUpdateList(teamUnitMoveSystem);
            simulationSystemGroup.AddSystemToUpdateList(neutralForceUnitHitCheckSystem);
            simulationSystemGroup.AddSystemToUpdateList(removeNeutralAbsorbableSystem);
            presentationSystemGroup.AddSystemToUpdateList(neutralForceUnitRenderSystem);
        }
    }
}
