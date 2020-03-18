namespace JoyBrick.Walkio.Game.Utility
{
    using Unity.Entities;

    public static class Bootstrap
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

        public static void AddGameObjectBeforeConversionSystem<T>() where T : ComponentSystemBase
        {
            AddSystemToGroup<T, GameObjectBeforeConversionGroup>();
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
}