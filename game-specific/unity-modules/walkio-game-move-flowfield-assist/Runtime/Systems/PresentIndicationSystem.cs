namespace JoyBrick.Walkio.Game.Move.FlowField.Assist
{
    using Drawing;
    using UniRx;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;

    [DisableAutoCreation]
    public class PresentIndicationSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(PresentIndicationSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginPresentationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _entityQuery;

        //
        private bool _canUpdate;

        public void Construct()
        {
            _logger.Debug($"PresentWorldSystem - Construct");

#if WALKIO_FLOWCONTROL_SYSTEM
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"PresentIndicationSystem - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
#else
            Observable.Timer(System.TimeSpan.FromMilliseconds(500))
                .Subscribe(_ =>
                {
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();

            _entityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(FlowFieldMoveIndication)
                }
            });
        }

        [BurstCompile]
        struct DrawingJob : IJob
        {
            public ArchetypeChunkComponentType<FlowFieldMoveIndication> FlowFieldIndications;
            public CommandBuilder CommandBuilder;

            public void Execute()
            {
                // for (var i = 0; i < FlowFieldIndications.)
            }
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            // _entityQuery.CreateArchetypeChunkArray(Allocator.TempJob);

            // var a = _entityQuery.ToComponentDataArray<FlowFieldMoveIndication>();
            var flowFieldMoveIndications = GetArchetypeChunkComponentType<FlowFieldMoveIndication>();

            using (var commandBuilder = DrawingManager.GetBuilder(true))
            {
                Entities
                    .WithAll<FlowFieldMoveIndication>()
                    .ForEach((Entity entity, LocalToWorld localToWorld) =>
                    {
                        var boxSize = new float3(1.0f, 1.6f, 1.0f);
                        var adjustedPos = localToWorld.Position;
                        adjustedPos.y = 0.8f;
                        var bounds = new Bounds(adjustedPos, boxSize);
                        // Use bounds will have no error
                        commandBuilder.WireBox(bounds, Color.yellow);
                    })
                    // .WithBurst()
                    // .Schedule();
                    .WithoutBurst()
                    .Run();
                // new DrawingJob
                // {
                //     FlowFieldIndications = flowFieldMoveIndications,
                //     CommandBuilder = commandBuilder
                // }.Schedule().Complete();
            }

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
