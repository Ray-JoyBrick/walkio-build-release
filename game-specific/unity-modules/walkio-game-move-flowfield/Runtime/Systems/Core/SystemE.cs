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
    [UpdateAfter(typeof(CheckTargetAtTileChangeSystem))]
    public class SystemE : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemD));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _gridWorldEntityQuery;

        //
        private bool _canUpdate;
        private EntityQuery _leadingToSetPlaceholderEntityQuery;

        //
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif
        public IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

        public void Construct()
        {
            _logger.Debug($"Module - SystemE - Construct");

#if WALKIO_FLOWCONTROL
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - FlowField - SystemE - Construct - Receive AllDoneSettingAsset");
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

            _leadingToSetPlaceholderEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<LeadingToSetPlaceholder>(),
                    ComponentType.ReadOnly<LeadingToSetPlaceholderTileBuffer>()
                }
            });

            RequireForUpdate(_gridWorldEntityQuery);
            RequireForUpdate(_leadingToSetPlaceholderEntityQuery);
        }

        protected override void OnUpdate()
        {
            // if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            var gridWorldProperty = _gridWorldEntityQuery.GetSingleton<GameLevel.GridWorldProperty>();

            var gridCellCount = gridWorldProperty.CellCount;
            var gridCellSize = gridWorldProperty.CellSize;
            var flowFieldWorldData = FlowFieldWorldProvider.FlowFieldWorldData as Template.FlowFieldWorldData;
            var tileCellCount = new int2(flowFieldWorldData.tileCellCount.x, flowFieldWorldData.tileCellCount.y);
            var tileCellSize = (float2)flowFieldWorldData.tileCellSize;

            var flowFieldTileEntityArchetype = EntityManager.CreateArchetype(
                typeof(FlowFieldTile),
                typeof(FlowFieldTileProperty),
                typeof(FlowFieldTileCellBuffer),
                typeof(ChaseTargetAtTileBuffer));

            // _leadingToSetPlaceholderEntityQuery.ToComponentArray<DynamicBuffer<LeadingToSetPlaceholderTileBuffer>>()

            Entities
                .WithAll<LeadingToSetPlaceholder>()
                .ForEach((Entity entity, LeadingToSetPlaceholderProperty leadingToSetPlaceholderProperty, DynamicBuffer<LeadingToSetPlaceholderTileBuffer> leadingToSetPlaceholderTileBuffer) =>
                {
                    // _logger.Debug($"Module - SystemE - OnUpdate - event entity: {entity}");

                    // GetBufferFromEntity<LeadingToSetPlaceholderTileBuffer>();

                    // commandBuffer.

                    var positions = new List<float3>();
                    for (var i = 0; i < leadingToSetPlaceholderTileBuffer.Length; ++i)
                    {
                        var fft = EntityManager.GetComponentData<FlowFieldTileProperty>(leadingToSetPlaceholderTileBuffer[i].Value);

                        var tileIndex = fft.TileIndex;
                        var positionXZ =
                            Utility.FlowFieldTileHelper.TileIndexToPosition2D(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                tileIndex);

                        var position = new float3(positionXZ.x, 0, positionXZ.y);

                        // _logger.Debug($"Module - SystemA - UpdateEachChaseTarget - tile center position {i}: {position}");

                        positions.Add(position);
                    }

                    // FlowFieldWorldProvider?.CalculateLeadingTilePath(leadingToSetPlaceholderProperty.GroupId, leadingToSetPlaceholderProperty.ChangeToPosition, positions);

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
