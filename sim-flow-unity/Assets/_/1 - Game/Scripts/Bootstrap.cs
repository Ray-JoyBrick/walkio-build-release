namespace Game
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;

    public class Bootstrap : MonoBehaviour
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(Bootstrap));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        
        void SetupEcsWorldSystem()
        {
            _logger.Debug($"Bootstrap - SetupEcsWorldSystem");

            //
            var initializationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitializationSystemGroup>();
            var simulationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>();
            var presentationSystemGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PresentationSystemGroup>();
            
            var presentWorldSystem =
                World.DefaultGameObjectInjectionWorld
                    .GetOrCreateSystem<Move.FlowField.PresentWorldSystem>();
            
            presentationSystemGroup.AddSystemToUpdateList(presentWorldSystem);
        }

        void Start()
        {
            SetupEcsWorldSystem();
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
    
}
