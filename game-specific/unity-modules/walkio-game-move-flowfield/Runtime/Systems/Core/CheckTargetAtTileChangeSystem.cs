namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;

    [DisableAutoCreation]
    [UpdateAfter(typeof(SetupInitialLeadingToSetSystem))]
    public class CheckTargetAtTileChangeSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(CheckTargetAtTileChangeSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        //
        public IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

        public void Construct()
        {
            _logger.Debug($"Module - CheckTargetAtTileChangeSystem - Construct");
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var atTileChangeEventEntityArchetype = EntityManager.CreateArchetype(
                typeof(AtTileChange),
                typeof(AtTileChangeProperty));

            int2 gridCellCount = new int2(256, 192);
            float2 gridCellSize = new float2(1.0f, 1.0f);
            var flowFieldWorldData = FlowFieldWorldProvider.FlowFieldWorldData as Template.FlowFieldWorldData;
            var tileCellCount = new int2(flowFieldWorldData.tileCellCount.x, flowFieldWorldData.tileCellCount.y);
            float2 tileCellSize = (float2)flowFieldWorldData.tileCellSize;

            Entities
                .WithAll<ToBeChasedTarget>()
                .ForEach((Entity entity, LocalToWorld localToWorld, ref ToBeChasedTargetProperty toBeChasedTargetProperty) =>
                {
                    if (toBeChasedTargetProperty.Initialized)
                    {
                        var updatedTileIndex =
                            Utility.FlowFieldTileHelper.PositionToTileIndexAtGrid2D(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                new float2(localToWorld.Position.x, localToWorld.Position.z));

                        var originalTile = toBeChasedTargetProperty.AtTileIndex;
                        var atOriginalTile =
                            (updatedTileIndex.x == toBeChasedTargetProperty.AtTileIndex.x)
                            && (updatedTileIndex.y == toBeChasedTargetProperty.AtTileIndex.y);

                        // Update first
                        // commandBuffer.SetComponent(entity, new ToBeChasedTargetProperty
                        // {
                        //     BelongToGroup = toBeChasedTargetProperty.BelongToGroup,
                        //     AtTileIndex = updatedTileIndex,
                        //     LeadingToSetEntity = toBeChasedTargetProperty.LeadingToSetEntity
                        // });

                        toBeChasedTargetProperty.AtTileIndex = updatedTileIndex;

                        if (!atOriginalTile)
                        {
                            _logger.Debug($"Module - CheckTargetAtTileChangeSystem - OnUpdate - not at original tile: {originalTile}, but at: {updatedTileIndex}");
                            // This is event entity. It notifies target at tile is changed
                            var atTileChangeEventEntity = commandBuffer.CreateEntity(atTileChangeEventEntityArchetype);

                            commandBuffer.SetComponent(atTileChangeEventEntity, new AtTileChangeProperty
                            {
                                GroupId = toBeChasedTargetProperty.BelongToGroup,
                                ChangeToPosition = localToWorld.Position,
                                ChangeToTileIndex = updatedTileIndex
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
