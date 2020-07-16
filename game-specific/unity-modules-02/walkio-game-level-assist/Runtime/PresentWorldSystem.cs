namespace JoyBrick.Walkio.Game.Level.Assist
{
    using System.Collections.Generic;
    using Common;
    using Drawing;
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.Rendering;

    //
    [DisableAutoCreation]
    public class PresentWorldSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(PresentWorldSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginPresentationEntityCommandBufferSystem _entityCommandBufferSystem;

        //
        private bool _canUpdate;

#if WALKIO_FLOWCONTROL_SYSTEM
        public GameCommon.IFlowControl FlowControl { get; set; }
#endif
        public IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }
        
        public void Construct()
        {
            _logger.Debug($"PresentWorldSystem - Construct");

#if WALKIO_FLOWCONTROL_SYSTEM
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"PresentWorldSystem - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();
        }

        // private void UpdateEachFlowFieldTile(
        //     FlowFieldWorldProperty flowFieldWorldProperty,
        //     FlowFieldTileProperty flowFieldTileProperty,
        //     DynamicBuffer<GameMove.FlowField.FlowFieldTileCellBuffer> buffer,
        //     CommandBuilder builder)
        // {
        //     _logger.Debug($"PresentWorldSystem - OnUpdate - flowFieldTileProperty: {flowFieldTileProperty}");

        //     var tilePosIndexX = flowFieldTileProperty.Index % flowFieldWorldProperty.TileCount.x;
        //     var tilePosIndexZ = flowFieldTileProperty.Index / flowFieldWorldProperty.TileCount.y;

        //     var tilePosX = tilePosIndexX * flowFieldWorldProperty.TileCellCount.x - 8.0f;
        //     var tilePosZ = tilePosIndexZ * flowFieldWorldProperty.TileCellCount.y - 8.0f;

        //     // _logger.Debug($"PresentWorldSystem - OnUpdate - Going to draw entity: {entity}");
        //     var tilePos = new float3(tilePosX, 0, tilePosZ);
        //     var cells = flowFieldWorldProperty.TileCellCount;
        //     var totalSizes = new float2(flowFieldWorldProperty.TileCellCount.x * 1.0f,
        //         flowFieldWorldProperty.TileCellCount.y * 1.0f);
        //     builder.WireGrid(tilePos, Quaternion.identity, cells, totalSizes);

        //     var direction = new float3(0, 0, 1.0f);
        //     var radius = 0.25f;
        //     var color = new Color32(100, 255, 100, 255);

        //     // var buffer = EntityManager.GetBuffer<FlowFieldTileCellBuffer>(entity);
        //     // var buffer = GetBufferFromEntity<FlowFieldTileCellBuffer>();
        //     // var tileCellCount = 

        //     for (var v = 0; v < 10; ++v)
        //     {
        //         for (var h = 0; h < 10; ++h)
        //         {
        //             var pos = new float3(
        //                 tilePos.x + h - 5.0f + 0.5f,
        //                 0,
        //                 tilePos.z + v - 5.0f + 0.5f);

        //             var posIndex = v * 10 + h;

        //             var directionValue = buffer[posIndex];

        //             if (directionValue == 0)
        //             {
        //                 direction = math.normalize(new float3(0, 0, 1.0f));
        //             }
        //             else if (directionValue == 1)
        //             {
        //                 direction = math.normalize(new float3(1.0f, 0, 1.0f));
        //             }
        //             else if (directionValue == 2)
        //             {
        //                 direction = math.normalize(new float3(1.0f, 0, 0.0f));
        //             }
        //             else if (directionValue == 3)
        //             {
        //                 direction = math.normalize(new float3(1.0f, 0, -1.0f));
        //             }
        //             else if (directionValue == 4)
        //             {
        //                 direction = math.normalize(new float3(0.0f, 0, -1.0f));
        //             }
        //             else if (directionValue == 5)
        //             {
        //                 direction = math.normalize(new float3(-1.0f, 0, -1.0f));
        //             }
        //             else if (directionValue == 6)
        //             {
        //                 direction = math.normalize(new float3(-1.0f, 0, 0.0f));
        //             }
        //             else if (directionValue == 7)
        //             {
        //                 direction = math.normalize(new float3(-1.0f, 0, 1.0f));
        //             }

        //             builder.Arrowhead(pos, direction, radius, color);

        //             // builder.Label2D(pos, "Cool", Color.white);
        //         }
        //     }

        // }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            // var flowFieldWorldProperty =
            //     EntityManager.GetComponentData<GameMove.FlowField.Common.FlowFieldWorldProperty>(
            //         FlowFieldWorldProvider.FlowFieldWorldEntity);

            var gridWorldProperty = GetSingleton<GridWorldProperty>();
            
            using (var builder = DrawingManager.GetBuilder(true))
            {
                // Entities
                //     .WithAll<GameMove.FlowField.FlowFieldTile>()
                //     .ForEach((Entity entity, GameMove.FlowField.FlowFieldTileProperty flowFieldTileProperty,
                //         DynamicBuffer<GameMove.FlowField.FlowFieldTileCellBuffer> buffer) =>
                //     {
                //         UpdateEachFlowFieldTile(flowFieldWorldProperty, flowFieldTileProperty, buffer, builder);
                //     })
                //     .WithoutBurst()
                //     .Run();
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
