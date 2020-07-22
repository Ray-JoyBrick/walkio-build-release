namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;

    //
#if WALKIO_LEVEL
    using GameLevel = JoyBrick.Walkio.Game.Level;
#endif

#if WALKIO_MOVE_FLOWFIELD
    using GameMoveFlowField = JoyBrick.Walkio.Game.Move.FlowField;
#endif

    public partial class Bootstrap
    {
        private void Level_LoadAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_LEVEL
            _logger.Debug($"Bootstrap - Module Creation - Level_LoadAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameLevel.LoadAssetSystem>();

            //
            createdSystem.GridWorldProvider = (GameLevel.IGridWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - Level_LoadAssetSystem");
#endif
        }

        private void Level_SetupAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_LEVEL
            _logger.Debug($"Bootstrap - Module Creation - Level_SetupAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameLevel.SetupAssetSystem>();

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - Level_SetupAssetSystem");
#endif
        }

        private void MoveFlowField_LoadAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_LoadAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.LoadAssetSystem>();

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_LoadAssetSystem");
#endif
        }

        private void MoveFlowField_SetupAssetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SetupAssetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SetupAssetSystem>();

            //
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SetupAssetSystem");
#endif
        }

        private void MoveFlowField_SetupInitialLeadingToSetSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SetupInitialLeadingToSetSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SetupInitialLeadingToSetSystem>();

            //
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SetupInitialLeadingToSetSystem");
#endif
        }

        private void MoveFlowField_CheckTargetAtTileChangeSystem(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_CheckTargetAtTileChangeSystem");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.CheckTargetAtTileChangeSystem>();

            //
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_CheckTargetAtTileChangeSystem");
#endif
        }

        private void MoveFlowField_SystemA(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SystemA");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SystemA>();

            //
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SystemA");
#endif
        }

        private void MoveFlowField_SystemB(ComponentSystemGroup componentSystemGroup)
        {
#if WALKIO_MOVE_FLOWFIELD
            _logger.Debug($"Bootstrap - Module Creation - MoveFlowField_SystemB");

            var createdSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<GameMoveFlowField.SystemB>();

            //
            createdSystem.FlowFieldWorldProvider = (GameMoveFlowField.IFlowFieldWorldProvider) this;

            //
            createdSystem.Construct();

            //
            componentSystemGroup.AddSystemToUpdateList(createdSystem);
#else
            _logger.Debug($"Bootstrap - No Module - MoveFlowField_SystemB");
#endif
        }
    }
}
