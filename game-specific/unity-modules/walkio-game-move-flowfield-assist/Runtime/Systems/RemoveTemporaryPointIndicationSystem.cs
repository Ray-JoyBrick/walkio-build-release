namespace JoyBrick.Walkio.Game.Move.FlowField.Assist
{
    using Drawing;
    using UniRx;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;

    //
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;

    //
    using GameMoveFlowField = JoyBrick.Walkio.Game.Move.FlowField;
    using GameMoveFlowFieldUtility = JoyBrick.Walkio.Game.Move.FlowField.Utility;

    [DisableAutoCreation]
    public class RemoveTemporaryPointIndicationSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(RemoveTemporaryPointIndicationSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _entityQuery;

        //
        private bool _canUpdate;

        //
        public GameMoveFlowField.IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }
        public GameFlowControl.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"Module - PresentTemporaryPointIndicationSystem - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - RemoveTemporaryPointIndicationSystem - Construct - Receive FlowReadyToStart");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            _entityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(FlowFieldMoveIndication)
                }
            });
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            //
            var deltaTime = Time.DeltaTime;

            Entities
                .WithAll<TemporaryPointIndication>()
                .ForEach((Entity entity, ref TemporaryPointIndicationProperty temporaryPointIndicationProperty) =>
                {
                    var elapsedTime = temporaryPointIndicationProperty.CountDown + deltaTime;

                    temporaryPointIndicationProperty.CountDown = elapsedTime;

                    commandBuffer.SetComponent(entity, temporaryPointIndicationProperty);
                    if (elapsedTime >= temporaryPointIndicationProperty.IntervalMax)
                    {
                        temporaryPointIndicationProperty.CountDown = 0;

                        commandBuffer.DestroyEntity(entity);
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
