namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;

    //
#if WALKIO_COMMON
    using GameCommon = JoyBrick.Walkio.Game.Common;
#endif
    
    [DisableAutoCreation]
    public class SetupMonitorTileChangeSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupMonitorTileChangeSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        //
        private bool _canUpdate;

#if WALKIO_COMMON
        public GameCommon.IFlowControl FlowControl { get; set; }
#endif

        public void Construct()
        {
            _logger.Debug($"SetupMonitorTileChangeSystem - Construct");

#if WALKIO_COMMON
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"SetupMonitorTileChangeSystem - Construct - Receive DoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            //
            Entities
                .WithAll<MonitorTileChange>()
                .ForEach((Entity entity, ref MonitorTileChangeProperty monitorTileChangeProperty) =>
                {
                    monitorTileChangeProperty.CanMonitor = true;
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
