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

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    using GameLevel = JoyBrick.Walkio.Game.Level;

    [DisableAutoCreation]
    [UpdateAfter(typeof(SystemA))]
    public class SystemC : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemC));

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
            _logger.Debug($"Module - Move - FlowField - SystemC - Construct");

            //
#if WALKIO_FLOWCONTROL
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - FlowField - SystemC - Construct - Receive AllDoneSettingAsset");
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

        // private void CreateLeadingToSetEntity()
        // {
        //     var leadingToSetEntityArchetype = EntityManager.CreateArchetype(
        //         typeof(LeadingToSet),
        //         typeof(LeadingToTileBuffer));

        //     var leadingToSetEntity = commandBuffer.CreateEntity(leadingToSetEntityArchetype);

        //     var leadingToTileBuffer = commandBuffer.AddBuffer<LeadingToTileBuffer>(leadingToSetEntity);


        // }

        protected override void OnUpdate()
        {
            // if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var atTileChangeEventEntityArchetype = EntityManager.CreateArchetype(
                typeof(AtTileChange),
                typeof(AtTileChangeProperty));

            var atTileCellChangeEventEntityArchetype = EntityManager.CreateArchetype(
                typeof(AtTileCellChange),
                typeof(AtTileCellChangeProperty));

            var gridWorldProperty = _gridWorldEntityQuery.GetSingleton<GameLevel.GridWorldProperty>();

            var gridCellCount = gridWorldProperty.CellCount;
            var gridCellSize = gridWorldProperty.CellSize;
            var flowFieldWorldData = FlowFieldWorldProvider.FlowFieldWorldData as Template.FlowFieldWorldData;
            var tileCellCount = new int2(flowFieldWorldData.tileCellCount.x, flowFieldWorldData.tileCellCount.y);
            var tileCellSize = (float2)flowFieldWorldData.tileCellSize;

            Entities
                .WithAll<ToBeChasedTarget>()
                .ForEach((Entity entity, LocalToWorld localToWorld, ref ToBeChasedTargetProperty toBeChasedTargetProperty) =>
                {
                    //
                    var initialized = toBeChasedTargetProperty.Initialized;
                    // var initialized = (toBeChasedTargetProperty.LeadingToSetEntity != Entity.Null);

                    //
                    var updatedTileAndTileCellIndex =
                        Utility.FlowFieldTileHelper.PositionToTileAndTileCellIndexAtGridTo2DIndex(
                            gridCellCount, gridCellSize,
                            tileCellCount, tileCellSize,
                            new float2(localToWorld.Position.x, localToWorld.Position.z));

                    var originalTileIndex = toBeChasedTargetProperty.AtTileIndex;
                    var originalTileCellIndex = toBeChasedTargetProperty.AtTileCellIndex;
                    var updatedTileIndex = new int2(updatedTileAndTileCellIndex.x, updatedTileAndTileCellIndex.y);
                    var updatedTileCellIndex = new int2(updatedTileAndTileCellIndex.z, updatedTileAndTileCellIndex.w);
                    var atOriginalTile =
                        (updatedTileIndex.x == originalTileIndex.x)
                        && (updatedTileIndex.y == originalTileIndex.y);
                    var atOriginalTileCell =
                        (updatedTileCellIndex.x == originalTileCellIndex.x)
                        && (updatedTileCellIndex.y == originalTileCellIndex.y);

                    toBeChasedTargetProperty.AtTileIndex = updatedTileIndex;
                    toBeChasedTargetProperty.AtTileCellIndex = updatedTileCellIndex;

                    var issueTileChangeEvent = (!initialized || !atOriginalTile);

                    if (issueTileChangeEvent)
                    {
                        _logger.Debug($"Module - Move - FlowField - SystemC - OnUpdate - need to issue tile change event, not at original tile: {originalTileIndex}, but at: {updatedTileIndex}");
                        // This is event entity. It notifies target at tile is changed
                        var atTileChangeEventEntity = commandBuffer.CreateEntity(atTileChangeEventEntityArchetype);

                        commandBuffer.SetComponent(atTileChangeEventEntity, new AtTileChangeProperty
                        {
                            GroupId = toBeChasedTargetProperty.BelongToGroup,
                            ChangeToPosition = localToWorld.Position,
                            ChangeToTileIndex = updatedTileIndex
                        });

                        toBeChasedTargetProperty.Initialized = true;
                    }
                    else
                    {
                        // Tile is the same, but tile cell might be different
                        var issueTileCellChangeEvent = !atOriginalTileCell;
                        if (issueTileCellChangeEvent)
                        {
                            _logger.Debug($"Module - Move - FlowField - SystemC - OnUpdate - need to issue tile cell change event, not at original tile cell: {originalTileCellIndex}, but at: {updatedTileCellIndex}");

                            var atTileCellChangeEventEntity = commandBuffer.CreateEntity(atTileCellChangeEventEntityArchetype);

                            commandBuffer.SetComponent(atTileCellChangeEventEntity, new AtTileCellChangeProperty
                            {
                                GroupId = toBeChasedTargetProperty.BelongToGroup,
                                ChangeToPosition = localToWorld.Position,
                                ChangeToTileIndex = updatedTileIndex,
                                // LeadingToSetEntity should still be valid
                                LeadingToSetEntity = toBeChasedTargetProperty.LeadingToSetEntity
                            });
                        }
                    }

                })
                .WithoutBurst()
                .Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
