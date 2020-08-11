namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public partial class Bootstrap :
        GameCommon.IEcsSettingProvider
    {
        private GameObjectConversionSettings _gameObjectConversionSettings;
        private EntityManager _entityManager;

        public EntityManager EntityManager => _entityManager;

        public GameObjectConversionSettings RefGameObjectConversionSettings => _gameObjectConversionSettings;

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
            FlowControl_CleanupSystem(initializationSystemGroup);

            FlowControl_CleanupStageUseEntitySystem(initializationSystemGroup);
            PhysicsExtension_CleanupPhysicsConstrainedPairSystem(initializationSystemGroup);

            //
            FlowControl_PrepareAssetSystem(initializationSystemGroup);
            FlowControlPreparation_PrepareAssetSystem(initializationSystemGroup);
            FlowControlStage_PrepareAssetSystem(initializationSystemGroup);

            //
            HudApp_PrepareAssetSystem(initializationSystemGroup);
            HudApp_SetupAssetSystem(initializationSystemGroup);
            HudPreparation_PrepareAssetSystem(initializationSystemGroup);
            HudPreparation_SetupAssetSystem(initializationSystemGroup);
            HudStage_PrepareAssetSystem(initializationSystemGroup);
            HudStage_SetupAssetSystem(initializationSystemGroup);

            //
            Creature_PrepareAssetSystem(initializationSystemGroup);
            Creature_SpawnTeamUnitSystem(initializationSystemGroup);

            Level_PrepareAssetSystem(initializationSystemGroup);
            Level_SetupAssetSystem(initializationSystemGroup);

            //
            Level_CheckTeamForceCountSystem(initializationSystemGroup);
            Level_SystemP01(initializationSystemGroup);

            //
            MoveCrowdSimulate_PrepareAssetSystem(initializationSystemGroup);
            MoveCrowdSimulate_SetupAssetSystem(initializationSystemGroup);

            MoveFlowField_PrepareAssetSystem(initializationSystemGroup);
            MoveFlowField_SetupAssetSystem(initializationSystemGroup);

            MoveWaypoint_PrepareAssetSystem(initializationSystemGroup);
            MoveWaypoint_SetupAssetSystem(initializationSystemGroup);

            // MoveFlowField_SetupInitialLeadingToSetSystem(initializationSystemGroup);
            // MoveFlowField_CheckTargetAtTileChangeSystem(initializationSystemGroup);
            //
            // MoveFlowField_SystemC(initializationSystemGroup);
            // MoveFlowField_SystemD(initializationSystemGroup);
            // MoveFlowField_SystemE(initializationSystemGroup);
            // // MoveFlowField_SystemA(initializationSystemGroup);
            // MoveFlowField_SystemB(initializationSystemGroup);

            MoveFlowField_SystemH01(initializationSystemGroup);

            MoveFlowField_SystemM01(initializationSystemGroup);
            MoveFlowField_SystemM02(initializationSystemGroup);
            MoveFlowField_SystemM02_2(initializationSystemGroup);
            MoveFlowField_SystemM03(initializationSystemGroup);
            MoveFlowField_SystemM08(initializationSystemGroup);

            Level_SystemN01(simulationSystemGroup);

            MoveFlowField_SystemM09(simulationSystemGroup);

            MoveCrowdSimulate_SystemA(simulationSystemGroup);

            // MoveCrowdSimulate_SystemC01(simulationSystemGroup);
            // MoveCrowdSimulate_SystemC02(simulationSystemGroup);
            // MoveCrowdSimulate_SystemC03(simulationSystemGroup);

            //
            Creature_PresentUnitIndicationSystem(presentationSystemGroup);
        }

        void CleanUpEcsWorldContext()
        {
            _gameObjectConversionSettings?.BlobAssetStore.Dispose();
        }
    }
}
