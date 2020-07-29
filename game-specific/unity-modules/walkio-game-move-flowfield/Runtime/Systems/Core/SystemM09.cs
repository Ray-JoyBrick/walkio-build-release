namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameMove = JoyBrick.Walkio.Game.Move;

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    using GameLevel = JoyBrick.Walkio.Game.Level;

    [DisableAutoCreation]
    // [UpdateAfter(typeof(SystemA))]
    public class SystemM09 : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemM09));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _gridWorldEntityQuery;

        //
        private bool _canUpdate;

#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        //
        public IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

        public void Construct()
        {
            _logger.Debug($"Module - Move - FlowField - SystemM08 - Construct");

            //
#if WALKIO_FLOWCONTROL
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - FlowField - SystemM08 - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            _gridWorldEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<GameLevel.GridWorld>(),
                    ComponentType.ReadOnly<GameLevel.GridWorldProperty>()
                }
            });

            RequireForUpdate(_gridWorldEntityQuery);
        }

                private void ForEachChaseTarget(int2 gridCellCount, float2 gridCellSize, int2 tileCellCount, float2 tileCellSize,
            DynamicBuffer<LeadingToTileBuffer> leadingToTileBuffers)
        {
            Entities
                .WithAll<ChaseTarget>()
                .ForEach((Entity chaseTargetEntity, LocalToWorld localToWorld, ref ChaseTargetProperty chaseTargetProperty,
                    ref GameMove.MoveByForce moveByForce) =>
                {
                    var tileAndTileCellIndex =
                        Utility.FlowFieldTileHelper.PositionToTileAndTileCellIndexAtGridTo2DIndex(
                            gridCellCount, gridCellSize,
                            tileCellCount, tileCellSize,
                            localToWorld.Position.xz);

                    var shouldUseTileEntity = Entity.Null;

                    for (var i = 0; i < leadingToTileBuffers.Length; ++i)
                    {
                        var inSetTileIndex = leadingToTileBuffers[i].Value.TileIndex;

                        if (inSetTileIndex.x == tileAndTileCellIndex.x &&
                            inSetTileIndex.y == tileAndTileCellIndex.y)
                        {
                            // matched
                            shouldUseTileEntity = leadingToTileBuffers[i].Value.Tile;
                            break;
                        }
                    }

                    if (shouldUseTileEntity != Entity.Null)
                    {
                        // Has the tile entity to use
                        var flowFieldTileCellBuffer = GetBufferFromEntity<FlowFieldTileCellBuffer>();
                        var flowFieldTileCells = flowFieldTileCellBuffer[shouldUseTileEntity];

                        var tileCellIndex = tileAndTileCellIndex.w * tileCellCount.x + tileAndTileCellIndex.z;

                        var direction = flowFieldTileCells[tileCellIndex].Value.Direction;
                        moveByForce.Direction =
                            Utility.FlowFieldTileHelper.FromIntDirectionToNormalizedVector(direction);
                        moveByForce.Force = 0.80f;
                    }
                })
                .WithoutBurst()
                .Run();
        }

        protected override void OnUpdate()
        {
            // if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var gridWorldProperty = _gridWorldEntityQuery.GetSingleton<GameLevel.GridWorldProperty>();

            var gridCellCount = gridWorldProperty.CellCount;
            var gridCellSize = gridWorldProperty.CellSize;
            var flowFieldWorldData = FlowFieldWorldProvider.FlowFieldWorldData as Template.FlowFieldWorldData;
            var tileCellCount = new int2(flowFieldWorldData.tileCellCount.x, flowFieldWorldData.tileCellCount.y);
            var tileCellSize = (float2)flowFieldWorldData.tileCellSize;

            var toBeDeletedLeadingToSetEventEntityArchetype = EntityManager.CreateArchetype(
                typeof(ToBeDeletedLeadingToSet),
                typeof(ToBeDeletedLeadingToSetProperty));

            Entities
                .WithAll<LeadingToSet>()
                .WithNone<ToBeDeletedLeadingToSet>()
                .ForEach((Entity leadingToSet, DynamicBuffer<LeadingToTileBuffer> leadingToTileBuffers) =>
                {
                    ForEachChaseTarget(gridCellCount, gridCellSize, tileCellCount, tileCellSize, leadingToTileBuffers);
                })
                .WithoutBurst()
                .Run();

            // Entities
            //     .WithAll<ChaseTarget>()
            //     .ForEach((Entity entity, LocalToWorld localToWorld, ref ChaseTargetProperty chaseTargetProperty, ref GameMove.MoveByForce moveByForce) =>
            //     // .ForEach((Entity entity, int entityInQueryIndex, LeadingToSetProperty leadingToSetProperty,
            //     //     DynamicBuffer<LeadingToTileBuffer> leadingToTileBuffers) =>
            //     {
            //         if (chaseTargetProperty.LeadingToSet != Entity.Null)
            //         {
            //             var leadingToTileBuffer = GetBufferFromEntity<LeadingToTileBuffer>();
            //
            //             var leadingToSetTiles = leadingToTileBuffer[chaseTargetProperty.LeadingToSet];
            //
            //             var tileAndTileCellIndex =
            //                 Utility.FlowFieldTileHelper.PositionToTileAndTileCellIndexAtGridTo2DIndex(
            //                     gridCellCount, gridCellSize,
            //                     tileCellCount, tileCellSize,
            //                     localToWorld.Position.xz);
            //
            //             var shouldUseTileEntity = Entity.Null;
            //
            //             for (var i = 0; i < leadingToSetTiles.Length; ++i)
            //             {
            //                 var inSetTileIndex = leadingToSetTiles[i].Value.TileIndex;
            //
            //                 if (inSetTileIndex.x == tileAndTileCellIndex.x &&
            //                     inSetTileIndex.y == tileAndTileCellIndex.y)
            //                 {
            //                     // matched
            //                     shouldUseTileEntity = leadingToSetTiles[i].Value.Tile;
            //                     break;
            //                 }
            //             }
            //
            //             if (shouldUseTileEntity != Entity.Null)
            //             {
            //                 // Has the tile entity to use
            //                 var flowFieldTileCellBuffer = GetBufferFromEntity<FlowFieldTileCellBuffer>();
            //                 var flowFieldTileCells = flowFieldTileCellBuffer[shouldUseTileEntity];
            //
            //                 var tileCellIndex = tileAndTileCellIndex.w * tileCellCount.x + tileAndTileCellIndex.z;
            //
            //                 var direction = flowFieldTileCells[tileCellIndex].Value.Direction;
            //                 moveByForce.Direction =
            //                     Utility.FlowFieldTileHelper.FromIntDirectionToNormalizedVector(direction);
            //             }
            //         }
            //
            //         // var tileEntity = chaseTargetProperty.AtFlowFieldTile;
            //         // if (tileEntity != Entity.Null)
            //         // {
            //         //     var lookupBuffer = GetBufferFromEntity<FlowFieldTileCellBuffer>();
            //         //
            //         //     var tileAndTileCellIndex =
            //         //         Utility.FlowFieldTileHelper.PositionToTileAndTileCellIndexAtGridTo2DIndex(
            //         //             gridCellCount, gridCellSize,
            //         //             tileCellCount, tileCellSize,
            //         //             localToWorld.Position.xz);
            //         //
            //         //
            //         //
            //         //     var flowFieldTileCellBuffers = lookupBuffer[tileEntity];
            //         //     var cellIndex = chaseTargetProperty.AtTileCellIndex.y * tileCellCount.x + chaseTargetProperty.AtTileCellIndex.x;
            //         //     var cellValue = flowFieldTileCellBuffers[cellIndex];
            //         //
            //         //     //
            //         //     moveByForce.Direction =
            //         //         Utility.FlowFieldTileHelper.FromIntDirectionToNormalizedVector(cellValue.Value.Direction);
            //         // }
            //     })
            //     .WithoutBurst()
            //     .Run();
            //     // .WithBurst()
            //     // .Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
