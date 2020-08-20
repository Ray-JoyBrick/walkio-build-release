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
    using UnityEngine;
    using GameMove = JoyBrick.Walkio.Game.Move;

    //
#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    [DisableAutoCreation]
    public class SystemC02 : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemC02));

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
            _logger.Debug($"Module - Move - CrowdSimulate - SystemC02 - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - CrowdSimulate - SystemC02 - Construct - Receive AllDoneSettingAsset");
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

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            //
            var deltaTime = Time.DeltaTime;

            Entities
                .WithAll<Particle>()
                .ForEach((Entity entity, LocalToWorld localToWorld, DynamicBuffer<ParticleNearbyBuffer> nearbyParticleBuffers,  GameMove.MoveByForce moveByForce, ref ParticleProperty particleProperty) =>
                {
                    particleProperty.PrefVelocity = moveByForce.Direction;
                    
                    var position = localToWorld.Position;
                    var radius = particleProperty.Radius;
                    var velocity = particleProperty.Velocity;
                    
                    // particleProperty.Force = (particleProperty.PrefVelocity - particleProperty.Velocity) / particleProperty.Ksi;
                    
                    var nearbyAgentCount = nearbyParticleBuffers.Length;
                    for (var i = 0; i < nearbyAgentCount; ++i)
                    {
                        var otherAgent = nearbyParticleBuffers[i];

                        var distanceToOtherAgentSquare = math.distance(otherAgent.Value.Position, position);
                        var radiusOfBothAgent = (otherAgent.Value.Radius + radius);
                        var radiusOfBothAgentSquare = (radiusOfBothAgent * radiusOfBothAgent);

                        if (distanceToOtherAgentSquare != radiusOfBothAgentSquare)
                        {
                            // Debug.Log($"Agent - ComputeForces - distanceToOtherAgentSquare: {distanceToOtherAgentSquare}");

                            if (distanceToOtherAgentSquare < radiusOfBothAgentSquare)
                            {
                                var distanceToOtherAgent = math.distance(otherAgent.Value.Position, position);
                                var recalculateLength = ((otherAgent.Value.Radius + radius) - distanceToOtherAgent);
                                radiusOfBothAgentSquare = (recalculateLength * recalculateLength);
                            }

                            var w = otherAgent.Value.Position - position;
                            var v = velocity - otherAgent.Value.Velocity;

                            var a = math.dot(v, v);
                            var b = math.dot(w, v);
                            var c = math.dot(w, w) - radiusOfBothAgentSquare;

                            var discr = (b * b) - (a * c);
                            if ((discr > Mathf.Epsilon) && (Mathf.Abs(a) > Mathf.Epsilon))
                            {
                                var discrSquareRoot = Mathf.Sqrt(discr);
                                var t = ((b - discrSquareRoot) / a);
                                if (t > Mathf.Epsilon)
                                {
                                    // particleProperty.Force += (
                                    //     (-particleProperty.K * Mathf.Exp(-t / particleProperty.T0))
                                    //     * (v - (b * v - a * w) / discrSquareRoot)
                                    //     / (a * Mathf.Pow(t, particleProperty.M))
                                    //     * (particleProperty.M / t + 1.0f / particleProperty.T0)
                                    // );
                                }
                            }
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
