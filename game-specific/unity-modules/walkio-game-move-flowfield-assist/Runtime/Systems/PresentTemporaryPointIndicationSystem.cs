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

    //
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;

    //
    using GameMoveFlowField = JoyBrick.Walkio.Game.Move.FlowField;
    using GameMoveFlowFieldUtility = JoyBrick.Walkio.Game.Move.FlowField.Utility;

    [DisableAutoCreation]
    public class PresentTemporaryPointIndicationSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(PresentTemporaryPointIndicationSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginPresentationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _entityQuery;

        //
        private bool _canUpdate;

        //
        public GameMoveFlowField.IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }
        public GameMoveFlowField.Assist.IFlowFieldWorldProvider AssistFlowFieldWorldProvider { get; set; }

        public GameFlowControl.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"Module Assist - Move - FlowField - PresentTemporaryPointIndicationSystem - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module Assist - Move - FlowField - PresentTemporaryPointIndicationSystem - Construct - Receive FlowReadyToStart");
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

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            var gridCellCount = new int2(256, 192);
            var gridCellSize = new float2(1.0f, 1.0f);
            var flowFieldWorldData = FlowFieldWorldProvider.FlowFieldWorldData as GameMoveFlowField.Template.FlowFieldWorldData;
            var tileCellCount = new int2(flowFieldWorldData.tileCellCount.x, flowFieldWorldData.tileCellCount.y);
            var tileCellSize = (float2)flowFieldWorldData.tileCellSize;

            var oneTileOffset = tileCellCount * tileCellSize;
            var basePosition = new float3(0, 0.1f, 0);

            // _entityQuery.CreateArchetypeChunkArray(Allocator.TempJob);

            // var a = _entityQuery.ToComponentDataArray<FlowFieldMoveIndication>();
            var flowFieldMoveIndications = GetComponentTypeHandle<FlowFieldMoveIndication>();

            var assistFlowFieldWorldData =
                AssistFlowFieldWorldProvider.FlowFieldWorldData as Template.FlowFieldWorldData;

            using (var commandBuilder = DrawingManager.GetBuilder(false))
            {
                Entities
                    .WithAll<TemporaryPointIndication>()
                    .ForEach((Entity entity, TemporaryPointIndicationProperty temporaryPointIndicationProperty) =>
                    {
                        var groupId = temporaryPointIndicationProperty.GroupId;
                        var color = assistFlowFieldWorldData.groups[groupId].color;
                        var pos = temporaryPointIndicationProperty.Location;
                        pos.y = 0.1f;
                        commandBuilder.CircleXZ(pos, 0.5f, color);

                    })
                    // .WithBurst()
                    // .Schedule();
                    .WithoutBurst()
                    .Run();
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
