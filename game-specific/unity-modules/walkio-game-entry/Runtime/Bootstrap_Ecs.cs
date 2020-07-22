namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;

    public partial class Bootstrap
    {
        private GameObjectConversionSettings _gameObjectConversionSettings;
        private EntityManager _entityManager;

        public EntityManager EntityManager => _entityManager;

        void SetupEcsWorldContext()
        {
            _logger.Debug($"Bootstrap - SetupEcsWorldContext");

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            _gameObjectConversionSettings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, new BlobAssetStore());
        }

        void SetupEcsWorldSystem()
        {
            _logger.Debug($"Bootstrap - SetupEcsWorldSystem");

            //
            var initializationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();
            var simulationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>();
            var presentationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PresentationSystemGroup>();

            //
            FlowControl_LoadingDoneCheckSystem(initializationSystemGroup);
            FlowControl_SettingDoneCheckSystem(initializationSystemGroup);

            //
            HudApp_LoadAssetSystem(initializationSystemGroup);

            //
            Level_LoadAssetSystem(initializationSystemGroup);
            Level_SetupAssetSystem(initializationSystemGroup);

            //
            MoveFlowField_LoadAssetSystem(initializationSystemGroup);
            MoveFlowField_SetupAssetSystem(initializationSystemGroup);

            MoveFlowField_SetupInitialLeadingToSetSystem(initializationSystemGroup);
            MoveFlowField_CheckTargetAtTileChangeSystem(initializationSystemGroup);
            //
            MoveFlowField_SystemA(initializationSystemGroup);
            MoveFlowField_SystemB(initializationSystemGroup);
        }

        void CleanUpEcsWorldContext()
        {
            _gameObjectConversionSettings?.BlobAssetStore.Dispose();
        }
    }
}
