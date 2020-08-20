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
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class MoveSystemA01 : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(MoveSystemA01));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        // private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        private EntityQuery _crowdSimulationWorldEntityQuery;
        private EntityQuery _particleEntityQuery;

        private List<ParticleProperty> _particleProperties = new List<ParticleProperty>();
        private SPHSolver _sphSolver;

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
                    _logger.Debug($"Module - Move - CrowdSimulate - MoveSystemA01 - Construct - Receive FlowReadyToStart");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            // _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            _crowdSimulationWorldEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(CrowdSimulationWorld),
                    typeof(CrowdSimulationWorldProperty)
                }
            });

            _particleEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(Particle),
                    typeof(ParticleProperty),
                    typeof(ParticleNearbyBuffer)
                }
            });

            RequireForUpdate(_crowdSimulationWorldEntityQuery);
            RequireForUpdate(_particleEntityQuery);

        }

        public static float KernelPoly6(float kPoly6Const, float kr2, float inverseKr9, float3 r)
        {
            var sqrDiff = (kr2 - math.dot(r, r));
            if (sqrDiff < 0) return 0;
            return kPoly6Const * inverseKr9 * sqrDiff * sqrDiff * sqrDiff;
        }

        public static float3 GradKernelSpiky(float gradKSpikyConst, float kernelRadius, float inverseKr6, float3 r)
        {
            float mag = math.length(r);
            float diff = (kernelRadius - mag);
            if (diff < 0 || mag <= 0) return float3.zero;

            r *= (1 / mag);
            return gradKSpikyConst * inverseKr6 * diff * diff * r;
        }

        public static float LaplacianKernelViscosity(float lapkViscosityConst, float kernelRadius, float inverseKr6, float3 r)
        {
            float mag = math.length(r);
            float diff = kernelRadius - mag;
            if (diff < 0 || mag <= 0) return 0;
            return lapkViscosityConst * inverseKr6 * diff;
        }

        protected override void OnUpdate()
        {
            if (_canUpdate) return;

            var crowdSimulationWorldEntity = _crowdSimulationWorldEntityQuery.GetSingletonEntity();
            var crowdSimulationWorldProperty = _crowdSimulationWorldEntityQuery.GetSingleton<CrowdSimulationWorldProperty>();

            var kPoly6Const = crowdSimulationWorldProperty.SPHSolverBlobAssetRef.Value.Context.Value.KPoly6Const;
            var gradKSpikyConst = -14.3239448783f;
            var lapkViscosityConst = 14.3239448783f;

            var kernelRadius = crowdSimulationWorldProperty.SPHSolverBlobAssetRef.Value.Context.Value.KernelRadius;
            var stiffness = crowdSimulationWorldProperty.SPHSolverBlobAssetRef.Value.Context.Value.Stiffness;
            var restDensity = crowdSimulationWorldProperty.SPHSolverBlobAssetRef.Value.Context.Value.RestDensity;
            var viscosity = crowdSimulationWorldProperty.SPHSolverBlobAssetRef.Value.Context.Value.Viscosity;


            var kr2 = kernelRadius * kernelRadius;
            var inverseKr3 = 1.0f / (kernelRadius * kr2);
            var inverseKr6 = inverseKr3 * inverseKr3;;
            var inverseKr9 = inverseKr6 * inverseKr3;

            // if (_sphSolver == null && crowdSimulationWorldProperty.SPHSolverBlobAssetRef.Value.Context.Value.SPHShader != null)
            // {
            //     Debug.Log($"MoveSystem - OnUpdate - assign to sph solver");
            //     _sphSolver = new SPHSolver(
            //         crowdSimulationWorldProperty.SPHSolverBlobAssetRef.Value.Context.Value.Stiffness,
            //         crowdSimulationWorldProperty.SPHSolverBlobAssetRef.Value.Context.Value.SPHShader);
            //
            //     // _sphSolver.InitializeOnGPU();
            // }

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            var deltaTime = Time.DeltaTime;

            //
            // _particleProperties.Clear();
            var particlePropertyComponentDatas =  GetComponentDataFromEntity<ParticleProperty>();
            // var particlePropertyComponentDatas =  GetComponentDataFromEntity<ParticleProperty>(true);

            Entities
                .WithAll<Particle>()
                // .WithReadOnly(particlePropertyComponentDatas)
                .ForEach((Entity entity,
                    int entityInQueryIndex,
                    DynamicBuffer<ParticleNearbyBuffer> particleNearybyBuffers//,
                    // ref ParticleProperty particleProperty
                    ) =>
                {
                    // var particlePropertyComponentDatas =  GetComponentDataFromEntity<ParticleProperty>();
                    // _particleProperties.Add(particleProperty);

                    var particleProperty = particlePropertyComponentDatas[entity];

                    particleProperty.InverseDensity = 0;

                    for (var i = 0; i < particleNearybyBuffers.Length; ++i)
                    {
                        var nearbyEntity = particleNearybyBuffers[i].Value.Entity;
                        var nearbyParticleProperty =
                            particlePropertyComponentDatas[nearbyEntity];

                        var relativePosition = particleProperty.Position - nearbyParticleProperty.Position;
                        particleProperty.InverseDensity +=
                            (nearbyParticleProperty.Mass *
                             KernelPoly6(kPoly6Const, kr2, inverseKr9, relativePosition));
                    }

                    if (particleProperty.InverseDensity <= restDensity)
                    {
                        particleProperty.Pressure = 0;
                        particleProperty.InverseDensity = 1.0f / restDensity;
                    }
                    else
                    {
                        particleProperty.Pressure = stiffness * (particleProperty.InverseDensity - restDensity);
                        particleProperty.InverseDensity = 1.0f / particleProperty.InverseDensity;
                    }

                    // Debug.Log($"MoveSystem - OnUpdate - {entity} InverseDensity: {particleProperty.InverseDensity}");
                })
                // .WithoutBurst()
                // .Run();
                .WithBurst()
                .Schedule();


            Entities
                .WithAll<Particle>()
                // .WithReadOnly(particlePropertyComponentDatas)
                .ForEach((Entity entity,
                    int entityInQueryIndex,
                    DynamicBuffer<ParticleNearbyBuffer> particleNearbyBuffers//,
                    // ref ParticleProperty particleProperty
                ) =>
                {
                    // var particlePropertyComponentDatas =  GetComponentDataFromEntity<ParticleProperty>();
                    // _particleProperties.Add(particleProperty);

                    var particleProperty = particlePropertyComponentDatas[entity];

                    particleProperty.ForcePressure = float3.zero;
                    particleProperty.ForceViscosity = float3.zero;

                    for (var i = 0; i < particleNearbyBuffers.Length; ++i)
                    {
                        var nearbyEntity = particleNearbyBuffers[i].Value.Entity;
                        // var hasComp = HasComponent<ParticleProperty>(nearbyEntity);
                        // if (hasComp)
                        {
                            var nearbyParticleProperty =
                                particlePropertyComponentDatas[nearbyEntity];

                            var relativePosition = particleProperty.Position - nearbyParticleProperty.Position;
                            particleProperty.ForcePressure +=
                                (-0.5f * nearbyParticleProperty.Mass *
                                 (particleProperty.Pressure + nearbyParticleProperty.Pressure) *
                                 nearbyParticleProperty.InverseDensity *
                                 GradKernelSpiky(gradKSpikyConst, kernelRadius, inverseKr6, relativePosition));

                            particleProperty.ForceViscosity +=
                                (nearbyParticleProperty.Mass *
                                 (nearbyParticleProperty.Direction - particleProperty.Direction) *
                                 nearbyParticleProperty.InverseDensity *
                                 LaplacianKernelViscosity(lapkViscosityConst, kernelRadius, inverseKr6, relativePosition));

                            //      (particleProperty.Pressure + particleNearybyBuffers[i].Value.Pressure) *
                            //      KernelPoly6(kPoly6Const, kr2, inverseKr9, relativePosition));

                            // Debug.Log($"MoveSystem - OnUpdate - {entity} ForcePressure: {particleProperty.ForcePressure}");
                        }
                    }

                    particleProperty.ForceViscosity *= viscosity;
                    // Debug.Log($"MoveSystem - OnUpdate - {entity} ForceViscosity: {particleProperty.ForceViscosity}");
                })
                // .WithoutBurst()
                // .Run();
                .WithBurst()
                .Schedule();

            Entities
                .WithAll<Particle>()
                .ForEach((Entity entity,
                    int entityInQueryIndex,
                    ref ParticleProperty particleProperty,
                    ref Translation translation,
                    // ref PhysicsVelocity physicsVelocity,
                    ref Rotation rotation) =>
                {
                    var acceleration =
                        (particleProperty.ForcePressure +
                        particleProperty.ForceViscosity) / particleProperty.Mass;

                    particleProperty.Position += particleProperty.Direction * 0.01f;
                    particleProperty.Direction += acceleration * 0.01f;

                    translation.Value = particleProperty.Position;
                    // physicsVelocity.Linear = particleProperty.Direction;
                    // physicsVelocity.Linear = acceleration;

                    var adjustedDirection = math.normalize(particleProperty.Direction);
                    var smoothedRotation = math.slerp(
                        rotation.Value,
                        quaternion.LookRotationSafe(adjustedDirection, math.up()), 1f - math.exp(-deltaTime));
                    rotation.Value = smoothedRotation;
                })
                // .WithoutBurst()
                // .Run();
                .WithBurst()
                .Schedule();


            // //
            // using (var sphSolver = new SPHSolver(
            //     crowdSimulationWorldProperty.SPHSolverBlobAssetRef.Value.Context.Value.Stiffness,
            //     crowdSimulationWorldProperty.SPHSolverBlobAssetRef.Value.Context.Value.SPHShader))
            // {
            //     sphSolver?.AddParticles(_particleProperties);
            //     sphSolver?.StepOnGPU();
            // }

            // using (var particles = new NativeArray<ParticleProperty>(particleList.Count, Allocator.TempJob))
            // {
            //     particles.CopyFrom(particleList.ToArray());
            // }

            // using (var particleEntities = _particleEntityQuery.ToEntityArray(Allocator.TempJob))
            // using (var particles = new NativeArray<Particle>(particleEntities.Length, Allocator.TempJob))
            // {
            //     var particleComponentDatas =  GetComponentDataFromEntity<Particle>(true);
            //
            //     Debug.Log($"Particle count: {particleEntities.Length}");
            //
            //     var particleArray = new Particle[particleEntities.Length];
            //
            //     for (var i = 0; i < particleEntities.Length; ++i)
            //     {
            //         var particleEntity = particleEntities[i];
            //         var particleComponentData = particleComponentDatas[particleEntity];
            //         // particleArray[i] = particleComponentData;
            //     }
            //
            //     // particles.CopyFrom(particleArray);
            //     //
            //     // _sphSolver.AddParticles(particles);
            // }


            // var arr = _particleEntityQuery.CreateArchetypeChunkArray(Allocator.TempJob);
            // arr[0].

            // Entities
            //     .WithAll<Particle>()
            //     .ForEach((Entity entity,
            //         int entityInQueryIndex,
            //         ParticleProperty particleProperty,
            //         DynamicBuffer<ParticleNearybyBuffer> particleNearybyBuffers,
            //         ref PhysicsVelocity physicsVelocity,
            //         ref Rotation rotation) =>
            //     {
            //         var direction = particleProperty.Direction;
            //         for (var i = 0; i < particleNearybyBuffers.Length; ++i)
            //         {
            //             direction += particleNearybyBuffers[i].Value.Direction;
            //         }
            //
            //         physicsVelocity.Linear = particleProperty.Direction;
            //
            //         var adjustedDirection = math.normalize(direction);
            //
            //         var smoothedRotation = math.slerp(
            //             rotation.Value,
            //             quaternion.LookRotationSafe(adjustedDirection, math.up()), 1f - math.exp(-deltaTime));
            //         rotation.Value = smoothedRotation;
            //
            //         // if (particleNearybyBuffers.Length > 0)
            //         // {
            //         //     Debug.Log($"Particle: {entity.ToString()} nearby count: {particleNearybyBuffers.Length} first: {particleNearybyBuffers[0].Value.ToString()}");
            //         //     // for (var i = 0; i < particleNearybyBuffers.Length; ++i)
            //         //     // {
            //         //     //     Debug.Log($"Entity: {particleNearybyBuffers[i].Value}");
            //         //     // }
            //         // }
            //     })
            //     // .WithoutBurst()
            //     // .Run();
            //     .WithBurst()
            //     .Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _sphSolver?.Dispose();
        }
    }

}
