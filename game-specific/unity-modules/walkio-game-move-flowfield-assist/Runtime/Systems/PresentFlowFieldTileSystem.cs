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
    using GameLevel = JoyBrick.Walkio.Game.Level;

    using GameMoveFlowField = JoyBrick.Walkio.Game.Move.FlowField;
    using GameMoveFlowFieldUtility = JoyBrick.Walkio.Game.Move.FlowField.Utility;

    [DisableAutoCreation]
    public class PresentFlowFieldTileSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(PresentFlowFieldTileSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginPresentationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _entityQuery;
        private EntityQuery _gridWorldEntityQuery;

        //
        private bool _canUpdate;

        //
        public GameLevel.IGridWorldProvider GridWorldProvider { get; set; }
        public GameMoveFlowField.IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }
        public IFlowFieldWorldProvider AssistFlowFieldWorldProvider { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"PresentWorldSystem - Construct");

#if WALKIO_FLOWCONTROL_SYSTEM
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - PresentIndicationSystem - Construct - Receive AllDoneSettingAsset");
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

            _gridWorldEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(GameLevel.GridWorld),
                    typeof(GameLevel.GridWorldProperty)
                }
            });

            RequireForUpdate(_entityQuery);
            RequireForUpdate(_gridWorldEntityQuery);
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            var gridWorldProperty = _gridWorldEntityQuery.GetSingleton<GameLevel.GridWorldProperty>();

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            // var gridCellCount = new int2(256, 192);
            // var gridCellSize = new float2(1.0f, 1.0f);
            var gridCellCount = gridWorldProperty.CellCount;
            var gridCellSize = gridWorldProperty.CellSize;
            var flowFieldWorldData = FlowFieldWorldProvider.FlowFieldWorldData as GameMoveFlowField.Template.FlowFieldWorldData;
            var tileCellCount = new int2(flowFieldWorldData.tileCellCount.x, flowFieldWorldData.tileCellCount.y);
            var tileCellSize = (float2)flowFieldWorldData.tileCellSize;

            var oneTileOffset = tileCellCount * tileCellSize;
            var basePosition = new float3(0, 0.1f, 0);

            // _logger.Debug($"Module - PresentFlowFieldTileSystem - OnUpdate - cell count: {gridCellCount}, cell size: {gridCellSize} tile cell count: {tileCellCount} tileCellSize: {tileCellSize} oneTileOffset: {oneTileOffset}");

            //
            var assistFlowFieldWorldData = AssistFlowFieldWorldProvider.FlowFieldWorldData as Template.FlowFieldWorldData;


            // _entityQuery.CreateArchetypeChunkArray(Allocator.TempJob);

            // var a = _entityQuery.ToComponentDataArray<FlowFieldMoveIndication>();
            var flowFieldMoveIndications = GetArchetypeChunkComponentType<FlowFieldMoveIndication>();

            using (var commandBuilder = DrawingManager.GetBuilder(true))
            {
                Entities
                    .WithAll<FlowFieldTile>()
                    .ForEach((Entity entity, FlowFieldTileProperty flowFieldTileProperty) =>
                    {
                        var tileIndex = flowFieldTileProperty.TileIndex;
                        var centerOfTile =
                            GameMoveFlowFieldUtility.FlowFieldTileHelper.TileIndexToPosition2D(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                tileIndex);
                        // var tilePos = new float3(centerOfTile.x, 0, centerOfTile.y) + new float3(oneTileOffset.x, 0, oneTileOffset.y);
                        var tilePos = new float3(centerOfTile.x, 0, centerOfTile.y);
                        tilePos += basePosition;
                        var cells = tileCellCount;
                        var totalSizes = oneTileOffset;

                        var group = assistFlowFieldWorldData.groups[flowFieldTileProperty.GroupId];
                        var color = group.color;

                        commandBuilder.WireGrid(tilePos, Quaternion.identity, cells, totalSizes, color);
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
