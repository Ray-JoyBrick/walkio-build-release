namespace JoyBrick.Walkio.Game.Move.FlowField.Assist
{
    using System.Collections.Generic;
    using Common;
    using Drawing;
    using UniRx;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.Rendering;

    //
#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    [DisableAutoCreation]
    public class PresentWorldSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(PresentWorldSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginPresentationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _entityQuery;

        //
        private bool _canUpdate;

#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif
        public IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

        public void Construct()
        {
            _logger.Debug($"PresentWorldSystem - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"PresentWorldSystem - Construct - Receive FlowReadyToStart");
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
                    typeof(FlowFieldWorld),
                    typeof(FlowFieldWorldProperty)
                }
            });
        }

        [BurstCompile]
        struct DrawingJob : IJob
        {
            public FlowFieldWorldProperty FlowFieldWorldProperty;
            public CommandBuilder CommandBuilder;

            private void DrawBoundaryGridTile(
                int hTileIndex, int vTileIndex,
                int2 tileCount,
                float2 oneTileOffset,
                float3 basePosition)
            {
                if (hTileIndex == 0 || vTileIndex == 0 ||
                    hTileIndex == tileCount.x - 1 || vTileIndex == tileCount.y - 1)
                {
                    var tilePos = new float3(
                        hTileIndex * oneTileOffset.x,
                        0,
                        vTileIndex * oneTileOffset.y);
                    tilePos += basePosition;
                    var cells = FlowFieldWorldProperty.TileCellCount;
                    var totalSizes = oneTileOffset;
                    CommandBuilder.WireGrid(tilePos, Quaternion.identity, cells, totalSizes);
                }
            }

            public void Execute()
            {
                for (var vTileIndex = 0; vTileIndex < FlowFieldWorldProperty.TileCount.y; ++vTileIndex)
                {
                    for (var hTileIndex = 0; hTileIndex < FlowFieldWorldProperty.TileCount.x; ++hTileIndex)
                    {
                        var basePosition = FlowFieldWorldProperty.OriginPosition - FlowFieldWorldProperty.PositionOffset;

                        var oneTileOffset = FlowFieldWorldProperty.TileCellCount * FlowFieldWorldProperty.TileCellSize;

                        // Debug.Log($"oneTileOffset: {oneTileOffset}");

                        DrawBoundaryGridTile(hTileIndex, vTileIndex, FlowFieldWorldProperty.TileCount, oneTileOffset, basePosition);

                        // var tilePos = new float3(
                        //     hTileIndex * oneTileOffset.x,
                        //     0,
                        //     vTileIndex * oneTileOffset.y);
                        // tilePos += basePosition;
                        // var cells = FlowFieldWorldProperty.TileCellCount;
                        // var totalSizes = oneTileOffset;
                        // CommandBuilder.WireGrid(tilePos, Quaternion.identity, cells, totalSizes);

                        // for (var vTileCellIndex = 0;
                        //     vTileCellIndex < FlowFieldWorldProperty.TileCellCount.y;
                        //     ++vTileCellIndex)
                        // {
                        //     for (var hTileCellIndex = 0;
                        //         hTileCellIndex < FlowFieldWorldProperty.TileCellCount.x;
                        //         ++hTileCellIndex)
                        //     {
                        //         var boxSize = new float3(1.0f, 2.0f, 1.0f);
                        //         var boxPos = new float3(tilePos.x + hTileCellIndex, 0, tilePos.z + vTileCellIndex);
                        //         boxPos = boxPos + FlowFieldWorldProperty.PositionOffset;
                        //         var bounds = new Bounds(boxPos, boxSize);
                        //         // Use bounds will have no error
                        //         CommandBuilder.WireBox(bounds, Color.blue);
                        //     }
                        // }

                    }
                }
            }
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var flowFieldWorldProperty = _entityQuery.GetSingleton<FlowFieldWorldProperty>();

            // _logger.Debug($"PresentWorldSystem - OnUpdate - Going to draw entity: {flowFieldWorldProperty}");

            using (var commandBuilder = DrawingManager.GetBuilder(false))
            {
                new DrawingJob
                {
                    FlowFieldWorldProperty = flowFieldWorldProperty,
                    CommandBuilder = commandBuilder
                }.Schedule().Complete();
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
