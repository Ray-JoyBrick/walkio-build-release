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
    using GameMoveFlowField = JoyBrick.Walkio.Game.Move.FlowField;
    using GameMoveFlowFieldUtility = JoyBrick.Walkio.Game.Move.FlowField.Utility;

    [DisableAutoCreation]
    public class PresentChasedTargetSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(PresentChasedTargetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginPresentationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _entityQuery;

        //
        private bool _canUpdate;

        //
        public GameMoveFlowField.IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"Module - PresentChasedTargetSystem - Construct");

#if WALKIO_FLOWCONTROL_SYSTEM
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - PresentChasedTargetSystem - Construct - Receive AllDoneSettingAsset");
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

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var gridCellCount = new int2(256, 192);
            var gridCellSize = new float2(1.0f, 1.0f);
            var flowFieldWorldData = FlowFieldWorldProvider.FlowFieldWorldData as GameMoveFlowField.Template.FlowFieldWorldData;
            var tileCellCount = new int2(flowFieldWorldData.tileCellCount.x, flowFieldWorldData.tileCellCount.y);
            var tileCellSize = (float2)flowFieldWorldData.tileCellSize;

            var oneTileOffset = tileCellCount * tileCellSize;
            var basePosition = new float3(0, 0.1f, 0);

            // _entityQuery.CreateArchetypeChunkArray(Allocator.TempJob);

            // var a = _entityQuery.ToComponentDataArray<FlowFieldMoveIndication>();
            var flowFieldMoveIndications = GetArchetypeChunkComponentType<FlowFieldMoveIndication>();

            using (var commandBuilder = DrawingManager.GetBuilder(false))
            {
                Entities
                    .WithAll<ToBeChasedTarget>()
                    .ForEach((Entity entity, LocalToWorld localToWorld, ToBeChasedTargetProperty toBeChasedTargetProperty) =>
                    {
                        var tileIndex = toBeChasedTargetProperty.AtTileIndex;
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
                        commandBuilder.WireGrid(tilePos, Quaternion.identity, cells, totalSizes);

                        var boxSize = new float3(1.0f, 1.6f, 1.0f);
                        var adjustedPos = localToWorld.Position;
                        adjustedPos.y = 0.1f;
                        // var bounds = new Bounds(adjustedPos, boxSize);
                        // // Use bounds will have no error
                        // commandBuilder.WireBox(bounds, Color.yellow);

                        commandBuilder.CrossXZ(adjustedPos, Color.green);
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
