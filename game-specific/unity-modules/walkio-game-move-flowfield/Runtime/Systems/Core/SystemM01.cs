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
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
    using GameLevel = JoyBrick.Walkio.Game.Level;

    [DisableAutoCreation]
    // [UpdateAfter(typeof(SystemA))]
    public class SystemM01 : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemM01));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _gridWorldEntityQuery;

        //
        private bool _canUpdate;

        public GameFlowControl.IFlowControl FlowControl { get; set; }

        //
        public IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

        public void Construct()
        {
            _logger.Debug($"Module - Move - FlowField - SystemM01 - Construct");

            //
#if WALKIO_FLOWCONTROL
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - FlowField - SystemM01 - Construct - Receive FlowReadyToStart");
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
        private void UpdateAtFlowFieldTile(
            int totalTileCellCount,
            NativeArray<int> gridCellIndices,
            NativeArray<int> costs,
            NativeArray<int> directions,
            Entity flowFieldTileEntity)
        {
            Entities
                .WithAll<FlowFieldTile>()
                .ForEach((Entity entity, ref DynamicBuffer<FlowFieldTileCellBuffer> flowFieldTileCellBuffers) =>
                {
                    if (entity == flowFieldTileEntity)
                    {
                        // _logger.Debug($"Module - SystemM02_2 - UpdateForFlowFieldTile - find matched flow field tile: {entity}");

                        // flowFieldTileCellBuffers.ResizeUninitialized(totalTileCellCount);

                        for (var m = 0; m < totalTileCellCount; ++m)
                        {
                            // Assign grid cell index into tile cell for now
                            // Can also assign the content of that grid cell into tile cell
                            // tileCellBuffer[i] = gridCellIndices[i];
                            flowFieldTileCellBuffers[m] = new TileCellContent
                            {
                                CellIndex = gridCellIndices[m],
                                BaseCost = costs[m],
                                Direction = directions[m]
                            };
                        }
                    }
                })
                .WithoutBurst()
                .Run();
        }

        public void UpdateAtFlowFieldTileEntity(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int2 tileIndex, int2 tileCellIndex,
            GameLevel.GridWorldProperty gridWorldProperty,
            EntityCommandBuffer commandBuffer, Entity atTileEntity)
        {
            Entities
                .WithAll<FlowFieldTile>()
                .ForEach((Entity entity, FlowFieldTileProperty flowFieldTileProperty, ref DynamicBuffer<FlowFieldTileCellBuffer> flowFieldTileCellBuffers) =>
                {
                    if (entity == atTileEntity)
                    {
                        // _logger.Debug($"Module - Move - FlowField - SystemM01 - OnUpdate - at tile entity: {atTileEntity} matched");

                        var gridCellIndicesInUpdatedTile =
                            Utility.FlowFieldTileHelper.GetGridCellIndicesInTile(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                tileIndex);

                        var totalTileCellCount = tileCellCount.x * tileCellCount.y;
                        var baseCosts = new NativeArray<int>(totalTileCellCount, Allocator.Temp);

                        for (var inTileGridCellIndex = 0; inTileGridCellIndex < baseCosts.Length; ++inTileGridCellIndex)
                        {
                            var gridCellIndex = gridCellIndicesInUpdatedTile[inTileGridCellIndex];

                            // In boundary case
                            if (gridCellIndex != -1)
                            {
                                // Assigning a default value first
                                var gridMapContext = gridWorldProperty.GridMapBlobAssetRef.Value.GridMapContextArray[gridCellIndex];
                                // Should read from lookup table(data)
                                if (gridMapContext == 1)
                                {
                                    baseCosts[inTileGridCellIndex] = 50000;
                                }
                            }
                            else
                            {
                                // Out boundary
                                // _logger.Debug($"Module - Move - FlowField - SystemM01 - OnUpdate - gridCellIndex -1 at tileIndex: {updatedTileIndex} for inTileGridCellIndex: {inTileGridCellIndex}");
                                baseCosts[inTileGridCellIndex] = 50000;
                            }
                        }

                        var costs =
                            Utility.FlowFieldTileHelper.GetIntegrationCostForTile(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                tileCellIndex,
                                baseCosts);

                        var neighborDirection = 4;
                        var directions =
                            Utility.FlowFieldTileHelper.GetDirectionForTile(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                tileCellIndex,
                                neighborDirection,
                                baseCosts);

                        // flowFieldTileCellBuffers.ResizeUninitialized(totalTileCellCount);
                        for (var inTileTileCellIndex = 0; inTileTileCellIndex < totalTileCellCount; ++inTileTileCellIndex)
                        {
                            flowFieldTileCellBuffers[inTileTileCellIndex] = new TileCellContent
                            {
                                CellIndex = gridCellIndicesInUpdatedTile[inTileTileCellIndex],
                                BaseCost = costs[inTileTileCellIndex],
                                Direction = directions[inTileTileCellIndex]
                            };
                        }
                    }
                })
                .WithoutBurst()
                .Run();
        }


        private void GetAssignedFlowFieldTileEntity(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            int worldId, int groupId,
            int2 tileIndex, int2 tileCellIndex,
            GameLevel.GridWorldProperty gridWorldProperty,
            EntityCommandBuffer commandBuffer, Entity flowFieldTileEntity)
        {
            var gridCellIndicesInUpdatedTile =
                Utility.FlowFieldTileHelper.GetGridCellIndicesInTile(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    tileIndex);

            var totalTileCellCount = tileCellCount.x * tileCellCount.y;
            var baseCosts = new NativeArray<int>(totalTileCellCount, Allocator.Temp);

            for (var inTileGridCellIndex = 0; inTileGridCellIndex < baseCosts.Length; ++inTileGridCellIndex)
            {
                var gridCellIndex = gridCellIndicesInUpdatedTile[inTileGridCellIndex];

                // In boundary case
                if (gridCellIndex != -1)
                {
                    // Assigning a default value first
                    var gridMapContext = gridWorldProperty.GridMapBlobAssetRef.Value.GridMapContextArray[gridCellIndex];
                    // Should read from lookup table(data)
                    if (gridMapContext == 1)
                    {
                        baseCosts[inTileGridCellIndex] = 50000;
                    }
                }
                else
                {
                    // Out boundary
                    // _logger.Debug($"Module - Move - FlowField - SystemM01 - OnUpdate - gridCellIndex -1 at tileIndex: {updatedTileIndex} for inTileGridCellIndex: {inTileGridCellIndex}");
                    baseCosts[inTileGridCellIndex] = 50000;
                }
            }

            var costs =
                Utility.FlowFieldTileHelper.GetIntegrationCostForTile(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    tileCellIndex,
                    baseCosts);

            var neighborDirection = 4;
            var directions =
                Utility.FlowFieldTileHelper.GetDirectionForTile(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    tileCellIndex,
                    neighborDirection,
                    baseCosts);

            // var flowFieldTileEntity = commandBuffer.CreateEntity(flowFieldTileEntityArchetype);

            commandBuffer.SetComponent(flowFieldTileEntity, new FlowFieldTileProperty
            {
                WorldId = worldId,
                GroupId = groupId,
                TileIndex = tileIndex
            });

            var tileCellBuffer = commandBuffer.AddBuffer<FlowFieldTileCellBuffer>(flowFieldTileEntity);
            tileCellBuffer.ResizeUninitialized(totalTileCellCount);

            for (var inTileTileCellIndex = 0; inTileTileCellIndex < totalTileCellCount; ++inTileTileCellIndex)
            {
                tileCellBuffer[inTileTileCellIndex] = new TileCellContent
                {
                    CellIndex = gridCellIndicesInUpdatedTile[inTileTileCellIndex],
                    BaseCost = costs[inTileTileCellIndex],
                    Direction = directions[inTileTileCellIndex]
                };
            }
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            //
            var flowFieldTileEntityArchetype = EntityManager.CreateArchetype(
                typeof(FlowFieldTile),
                typeof(FlowFieldTileProperty),
                typeof(FlowFieldTileCellBuffer),
                typeof(GameFlowControl.StageUse));

            var atTileChangeEventEntityArchetype = EntityManager.CreateArchetype(
                typeof(AtTileChange),
                typeof(AtTileChangeProperty));

            var atTileCellChangeEventEntityArchetype = EntityManager.CreateArchetype(
                typeof(AtTileCellChange),
                typeof(AtTileCellChangeProperty));

            //
            var gridWorldProperty = _gridWorldEntityQuery.GetSingleton<GameLevel.GridWorldProperty>();

            var gridCellCount = gridWorldProperty.CellCount;
            var gridCellSize = gridWorldProperty.CellSize;
            var flowFieldWorldData = FlowFieldWorldProvider.FlowFieldWorldData as Template.FlowFieldWorldData;
            var tileCellCount = new int2(flowFieldWorldData.tileCellCount.x, flowFieldWorldData.tileCellCount.y);
            var tileCellSize = (float2)flowFieldWorldData.tileCellSize;

            //
            Entities
                .WithAll<ToBeChasedTarget>()
                .ForEach((Entity entity, LocalToWorld localToWorld,
                    ref ToBeChasedTargetProperty toBeChasedTargetProperty) =>
                {
                    var groupId = toBeChasedTargetProperty.BelongToGroup;

                    var updatedTileAndTileCellIndex =
                        Utility.FlowFieldTileHelper.PositionToTileAndTileCellIndexAtGridTo2DIndex(
                            gridCellCount, gridCellSize,
                            tileCellCount, tileCellSize,
                            new float2(localToWorld.Position.x, localToWorld.Position.z));

                    var originalTileIndex = toBeChasedTargetProperty.AtTileIndex;
                    var originalTileCellIndex = toBeChasedTargetProperty.AtTileCellIndex;
                    var updatedTileIndex = updatedTileAndTileCellIndex.xy;
                    var updatedTileCellIndex = updatedTileAndTileCellIndex.zw;

                    //
                    var assigningInitialTile = (toBeChasedTargetProperty.AtTileEntity == Entity.Null);

                    var atOriginalTile = (updatedTileIndex.x == originalTileIndex.x) && (updatedTileIndex.y == originalTileIndex.y);
                    var atOriginalTileCell = (updatedTileCellIndex.x == originalTileCellIndex.x) && (updatedTileCellIndex.y == originalTileCellIndex.y);

                    if (assigningInitialTile || !atOriginalTile)
                    {
                        // _logger.Debug($"Module - Move - FlowField - SystemM01 - OnUpdate - to be chased: [{groupId}] need to issue tile change event, not at original tile: {originalTileIndex}, but at: {updatedTileIndex}");

                        if (!assigningInitialTile)
                        {
                            // Discard the previous one by adding creating an event
                            commandBuffer.AddComponent(toBeChasedTargetProperty.AtTileEntity, new ToBeDeletedFlowFieldTile());
                        }

                        // Create a new at-tile entity
                        var flowFieldTileEntity = commandBuffer.CreateEntity(flowFieldTileEntityArchetype);
                        GetAssignedFlowFieldTileEntity(
                            gridCellCount, gridCellSize,
                            tileCellCount, tileCellSize,
                            0, groupId,
                            updatedTileIndex, updatedTileCellIndex,
                            gridWorldProperty,
                            commandBuffer, flowFieldTileEntity);

                        // _logger.Debug($"Module - Move - FlowField - SystemM01 - OnUpdate - to be chased: [{groupId}] created entity: {flowFieldTileEntity}");

                        // toBeChasedTargetProperty.AtTileEntity = flowFieldTileEntity;
                        commandBuffer.SetComponent(entity, new ToBeChasedTargetProperty
                        {
                            BelongToGroup = toBeChasedTargetProperty.BelongToGroup,
                            AtTileIndex = updatedTileIndex,
                            AtTileCellIndex = updatedTileCellIndex,
                            AtTileEntity = flowFieldTileEntity,
                            LeadingToSetEntity = toBeChasedTargetProperty.LeadingToSetEntity
                        });

                        var atTileChangeEventEntity = commandBuffer.CreateEntity(atTileChangeEventEntityArchetype);

                        // commandBuffer.SetComponent(atTileChangeEventEntity, new AtTileChangeProperty
                        commandBuffer.SetComponent(atTileChangeEventEntity,
                            new AtTileChangeProperty
                            {
                                GroupId = toBeChasedTargetProperty.BelongToGroup,
                                ChangeToPosition = localToWorld.Position,
                                ChangeToTileIndex = updatedTileIndex,
                                ChangeToTileCellIndex = updatedTileCellIndex,

                                AtTileEntity = flowFieldTileEntity
                            });
                    }
                    else if (!atOriginalTileCell)
                    {
                        // Change the integration for the entity
                        var atTileEntity = toBeChasedTargetProperty.AtTileEntity;

                        // _logger.Debug($"Module - Move - FlowField - SystemM01 - OnUpdate - to be chased: [{groupId}] need to issue tile cell change event, at entity: {atTileEntity} not at original tile cell: {originalTileCellIndex}, but at: {updatedTileCellIndex}");

                        UpdateAtFlowFieldTileEntity(
                            gridCellCount, gridCellSize,
                            tileCellCount, tileCellSize,
                            updatedTileIndex, updatedTileCellIndex,
                            gridWorldProperty,
                            commandBuffer,
                            atTileEntity);

                        //
                        toBeChasedTargetProperty.AtTileIndex = updatedTileIndex;
                        toBeChasedTargetProperty.AtTileCellIndex = updatedTileCellIndex;
                    }
                })
                .WithoutBurst()
                .Run();

            //
            // Entities
            //     .WithAll<ToBeChasedTarget>()
            //     // .ForEach((Entity entity, LocalToWorld localToWorld, ref ToBeChasedTargetProperty toBeChasedTargetProperty) =>
            //     .ForEach((Entity entity, int entityInQueryIndex, LocalToWorld localToWorld,
            //         ref ToBeChasedTargetProperty toBeChasedTargetProperty) =>
            //     {
            //         //
            //         var initialized = toBeChasedTargetProperty.Initialized;
            //         // var initialized = (toBeChasedTargetProperty.LeadingToSetEntity != Entity.Null);
            //         var belongToGroup = toBeChasedTargetProperty.BelongToGroup;
            //
            //         //
            //         var updatedTileAndTileCellIndex =
            //             Utility.FlowFieldTileHelper.PositionToTileAndTileCellIndexAtGridTo2DIndex(
            //                 gridCellCount, gridCellSize,
            //                 tileCellCount, tileCellSize,
            //                 new float2(localToWorld.Position.x, localToWorld.Position.z));
            //
            //         var originalTileIndex = toBeChasedTargetProperty.AtTileIndex;
            //         var originalTileCellIndex = toBeChasedTargetProperty.AtTileCellIndex;
            //         var updatedTileIndex = updatedTileAndTileCellIndex.xy;
            //         var updatedTileCellIndex = updatedTileAndTileCellIndex.zw;
            //
            //         // Compare before assigning, all index compare is int value, which will require no additional checking
            //         var atOriginalTile = (updatedTileIndex.x == originalTileIndex.x) && (updatedTileIndex.y == originalTileIndex.y);
            //         var atOriginalTileCell = (updatedTileCellIndex.x == originalTileCellIndex.x) && (updatedTileCellIndex.y == originalTileCellIndex.y);
            //
            //         //
            //         toBeChasedTargetProperty.AtTileIndex = updatedTileIndex;
            //         toBeChasedTargetProperty.AtTileCellIndex = updatedTileCellIndex;
            //
            //         //
            //         var issueTileChangeEvent = (!initialized || !atOriginalTile);
            //         if (issueTileChangeEvent)
            //         {
            //             _logger.Debug($"Module - Move - FlowField - SystemM01 - OnUpdate - to be chased: [{belongToGroup}] need to issue tile change event, not at original tile: {originalTileIndex}, but at: {updatedTileIndex}");
            //             // This is event entity. It notifies target at tile is changed
            //             // var atTileChangeEventEntity = commandBuffer.CreateEntity(atTileChangeEventEntityArchetype);
            //             var atTileChangeEventEntity = concurrentCommandBuffer.CreateEntity(entityInQueryIndex, atTileChangeEventEntityArchetype);
            //
            //             // commandBuffer.SetComponent(atTileChangeEventEntity, new AtTileChangeProperty
            //             concurrentCommandBuffer.SetComponent(entityInQueryIndex, atTileChangeEventEntity,
            //                 new AtTileChangeProperty
            //                 {
            //                     GroupId = toBeChasedTargetProperty.BelongToGroup,
            //                     ChangeToPosition = localToWorld.Position,
            //                     ChangeToTileIndex = updatedTileIndex,
            //
            //                     ForWhichLeader = entity
            //                 });
            //
            //             toBeChasedTargetProperty.Initialized = true;
            //         }
            //         else
            //         {
            //             // Tile is the same, but tile cell might be different
            //             var issueTileCellChangeEvent = !atOriginalTileCell;
            //             if (issueTileCellChangeEvent)
            //             {
            //                 _logger.Debug($"Module - Move - FlowField - SystemM01 - OnUpdate - to be chased: [{belongToGroup}] need to issue tile cell change event, not at original tile cell: {originalTileCellIndex}, but at: {updatedTileCellIndex}");
            //
            //                 var atTileCellChangeEventEntity =
            //                     // commandBuffer.CreateEntity(atTileCellChangeEventEntityArchetype);
            //                     concurrentCommandBuffer.CreateEntity(entityInQueryIndex, atTileCellChangeEventEntityArchetype);
            //
            //                 // commandBuffer.SetComponent(atTileCellChangeEventEntity, new AtTileCellChangeProperty
            //                 concurrentCommandBuffer.SetComponent(entityInQueryIndex, atTileCellChangeEventEntity,
            //                     new AtTileCellChangeProperty
            //                     {
            //                         GroupId = toBeChasedTargetProperty.BelongToGroup,
            //                         ChangeToPosition = localToWorld.Position,
            //                         ChangeToTileIndex = updatedTileIndex,
            //                         ChangeToTileCellIndex = updatedTileCellIndex,
            //                         // LeadingToSetEntity should still be valid
            //                         LeadingToSetEntity = toBeChasedTargetProperty.LeadingToSetEntity
            //                     });
            //
            //                 toBeChasedTargetProperty.AtTileCellIndex = updatedTileCellIndex;
            //             }
            //         }
            //     })
            //     // .WithoutBurst()
            //     // .Run();
            //     .WithBurst()
            //     .Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
