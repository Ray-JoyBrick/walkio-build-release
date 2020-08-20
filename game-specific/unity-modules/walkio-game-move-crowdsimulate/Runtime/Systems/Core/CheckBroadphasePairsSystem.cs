namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
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
    [UpdateBefore(typeof(StepPhysicsWorld))]
    public class CheckBroadphasePairsSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemA));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private EntityQuery _particleEntityQuery;

        private BuildPhysicsWorld _physicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;

        //
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityArchetype _eventEntityArchetype;

        private bool _canUpdate;

        //
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        public void Construct()
        {
            _logger.Debug($"Module - Move - CrowdSimulate - SystemA - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - CrowdSimulate - CheckBroadphasePairsSystem - Construct - Receive FlowReadyToStart");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            _physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            _particleEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(Particle),
                    typeof(ParticleProperty),
                    typeof(ParticleNearbyBuffer)
                }
            });

            RequireForUpdate(_particleEntityQuery);
        }

        // [BurstCompile]
        struct StorePairsJob : IBodyPairsJob
        {
            public NativeArray<RigidBody> Bodies;
            // [ReadOnly] public NativeArray<MotionVelocity> Motions;

            [ReadOnly] public ComponentDataFromEntity<ParticleProperty> ParticleProperties;
            public BufferFromEntity<ParticleNearbyBuffer> ParticleNearbyBuffers;

            public EntityCommandBuffer.ParallelWriter ConcurrentCommandBuffer;

            public unsafe void Execute(ref ModifiableBodyPair pair)
            {
                // Disable the pair if a box collides with a static object
                var entityA = pair.EntityA;
                var entityB = pair.EntityB;

                int indexA = pair.BodyIndexA;
                int indexB = pair.BodyIndexB;

                var isAParticle = ParticleProperties.HasComponent(entityA);
                var isBParticle = ParticleProperties.HasComponent(entityB);

                if (isAParticle && isBParticle && (entityA != entityB))
                {
                    var buffer = ParticleNearbyBuffers[entityA];

                    var bodyB = Bodies[indexB];
                    var particlePropertyB = ParticleProperties[entityB];

                    ConcurrentCommandBuffer.AppendToBuffer<ParticleNearbyBuffer>(0, entityA, new NearbyParticleDetail
                    {
                        Entity = entityB,
                        Position = bodyB.WorldFromBody.pos,
                        Direction = particlePropertyB.Direction,
                        Force = particlePropertyB.Force,

                        Mass = particlePropertyB.Mass,
                        // Pressure = particlePropertyB.Pressure
                    });

                    // var eventEntity = ConcurrentCommandBuffer.CreateEntity(0, EventEntityArchetype);
                    // ConcurrentCommandBuffer.SetComponent(0, eventEntity, new BroadphaseCollideProperty
                    // {
                    //     EntityA = entityA,
                    //     EntityB = entityB
                    // });
                }

                // int indexA = pair.BodyIndexA;
                // int indexB = pair.BodyIndexB;

                // if ((Bodies[indexA].Collider != null && Bodies[indexA].Collider.Value.Type == ColliderType.Box && indexB >= Motions.Length)
                //     || (Bodies[indexB].Collider != null && Bodies[indexB].Collider.Value.Type == ColliderType.Box && indexA >= Motions.Length))
                // {
                //     pair.Disable();
                // }
                // Debug.Log($"indexA: {indexA} indexB: {indexB}");

                // ConcurrentCommandBuffer.AppendToBuffer();
            }
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            if(_stepPhysicsWorld.Simulation.Type == SimulationType.NoPhysics) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            Dependency =
                Entities
                    .WithAll<Particle>()
                    .ForEach((Entity entity,
                        int entityInQueryIndex,
                        ref DynamicBuffer<ParticleNearbyBuffer> particleNearbyBuffers) =>
                    {
                        particleNearbyBuffers.Clear();
                    })
                    // .WithoutBurst()
                    // .Run();
                    .WithBurst()
                    .Schedule(Dependency);

            // Dependency.Complete();

            // Add a custom callback to the simulation, which will inject our custom job after the body pairs have been created
            SimulationCallbacks.Callback callback = (ref ISimulation simulation, ref PhysicsWorld world, JobHandle inDeps) =>
            {
                inDeps.Complete(); //<todo Needed to initialize our modifier

                // inDeps =
                //     Entities
                //         .WithAll<Particle>()
                //         .ForEach((Entity entity,
                //             int entityInQueryIndex,
                //             ref DynamicBuffer<ParticleNearybyBuffer> particleNearybyBuffers) =>
                //         {
                //             particleNearybyBuffers.Clear();
                //         })
                //         // .WithoutBurst()
                //         // .Run();
                //         .WithBurst()
                //         .Schedule(inDeps);

                return new StorePairsJob
                {
                    Bodies = _physicsWorld.PhysicsWorld.Bodies,
                    // Motions = _physicsWorld.PhysicsWorld.MotionVelocities,
                    ParticleProperties = GetComponentDataFromEntity<ParticleProperty>(true),
                    ParticleNearbyBuffers = GetBufferFromEntity<ParticleNearbyBuffer>(),

                    ConcurrentCommandBuffer = concurrentCommandBuffer
                // }.Schedule(simulation, ref world, Dependency);
                }.Schedule(simulation, ref world, inDeps);
            };
            _stepPhysicsWorld.EnqueueCallback(SimulationCallbacks.Phase.PostCreateDispatchPairs, callback, Dependency);

            // Dependency.Complete();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

            Dependency.Complete();

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
