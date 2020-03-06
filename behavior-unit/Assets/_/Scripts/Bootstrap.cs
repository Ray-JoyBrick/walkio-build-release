namespace JoyBrick.Walkio.Game
{
    using System.Collections;
    using System.Collections.Generic;
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
        public Mesh unitMesh;
        public Material unitMaterial;

        private EntityManager _entityManager;
        private Random _rnd;

        void Start()
        {
            //
            _rnd = new Unity.Mathematics.Random((uint) System.DateTime.UtcNow.Ticks);
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            CreateWorldMap();
            CreatePathfind();
            CreateUnit();
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

        private void CreateUnit()
        {
            var unityArchetype = _entityManager.CreateArchetype(
                typeof(RenderMesh),
                typeof(RenderBounds),
                typeof(LocalToWorld),
                typeof(Translation),
                typeof(Team),
                typeof(Unit));

            using (var unitEntities = _entityManager.CreateEntity(unityArchetype, 10, Allocator.Temp))
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
    public struct Team : IComponentData
    {
    }

    public struct Unit : IComponentData
    {
    }
}
