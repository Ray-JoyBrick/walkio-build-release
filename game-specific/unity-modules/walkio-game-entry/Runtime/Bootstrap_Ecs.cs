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
            FlowControl_PrepareAssetSystem(initializationSystemGroup);
            FlowControlPreparation_PrepareAssetSystem(initializationSystemGroup);

            //
            HudApp_PrepareAssetSystem(initializationSystemGroup);
            HudApp_SetupAssetSystem(initializationSystemGroup);
            HudPreparation_PrepareAssetSystem(initializationSystemGroup);
            HudPreparation_SetupAssetSystem(initializationSystemGroup);
            HudStage_PrepareAssetSystem(initializationSystemGroup);
            HudStage_SetupAssetSystem(initializationSystemGroup);

            //
            Level_LoadAssetSystem(initializationSystemGroup);
            Level_SetupAssetSystem(initializationSystemGroup);

            //
            MoveFlowField_PrepareAssetSystem(initializationSystemGroup);
            MoveFlowField_SetupAssetSystem(initializationSystemGroup);

            // MoveFlowField_SetupInitialLeadingToSetSystem(initializationSystemGroup);
            // MoveFlowField_CheckTargetAtTileChangeSystem(initializationSystemGroup);
            //
            // MoveFlowField_SystemC(initializationSystemGroup);
            // MoveFlowField_SystemD(initializationSystemGroup);
            // MoveFlowField_SystemE(initializationSystemGroup);
            // // MoveFlowField_SystemA(initializationSystemGroup);
            // MoveFlowField_SystemB(initializationSystemGroup);

            MoveFlowField_SystemM01(initializationSystemGroup);
            MoveFlowField_SystemM02(initializationSystemGroup);
            MoveFlowField_SystemM03(initializationSystemGroup);
            MoveFlowField_SystemM08(initializationSystemGroup);

            MoveCrowdSimulate_SystemA(initializationSystemGroup);

        }

        void CleanUpEcsWorldContext()
        {
            _gameObjectConversionSettings?.BlobAssetStore.Dispose();
        }
    }
}
