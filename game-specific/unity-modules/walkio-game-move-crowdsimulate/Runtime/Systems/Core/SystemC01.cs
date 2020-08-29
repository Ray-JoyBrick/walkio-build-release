namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;

    using GameMove = JoyBrick.Walkio.Game.Move;

    //
#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    [DisableAutoCreation]
    public class SystemC01 : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemC01));

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
        private EntityQuery _particleEntityQuery;

        public void Construct()
        {
            _logger.Debug($"Module - Move - CrowdSimulate - SystemC01 - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - CrowdSimulate - SystemC01 - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            
            _particleEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<Particle>(),
                    ComponentType.ReadOnly<ParticleProperty>(),
                    ComponentType.ReadOnly<GameMove.MoveByForce>(),
                    typeof(ParticleNearbyBuffer)
                }
            });

            RequireForUpdate(_particleEntityQuery);
        }

        private void CheckOtherParticle(
            EntityCommandBuffer commandBuffer,
            Entity selfEntity,
            DynamicBuffer<ParticleNearbyBuffer> selfNearbyParticleBuffers,
            float3 selfPosition)
        {
            Entities
                .WithStoreEntityQueryInField(ref _particleEntityQuery)
                .ForEach((Entity entity, LocalToWorld localToWorld, DynamicBuffer<ParticleNearbyBuffer> nearbyParticleBuffers, GameMove.MoveByForce moveByForce) =>
                {
                    if (selfEntity != entity)
                    {
                        // _logger.Debug($"Module - Move - CrowdSimulate - SystemC01 - CheckOtherParticle - self entity: {selfEntity} check with entity: {entity}");
                        var nearby = math.distancesq(localToWorld.Position, selfPosition) < 400.0f;
                        if (nearby)
                        {
                            // _logger.Debug($"Module - Move - CrowdSimulate - SystemC01 - CheckOtherParticle - self entity: {selfEntity} nearby: {entity}");
                            commandBuffer.AppendToBuffer<ParticleNearbyBuffer>(selfEntity, new NearbyParticleDetail
                            {
                                Velocity = moveByForce.Direction * moveByForce.Force,
                                Position = localToWorld.Position,
                                Radius = 1.0f
                            });
                        }
                    }
                })
                .WithoutBurst()
                .Run();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            //
            var deltaTime = Time.DeltaTime;

            Entities
                .WithAll<Particle>()
                .ForEach((Entity entity, LocalToWorld localToWorld, ParticleProperty particleProperty, DynamicBuffer<ParticleNearbyBuffer> nearbyParticleBuffers,  GameMove.MoveByForce moveByForce) =>
                {
                    //
                    nearbyParticleBuffers.Clear();

                    //
                    CheckOtherParticle(
                        commandBuffer,
                        entity,
                        nearbyParticleBuffers,
                        localToWorld.Position);
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
