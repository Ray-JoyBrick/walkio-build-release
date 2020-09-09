namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using JoyBrick.Walkio.Game.Move.FlowField;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;

    [DisableAutoCreation]
    public class SetupInitialLeadingToSetSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupInitialLeadingToSetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        //
        public IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

        public void Construct()
        {
            _logger.Debug($"Module - SetupInitialLeadingToSetSystem - Construct");
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            var leadingToSetEntityArchetype = EntityManager.CreateArchetype(
                typeof(LeadingToSet),
                typeof(LeadingToTileBuffer));

            int2 gridCellCount = new int2(256, 192);
            float2 gridCellSize = new float2(1.0f, 1.0f);
            var flowFieldWorldData = FlowFieldWorldProvider.FlowFieldWorldData as Template.FlowFieldWorldData;
            var tileCellCount = new int2(flowFieldWorldData.tileCellCount.x, flowFieldWorldData.tileCellCount.y);
            float2 tileCellSize = (float2)flowFieldWorldData.tileCellSize;

            Entities
                .WithAll<ToBeChasedTarget>()
                .ForEach((Entity entity, LocalToWorld localToWorld, ref ToBeChasedTargetProperty toBeChasedTargetProperty) =>
                {
                    var notAssigned = (toBeChasedTargetProperty.LeadingToSetEntity == Entity.Null);
                    if (notAssigned)
                    {
                        var leadingToSetEntity = commandBuffer.CreateEntity(leadingToSetEntityArchetype);

                        var leadingToTileBuffer = commandBuffer.AddBuffer<LeadingToTileBuffer>(leadingToSetEntity);
                        // var maxTileCount = 20;
                        // leadingToTileBuffer.ResizeUninitialized(maxTileCount);
                        //
                        // for (var tileIndex = 0; tileIndex < maxTileCount; ++tileIndex)
                        // {
                        //     leadingToTileBuffer[tileIndex] = Entity.Null;
                        // }

                        var atTileIndex =
                            Utility.FlowFieldTileHelper.PositionToTileIndexAtGrid2D(
                                gridCellCount, gridCellSize,
                                tileCellCount, tileCellSize,
                                new float2(localToWorld.Position.x, localToWorld.Position.z));

                        commandBuffer.SetComponent(entity, new ToBeChasedTargetProperty
                        {
                            BelongToGroup = toBeChasedTargetProperty.BelongToGroup,
                            Initialized = true,
                            AtTileIndex = atTileIndex,
                            LeadingToSetEntity = leadingToSetEntity
                        });

                        _logger.Debug($"Module - SetupInitialLeadingToSetSystem - OnUpdate - set for entity: {entity} atTileIndex: {atTileIndex}");

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
