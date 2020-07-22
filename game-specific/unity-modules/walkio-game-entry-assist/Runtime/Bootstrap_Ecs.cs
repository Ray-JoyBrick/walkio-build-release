namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using UniRx;
    using UnityEngine;
    using Unity.Entities;

    public partial class Bootstrap
    {
        void SetupEcsWorldContext()
        {
            _logger.Debug($"Bootstrap Assist - SetupEcsWorldContext");
        }

        void SetupEcsWorldSystem()
        {
            _logger.Debug($"Bootstrap Assist - SetupEcsWorldSystem");

            var initializationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();
            var simulationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>();
            var presentationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PresentationSystemGroup>();

            //
            MoveFlowField_RemoveTemporaryPointIndicationSystem(initializationSystemGroup);

            //
            Level_PresentWorldSystem(presentationSystemGroup);
            //
            MoveFlowField_PresentChasedTargetSystem(presentationSystemGroup);
            MoveFlowField_PresentFlowFieldTileSystem(presentationSystemGroup);
            MoveFlowField_PresentIndicationSystem(presentationSystemGroup);
            MoveFlowField_PresentWorldSystem(presentationSystemGroup);
            MoveFlowField_PresentTemporaryPointIndicationSystem(presentationSystemGroup);
        }

        void CleanUpEcsWorldContext()
        {
        }
    }
}
