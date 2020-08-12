namespace JoyBrick.Walkio.Game.Move.CrowdSimulate.Assist
{
    using UniRx;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;

    //
#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    [DisableAutoCreation]
    public class PresentParticleIndication : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(PresentParticleIndication));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginPresentationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _entityQuery;

#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        //
        private bool _canUpdate;

        //

        //
        public void Construct()
        {
            _logger.Debug($"Module - PresentParticleIndication - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - PresentParticleIndication - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
// #else
//             Observable.Timer(System.TimeSpan.FromMilliseconds(500))
//                 .Subscribe(_ =>
//                 {
//                     _canUpdate = true;
//                 })
//                 .AddTo(_compositeDisposable);
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
                    typeof(ParticleIndication)
                }
            });
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            // var gridCellCount = new int2(256, 192);
            // var gridCellSize = new float2(1.0f, 1.0f);
            // var flowFieldWorldData = FlowFieldWorldProvider.FlowFieldWorldData as GameMoveFlowField.Template.FlowFieldWorldData;
            // var tileCellCount = new int2(flowFieldWorldData.tileCellCount.x, flowFieldWorldData.tileCellCount.y);
            // var tileCellSize = (float2)flowFieldWorldData.tileCellSize;
            //
            // var oneTileOffset = tileCellCount * tileCellSize;
            // var basePosition = new float3(0, 0.1f, 0);
            //
            // // _entityQuery.CreateArchetypeChunkArray(Allocator.TempJob);
            //
            // // var a = _entityQuery.ToComponentDataArray<FlowFieldMoveIndication>();
            // var flowFieldMoveIndications = GetArchetypeChunkComponentType<FlowFieldMoveIndication>();
            //
            // var assistFlowFieldWorldData =
            //     AssistFlowFieldWorldProvider.FlowFieldWorldData as Template.FlowFieldWorldData;
            //
            // using (var commandBuilder = DrawingManager.GetBuilder(true))
            // {
            //     Entities
            //         .WithAll<TemporaryPointIndication>()
            //         .ForEach((Entity entity, TemporaryPointIndicationProperty temporaryPointIndicationProperty) =>
            //         {
            //             var groupId = temporaryPointIndicationProperty.GroupId;
            //             var color = assistFlowFieldWorldData.groups[groupId].color;
            //             var pos = temporaryPointIndicationProperty.Location;
            //             pos.y = 0.1f;
            //             commandBuilder.CircleXZ(pos, 0.5f, color);
            //
            //         })
            //         // .WithBurst()
            //         // .Schedule();
            //         .WithoutBurst()
            //         .Run();
            // }

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
