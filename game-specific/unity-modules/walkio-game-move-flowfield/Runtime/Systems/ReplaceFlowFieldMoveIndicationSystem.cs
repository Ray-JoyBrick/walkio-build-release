namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    // using Unity.Physics;
    using Unity.Transforms;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class ReplaceFlowFieldMoveIndicationSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(ReplaceFlowFieldMoveIndicationSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        public GameCommon.IFlowControl FlowControl { get; set; }
        public GameObject NeutralUnitPrefab { get; set; }

        protected override void OnCreate()
        {
            _logger.Debug($"ReplaceFlowFieldMoveIndicationSystem - OnCreate");
    
            base.OnCreate();

            //
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            //
            Entities
                .WithAll<FlowFieldMoveIndication>()
                .ForEach((Entity entity) =>
                {
                    //
                    commandBuffer.RemoveComponent<FlowFieldMoveIndication>(entity);
                    
                    // TODO: Replace the following code to get the actual start and end index for the path
                    commandBuffer.AddComponent(entity, new MoveOnFlowFieldTile());
                    commandBuffer.AddComponent(entity, new MoveOnFlowFieldTileProperty
                    {
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
