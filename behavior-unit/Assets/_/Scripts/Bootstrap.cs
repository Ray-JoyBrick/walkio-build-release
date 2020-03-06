namespace JoyBrick.Walkio.Game
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Rendering;
    using Unity.Transforms;
    using UnityEngine;
    using Random = Unity.Mathematics.Random;

    //
    public static class BootstrapUtility
    {
        public static void AddInitializationSystem<T>() where T : ComponentSystemBase
        {
            AddSystemToGroup<T, InitializationSystemGroup>();
        }

        public static void AddSimulationSystem<T>() where T : ComponentSystemBase
        {
            AddSystemToGroup<T, SimulationSystemGroup>();
        }

        public static void AddLateSimulationSystem<T>() where T : ComponentSystemBase
        {
            AddSystemToGroup<T, LateSimulationSystemGroup>();
        }

        public static void AddPresentationSystem<T>() where T : ComponentSystemBase
        {
            AddSystemToGroup<T, PresentationSystemGroup>();
        }

        static void AddSystemToGroup<TSystem, TGroup>()
            where TSystem : ComponentSystemBase
            where TGroup : ComponentSystemGroup
        {
            var group = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<TGroup>();
            var system = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<TSystem>();

            group.AddSystemToUpdateList(system);
        }
    }

    public class Bootstrap : MonoBehaviour
    {
        public GameObject unitPrefab;
        
        public Mesh unitMesh;
        public Material unitMaterial;

        private EntityManager _entityManager;
        private Random _rnd;

        void Start()
        {
            //
            _rnd = new Unity.Mathematics.Random((uint) System.DateTime.UtcNow.Ticks);
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            // //
            // CreateWorldMap();
            // CreatePathfind();
            // CreateUnit();

            // CreateUnitSpawner();
            
            //
            BootstrapUtility.AddSimulationSystem<PlayerInputSystem>();
            BootstrapUtility.AddSimulationSystem<UnitSpawnSystem>();
            BootstrapUtility.AddSimulationSystem<UnitMovementSystem>();
            BootstrapUtility.AddSimulationSystem<AssignNewTargetToUnitSystem>();
        }

        private void CreateWorldMap()
        {
            var archetype = _entityManager.CreateArchetype(
                typeof(MapCell));

            _entityManager.CreateEntity(archetype);
        }

        private void CreatePathfind()
        {
            var archetype = _entityManager.CreateArchetype(
                typeof(PathTile));

            _entityManager.CreateEntity(archetype);
        }

        private void CreatePlayerUnit()
        {
            var archetype = _entityManager.CreateArchetype(
                typeof(RenderMesh),
                typeof(RenderBounds),
                typeof(LocalToWorld),
                typeof(Translation),
                typeof(PlaybackPolicy),
                typeof(Team),
                typeof(Unit));

            _entityManager.CreateEntity(archetype);
        }

        private void CreateUnitSpawner()
        {
            var archetype = _entityManager.CreateArchetype(
                typeof(LocalToWorld),
                typeof(Translation),
                typeof(UnitSpawner));

            var entity = _entityManager.CreateEntity(archetype);
            
            _entityManager.SetComponentData(entity, new UnitSpawner
            {
                prefab = _entityManager.Instantiate(unitPrefab),
                countX = 3,
                countY = 3
            });
            
            _entityManager.SetComponentData(entity, new Translation
            {
                Value = float3.zero
            });
        }

        private void CreateUnit()
        {
            var archetype = _entityManager.CreateArchetype(
                typeof(RenderMesh),
                typeof(RenderBounds),
                typeof(LocalToWorld),
                typeof(Translation),
                typeof(Team),
                typeof(Unit),
                typeof(Minion));

            using (var unitEntities = _entityManager.CreateEntity(archetype, 10, Allocator.Temp))
            {
                foreach (var entity in unitEntities)
                {
                    _entityManager.SetSharedComponentData(entity, new RenderMesh
                    {
                        mesh = unitMesh,
                        material = unitMaterial
                    });

                    _entityManager.SetComponentData(entity, new Translation
                    {
                        Value = _rnd.NextFloat3(new float3(-5, -3, 0), new float3(5, 3, 0))
                    });
                }
            }
        }
    }

    //
    public struct MapCell : IBufferElementData
    {
        public int value;
        public static implicit operator int(MapCell v) => v.value;
        public static implicit operator MapCell(int v) => new MapCell {value = v};
    }
    
    //
    public struct PathTile : IBufferElementData
    {
        public int2 value;
        public static implicit operator int2(PathTile v) => v.value;
        public static implicit operator PathTile(int2 v) => new PathTile {value = v};
    }
    
    //
    public struct PlayerInput : IComponentData
    {
    }

    //
    public struct Team : IComponentData
    {
    }

    public struct Unit : IComponentData
    {
    }

    public struct UnitMovement : IComponentData
    {
        public float3 target;
        public float moveSpeed;
    }

    public struct UnitRequestNewTarget : IComponentData
    {
    }

    public struct Minion : IComponentData
    {
    }

    public struct UnitSpawner : IComponentData
    {
        public Entity prefab;

        public int countX;
        public int countY;
    }
    
    //
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class UnitSpawnSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem  _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem >();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var rnd = new Unity.Mathematics.Random((uint) System.DateTime.UtcNow.Ticks);
            
            Entities
                .WithName("SpawnUnitSystem")
                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
                .ForEach(
                    (Entity entity, int entityInQueryIndex, in UnitSpawner unitSpawner, in LocalToWorld location) =>
                    {
                        for (var x = 0; x < unitSpawner.countX; ++x)
                        {
                            for (var y = 0; y < unitSpawner.countY; ++y)
                            {
                                var instance = commandBuffer.Instantiate(entityInQueryIndex, unitSpawner.prefab);

                                var position = math.transform(
                                    location.Value,
                                    new float3(x * 1.3f, noise.cnoise(new float2(x, y) * 0.21f) * 2, y * 1.3f));
                            
                                // Has component Translation, just set it
                                commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = position });
                                // Has no component UnitMovement so adding it here
                                commandBuffer.AddComponent(entityInQueryIndex, instance, new UnitMovement
                                {
                                    target = rnd.NextFloat3(new float3(-20, -0, -20), new float3(20, 0, 20)),
                                    moveSpeed = rnd.NextFloat(1.0f, 3.0f)
                                });
                            
                                // Debug.Log($"Set comp for {entityInQueryIndex}");
                            }
                        }

                        commandBuffer.DestroyEntity(entityInQueryIndex, entity); 
                    }
                )
                .ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
    
    //
    [DisableAutoCreation]
    public class UnitMovementSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var deltaTime = Time.DeltaTime;

            Entities
                .WithName("UnitMovementSystem")
                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
                .ForEach(
                    (Entity entity, int entityInQueryIndex, ref UnitMovement unitMovement, ref Translation translation) =>
                    {
                        // Debug.Log($"This is unit movement");
                        var position = translation.Value;
                        var nearTarget = math.distance(unitMovement.target, position) < 0.1f;
                        if (nearTarget)
                        {
                            // Actually near target, ask to assign new target?
                            commandBuffer.AddComponent<UnitRequestNewTarget>(entityInQueryIndex, entity, new UnitRequestNewTarget());
                        }
                        else
                        {
                            var direction = math.normalize(unitMovement.target - position);
                            translation.Value += direction * deltaTime * unitMovement.moveSpeed;
                        }
                    }
                )
                .ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
    
    //
    [DisableAutoCreation]
    [UpdateAfter(typeof(UnitMovementSystem))]
    public class AssignNewTargetToUnitSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var rnd = new Unity.Mathematics.Random((uint) System.DateTime.UtcNow.Ticks);

            Entities
                .WithName("AssignNewTargetToUnitSystem")
                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
                .ForEach(
                    (Entity entity, int entityInQueryIndex, ref UnitMovement unitMovement, ref UnitRequestNewTarget unitRequestNewTarget) =>
                    {
                        unitMovement.target = rnd.NextFloat3(new float3(-20, -0, -20), new float3(20, 0, 20));
                        commandBuffer.RemoveComponent<UnitRequestNewTarget>(entityInQueryIndex, entity);
                    }
                )
                .ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
    
    //
    [DisableAutoCreation]
    public class PlayerInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<PlayerInput>()
                .ForEach((Entity entity) =>
                {
                    Debug.Log($"Entity: {entity}");
                    
                    //
                    var horizontal = Input.GetAxis("Horizontal");
                    var vertical = Input.GetAxis("Vertical");

                    if (horizontal != 0)
                    {
                        Debug.Log($"Player request horizontal input");
                    }

                    if (vertical != 0)
                    {
                        Debug.Log($"Player request vertical input");
                    }
                })
                .WithoutBurst()
                .Run();
        }
    }
}
