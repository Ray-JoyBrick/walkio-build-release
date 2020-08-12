namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Physics.Systems;
    using Unity.Transforms;

    using GameMove = JoyBrick.Walkio.Game.Move;

    //
#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    [DisableAutoCreation]
    [UpdateAfter(typeof(StepPhysicsWorld))]
    public class SystemA : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemA));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        //
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        //
        private bool _canUpdate;

        public void Construct()
        {
            _logger.Debug($"Module - Move - CrowdSimulate - SystemA - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - CrowdSimulate - SystemA - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
#endif
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

            //
            var deltaTime = Time.DeltaTime;

            Entities
                .WithAll<Particle>()
                .ForEach((Entity entity, LocalToWorld localToWorld, ParticleProperty particleProperty, GameMove.MoveByForce moveByForce, ref PhysicsVelocity physicsVelocity, ref Rotation rotation) =>
                {
                    // _logger.Debug($"Module - Move - CrowdSimulate - SystemA - OnUpdate - event entity: {entity}");

                    physicsVelocity.Linear = moveByForce.Direction * moveByForce.Force;

                    var adjustedDirection = moveByForce.Direction;
                    // if (moveByForce.Direction.x == 0 && moveByForce.Direction.y == 0 && moveByForce.Direction.z == 0)
                    // {
                    //     adjustedDirection = new float3(1.0f, 0, 0);
                    // }

                    var smoothedRotation = math.slerp(
                        rotation.Value,
                        quaternion.LookRotationSafe(adjustedDirection, math.up()), 1f - math.exp(-deltaTime));
                    rotation.Value = smoothedRotation;
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
