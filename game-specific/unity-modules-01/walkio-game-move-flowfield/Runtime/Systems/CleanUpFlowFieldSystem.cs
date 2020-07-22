namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    //
    [DisableAutoCreation]
    [UpdateAfter(typeof(AdjustMoveToTargetFlowFieldSystem))]
    public class CleanUpFlowFieldSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(CleanUpFlowFieldSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _theEnvironmentQuery;

        private bool _canUpdate;
        
        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            _logger.Debug($"CleanUpFlowFieldSystem - Construct");

            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"CleanUpFlowFieldSystem - Construct - Receive DoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
            
            FlowControl.CleaningAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _canUpdate = false;
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            //
            var deltaTime = Time.DeltaTime;

            Entities
                .WithAll<DiscardedFlowFieldTile>()
                .ForEach((Entity entity, ref DiscardedFlowFieldTileProperty discardedFlowFieldTileProperty) =>
                {
                    
                    var elapsedTime = discardedFlowFieldTileProperty.CountDown + deltaTime;

                    discardedFlowFieldTileProperty.CountDown = elapsedTime;

                    if (elapsedTime >= discardedFlowFieldTileProperty.IntervalMax)
                    {
                        discardedFlowFieldTileProperty.CountDown = 0;

                        _logger.Debug($"CleanUpFlowFieldSystem - OnUpdate - timed Destroy {entity}");

                        if (entity != Entity.Null)
                        {
                            commandBuffer.DestroyEntity(entity);
                        }
                    }

                    commandBuffer.SetComponent(entity, discardedFlowFieldTileProperty);
                })
                // .Schedule();
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
