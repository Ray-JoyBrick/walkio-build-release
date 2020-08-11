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

    // This system is going to be responsible to destroy old leading-to-set entity and all entity buffer associated
    // with it
    [DisableAutoCreation]
    // [UpdateAfter(typeof(SystemA))]
    public class SystemH01 : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemH01));

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
            _logger.Debug($"Module - Move - FlowField - SystemH01 - Construct");

            //
#if WALKIO_FLOWCONTROL
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - FlowField - SystemH01 - Construct - Receive FlowReadyToStart");
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

        protected override void OnUpdate()
        {
            // if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var deltaTime = Time.DeltaTime;

            Entities
                .WithAll<ToBeDeletedLeadingToSet>()
                .ForEach((Entity entity, DynamicBuffer<LeadingToTileBuffer> leadingToTileBuffers,  ref ToBeDeletedLeadingToSetProperty toBeDeletedLeadingToSetProperty) =>
                {
                    var elapsedTime = toBeDeletedLeadingToSetProperty.CountDown + deltaTime;

                    toBeDeletedLeadingToSetProperty.CountDown = elapsedTime;

                    if (elapsedTime >= toBeDeletedLeadingToSetProperty.IntervalMax)
                    {
                        toBeDeletedLeadingToSetProperty.CountDown = 0;

                        for (var i = 0; i < leadingToTileBuffers.Length; ++i)
                        {
                            // First tile should be previous at-tile which should be destroyed else where?

                            commandBuffer.DestroyEntity(leadingToTileBuffers[i].Value.Tile);
                        }

                        commandBuffer.DestroyEntity(entity);
                    }
                })
                .WithoutBurst()
                .Run();

            Entities
                .WithAll<ToBeDeletedFlowFieldTile>()
                .ForEach((Entity entity) =>
                {
                    commandBuffer.DestroyEntity(entity);
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
