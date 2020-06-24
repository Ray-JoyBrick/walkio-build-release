namespace JoyBrick.Walkio.Game.Move.Waypoint
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class ReplaceWaypointMoveIndicationSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(ReplaceWaypointMoveIndicationSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        public GameCommon.IFlowControl FlowControl { get; set; }
        public GameObject NeutralUnitPrefab { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            //
            Entities
                .WithAll<WaypointMoveIndication>()
                .ForEach((Entity entity) =>
                {
                    //
                    commandBuffer.RemoveComponent<WaypointMoveIndication>(entity);
                    
                    //
                    var startPathIndex = 0;
                    var endPathIndex = 1;

                    commandBuffer.AddComponent(entity, new MoveOnWaypointPath());
                    commandBuffer.AddComponent(entity, new MoveOnWaypointPathProperty
                    {
                        StartPathIndex = startPathIndex,
                        EndPathIndex = endPathIndex,
                        
                        AtIndex = startPathIndex                        
                    });
                })
                .WithoutBurst()
                .Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency); 
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
}
