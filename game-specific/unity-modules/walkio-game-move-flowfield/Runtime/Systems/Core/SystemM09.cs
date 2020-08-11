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
                    _logger.Debug($"Module - Move - FlowField - SystemM08 - Construct - Receive FlowReadyToStart");
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
                        // moveByForce.Force = 0.80f;
                        moveByForce.Force = 1.80f;
                    }
                })
                .WithoutBurst()
                .Run();
        }

        private float4 AssignFromTheSameTile(
            int2 tileCellCount,
            Entity atTileEntity,
            GameMove.MoveByForce moveByForce,
            int2 atTileCellIndex)
        {
            var directionWithForce = float4.zero;

            Entities
                .WithAll<FlowFieldTile>()
                .ForEach((Entity entity, DynamicBuffer<FlowFieldTileCellBuffer> flowFieldTileCellBuffers) =>
                {
                    if (atTileEntity == entity)
                    {
                        // _logger.Debug($"Module - Move - FlowField - SystemM09 - FindInLeadingToSet - matching at tile: {atTileEntity}");

                        var tileCellIndex = atTileCellIndex.y * tileCellCount.x + atTileCellIndex.x;
                        var direction = flowFieldTileCellBuffers[tileCellIndex].Value.Direction;
                        moveByForce.Direction =
                            Utility.FlowFieldTileHelper.FromIntDirectionToNormalizedVector(direction);
                        moveByForce.Force = 1.80f;

                        directionWithForce = new float4(moveByForce.Direction.xyz, moveByForce.Force);
                    }
                })
                .WithoutBurst()
                .Run();

            return directionWithForce;
        }

        private (float4, Entity) FindInLeadingToSet(
            int2 tileCellCount,
            Entity currentLeadingToSetEntity,
            GameMove.MoveByForce moveByForce,
            int2 atTileIndex,
            int2 atTileCellIndex)
        {
            var directionWithForce = float4.zero;
            var foundTileEntity = Entity.Null;

            Entities
                .WithAll<LeadingToSet>()
                .WithNone<ToBeDeletedLeadingToSet>()
                .ForEach((Entity entity, DynamicBuffer<LeadingToTileBuffer> leadingToTileBuffers) =>
                {
                    if (currentLeadingToSetEntity == entity)
                    {
                        // _logger.Debug($"Module - Move - FlowField - SystemM09 - FindInLeadingToSet - find leading entity: {currentLeadingToSetEntity}");

                        var foundTile = false;
                        for (var i = 0; i < leadingToTileBuffers.Length; ++i)
                        {
                            var matchingTileIndex =
                                (atTileIndex.x == leadingToTileBuffers[i].Value.TileIndex.x) &&
                                (atTileIndex.y == leadingToTileBuffers[i].Value.TileIndex.y);

                            if (matchingTileIndex)
                            {
                                foundTileEntity = leadingToTileBuffers[i].Value.Tile;
                                directionWithForce =
                                    AssignFromTheSameTile(tileCellCount, foundTileEntity, moveByForce,
                                        atTileCellIndex);

                                foundTile = true;
                                break;
                            }
                        }

                        if (!foundTile)
                        {
                            _logger.Debug($"Module - Move - FlowField - SystemM09 - FindInLeadingToSet - cant not find tile in leading set");
                        }
                    }
                })
                .WithoutBurst()
                .Run();

            return (directionWithForce, foundTileEntity);
        }

        protected override void OnUpdate()
        {
            // if (!_canUpdate) return;
            // if (true) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var gridWorldProperty = _gridWorldEntityQuery.GetSingleton<GameLevel.GridWorldProperty>();

            var gridCellCount = gridWorldProperty.CellCount;
            var gridCellSize = gridWorldProperty.CellSize;
            var flowFieldWorldData = FlowFieldWorldProvider.FlowFieldWorldData as Template.FlowFieldWorldData;
            var tileCellCount = new int2(flowFieldWorldData.tileCellCount.x, flowFieldWorldData.tileCellCount.y);
            var tileCellSize = (float2)flowFieldWorldData.tileCellSize;

            var defaultAssignedForce = flowFieldWorldData.defaultAssignedForce;

            var toBeDeletedLeadingToSetEventEntityArchetype = EntityManager.CreateArchetype(
                typeof(ToBeDeletedLeadingToSet),
                typeof(ToBeDeletedLeadingToSetProperty));

            Entities
                .WithAll<ChaseTarget>()
                .ForEach((Entity entity, LocalToWorld localToWorld, ref ChaseTargetProperty chaseTargetProperty, ref GameMove.MoveByForce moveByForce) =>
                {
                    var atTileAndTileCellIndex =
                        Utility.FlowFieldTileHelper.PositionToTileAndTileCellIndexAtGridTo2DIndex(
                            gridCellCount, gridCellSize,
                            tileCellCount, tileCellSize,
                            new float2(localToWorld.Position.x, localToWorld.Position.z));

                    var stillAtTheSameTile =
                        (atTileAndTileCellIndex.x == chaseTargetProperty.AtTileIndex.x)
                        && (atTileAndTileCellIndex.y == chaseTargetProperty.AtTileIndex.y);

                    var directionWithForce = float4.zero;

                    if (stillAtTheSameTile)
                    {
                        // _logger.Debug($"Module - Move - FlowField - SystemM09 - Update - {entity} still at tile: {atTileAndTileCellIndex.xy}");

                        // Just use the same tile, but update for cell
                        // chaseTargetProperty.AtFlowFieldTile
                        directionWithForce = AssignFromTheSameTile(tileCellCount, chaseTargetProperty.AtFlowFieldTile, moveByForce,
                            atTileAndTileCellIndex.zw);
                    }
                    else
                    {
                        // _logger.Debug($"Module - Move - FlowField - SystemM09 - Update - {entity} still at was at tile: {chaseTargetProperty.AtTileIndex} now at tile: {atTileAndTileCellIndex.xy}");
                        // If not at the same tile, find in leading-to-set
                        var pair = FindInLeadingToSet(tileCellCount, chaseTargetProperty.LeadingToSet, moveByForce,
                            atTileAndTileCellIndex.xy, atTileAndTileCellIndex.zw);
                        directionWithForce = pair.Item1;

                        chaseTargetProperty.AtFlowFieldTile = pair.Item2;
                    }
                    // _logger.Debug($"Module - Move - FlowField - SystemM09 - Update - returned: {directionWithForce}");

                    chaseTargetProperty.AtTileIndex = atTileAndTileCellIndex.xy;
                    chaseTargetProperty.AtTileCellIndex = atTileAndTileCellIndex.zw;

                    // if (directionWithForce.x == 0 && directionWithForce.y == 0 && directionWithForce.z == 0 &&
                    //     directionWithForce.w == 0)
                    if (directionWithForce.x == 0 && directionWithForce.y == 0 && directionWithForce.z == 0)
                    {
                        // _logger.Debug($"Module - Move - FlowField - SystemM09 - Update - {entity} direction is zero");
                        // commandBuffer.SetComponent(entity, new GameMove.MoveByForce
                        // {
                        //     Direction = new float3(1.0f, 0, 0),
                        //     Force = defaultAssignedForce
                        // });
                    }
                    else
                    {
                        commandBuffer.SetComponent(entity, new GameMove.MoveByForce
                        {
                            Direction = directionWithForce.xyz,
                            Force = defaultAssignedForce
                        });
                    }
                })
                .WithoutBurst()
                .Run();

            // Entities
            //     .WithAll<LeadingToSet>()
            //     .WithNone<ToBeDeletedLeadingToSet>()
            //     .ForEach((Entity leadingToSet, DynamicBuffer<LeadingToTileBuffer> leadingToTileBuffers) =>
            //     {
            //         ForEachChaseTarget(gridCellCount, gridCellSize, tileCellCount, tileCellSize, leadingToTileBuffers);
            //     })
            //     .WithoutBurst()
            //     .Run();

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
