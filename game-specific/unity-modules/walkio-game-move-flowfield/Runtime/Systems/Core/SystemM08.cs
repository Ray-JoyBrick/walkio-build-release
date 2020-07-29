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
    // [UpdateAfter(typeof(SystemA))]
    public class SystemM08 : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemM08));

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

        private void AssignToMatchedToBeChasedTarget(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            EntityCommandBuffer commandBuffer,
            Entity leadingToSetEntity,
            LeadingToSetProperty leadingToSetProperty,
            DynamicBuffer<LeadingToTileBuffer> leadingToTileBuffers,
            EntityArchetype toBeDeletedLeadingToSetEventEntityArchetype)
        {
            Entities
                .WithAll<ToBeChasedTarget>()
                .ForEach((Entity entity, ref ToBeChasedTargetProperty toBeChasedTargetProperty) =>
                // .ForEach((Entity entity, int entityInQueryIndex, ref ToBeChasedTargetProperty toBeChasedTargetProperty) =>
                {
                    if (toBeChasedTargetProperty.BelongToGroup == leadingToSetProperty.GroupId)
                    {
                        if (toBeChasedTargetProperty.LeadingToSetEntity != Entity.Null)
                        {
                            commandBuffer.AddComponent<ToBeDeletedLeadingToSet>(toBeChasedTargetProperty.LeadingToSetEntity);
                            commandBuffer.AddComponent(toBeChasedTargetProperty.LeadingToSetEntity, new ToBeDeletedLeadingToSetProperty
                            {
                                IntervalMax = 3,
                                CountDown = 0
                            });
                        }

                        toBeChasedTargetProperty.LeadingToSetEntity = leadingToSetEntity;
                    }
                })
                .WithoutBurst()
                .Run();
                // .WithBurst()
                // .Schedule();
        }

        private void AssignToMatchedChaseTarget(
            int2 gridCellCount, float2 gridCellSize,
            int2 tileCellCount, float2 tileCellSize,
            // EntityCommandBuffer commandBuffer,
            Entity leadingToSetEntity,
            LeadingToSetProperty leadingToSetProperty,
            DynamicBuffer<LeadingToTileBuffer> leadingToTileBuffers)
        {
            Entities
                .WithAll<ChaseTarget>()
                .ForEach((Entity entity, LocalToWorld localToWorld, ref ChaseTargetProperty chaseTargetProperty) =>
                // .ForEach((Entity entity, int entityInQueryIndex, LocalToWorld localToWorld,
                //     ref ChaseTargetProperty chaseTargetProperty) =>
                {
                    if (chaseTargetProperty.BelongToGroup == leadingToSetProperty.GroupId)
                    {
                        var atTileAndTileCellIndex =
                            Utility.FlowFieldTileHelper.PositionToTileAndTileCellIndexAtGridTo2DIndex(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                new float2(localToWorld.Position.x, localToWorld.Position.z));

                        #region Find matched tile if any, assign and update chaseTargetProperty then break the loop

                        for (var i = 0; i < leadingToTileBuffers.Length; ++i)
                        {
                            var tileIndex = leadingToTileBuffers[i].Value.TileIndex;
                            if (tileIndex.x == atTileAndTileCellIndex.x && tileIndex.y == atTileAndTileCellIndex.y)
                            {
                                // Assign to tile of latest leading-to-set
                                chaseTargetProperty.AtFlowFieldTile = leadingToTileBuffers[i].Value.Tile;

                                chaseTargetProperty.AtTileIndex = atTileAndTileCellIndex.xy;
                                chaseTargetProperty.AtTileCellIndex = atTileAndTileCellIndex.zw;

                                //
                                chaseTargetProperty.LeadingToSet = entity;

                                break;
                            }
                        }

                        #endregion
                    }
                })
                .WithoutBurst()
                .Run();
                // .WithBurst()
                // .Schedule();

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
                .WithAll<LeadingToSetCreated>()
                .ForEach((Entity entity, LeadingToSetProperty leadingToSetProperty,
                    DynamicBuffer<LeadingToTileBuffer> leadingToTileBuffers) =>
                // .ForEach((Entity entity, int entityInQueryIndex, LeadingToSetProperty leadingToSetProperty,
                //     DynamicBuffer<LeadingToTileBuffer> leadingToTileBuffers) =>
                {
                    AssignToMatchedChaseTarget(
                        gridCellCount, gridCellSize, tileCellCount, tileCellSize,
                        // commandBuffer,
                        entity,
                        leadingToSetProperty, leadingToTileBuffers);

                    AssignToMatchedToBeChasedTarget(
                        gridCellCount, gridCellSize, tileCellCount, tileCellSize,
                        commandBuffer,
                        entity,
                        leadingToSetProperty, leadingToTileBuffers,
                        toBeDeletedLeadingToSetEventEntityArchetype);

                    commandBuffer.RemoveComponent<LeadingToSetCreated>(entity);
                })
                .WithoutBurst()
                .Run();
                // .WithBurst()
                // .Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
