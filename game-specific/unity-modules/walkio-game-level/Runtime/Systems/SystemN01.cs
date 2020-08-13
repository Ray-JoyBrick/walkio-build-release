namespace JoyBrick.Walkio.Game.Level
{
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Physics.Systems;
    using Unity.Transforms;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    using GameLevel = JoyBrick.Walkio.Game.Level;

    // This system is going to be responsible to destroy old leading-to-set entity and all entity buffer associated
    // with it
    [DisableAutoCreation]
    [UpdateAfter(typeof(BuildPhysicsWorld))]
    // [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class SystemN01 : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemN01));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;
        private EndFramePhysicsSystem _endFramePhysicsSystem;

        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityArchetype _eventEntityArchetype;
        private EntityQuery _levelAbsorbableEntityQuery;

        //
        private bool _canUpdate;

#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        public void Construct()
        {
            _logger.Debug($"Module - Level - SystemN01 - Construct");

            //
#if WALKIO_FLOWCONTROL
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Level - SystemN01 - Construct - Receive FlowReadyToStart");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);

            FlowControl?.AssetUnloadingStarted
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Level - SystemN01 - Construct - Receive AssetUnloadingStarted");
                    _canUpdate = false;
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            //
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
            _endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            _eventEntityArchetype = EntityManager.CreateArchetype(
                typeof(LevelAbsorbableIsHit),
                typeof(LevelAbsorbable));

            _levelAbsorbableEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<LevelAbsorbable>()//,
                    // ComponentType.ReadOnly<LevelAbsorbableProperty>()
                }
            });

            RequireForUpdate(_levelAbsorbableEntityQuery);
        }

        // [BurstCompile]
        struct CollisionAbsorbableJob : ITriggerEventsJob
        {
            public EntityArchetype eventEntityArchetype;
            public EntityCommandBuffer entityCommandBuffer;

            [ReadOnly] public ComponentDataFromEntity<LevelAbsorbable> LevelAbsorbableGroup;

            [ReadOnly] public ComponentDataFromEntity<Translation> LevelAbosrbableGroupTranslation;
            // [ReadOnly] public ComponentDataFromEntity<LevelAbsorbableProperty> LevelAbsorbablePropertyGroup;
            public ComponentDataFromEntity<LevelAbsorber> LevelAbsorberGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity entityA = triggerEvent.EntityA;
                Entity entityB = triggerEvent.EntityB;

                // Debug.Log($"Module - Level - SystemN01 - entityA: {entityA} && entityB: {entityB}");

                bool isAAbsorbable = LevelAbsorbableGroup.HasComponent(entityA);
                bool isBAbosrbable = LevelAbsorbableGroup.HasComponent(entityB);

                bool isAAbsorber = LevelAbsorberGroup.HasComponent(entityA);
                bool isBAbsorber = LevelAbsorberGroup.HasComponent(entityB);

                // Debug.Log($"Module - Level - SystemN01 - isAAbsorbable: {isAAbsorbable} isBAbosrbable: {isBAbosrbable} isAAbsorber: {isAAbsorber} isBAbsorber: {isBAbsorber}");

                if (isAAbsorber && isBAbosrbable)
                {
                    var absorber = LevelAbsorberGroup[entityA];
                    var absorbable = LevelAbsorbableGroup[entityB];

                    var absorbableTranslation = LevelAbosrbableGroupTranslation[entityB];

                   // Debug.Log($"Module - Level - SystemN01 - isAAbsorber && isBAbosrbable");

                   if (!absorbable.TriggerCooldown)
                   {
                       absorbable.TriggerCooldown = true;
                       absorbable.IntervalMax = 0.2f;
                       absorbable.Countdown = 0;

                       var la = new LevelAbsorbable
                       {
                           Kind = absorbable.Kind,

                           TriggerCooldown = true,
                           IntervalMax = 0.2f,
                           Countdown = 0,
                           
                           HitPosition = absorbableTranslation.Value,
                           
                           AttachedEntity = entityB
                       };
                       entityCommandBuffer.SetComponent(entityB, la);

                       var eventEntity = entityCommandBuffer.CreateEntity(eventEntityArchetype);
                       entityCommandBuffer.SetComponent(eventEntity, la);
                   }
                }
                if (isBAbsorber && isAAbsorbable)
                {
                    var absorbable = LevelAbsorbableGroup[entityA];
                    var absorber = LevelAbsorberGroup[entityB];

                    var absorbableTranslation = LevelAbosrbableGroupTranslation[entityA];
                    // Debug.Log($"Module - Level - SystemN01 - isBAbsorber && isAAbsorbable");

                    if (!absorbable.TriggerCooldown)
                    {
                        absorbable.TriggerCooldown = true;
                        absorbable.IntervalMax = 0.2f;
                        absorbable.Countdown = 0;

                        var la = new LevelAbsorbable
                        {
                            Kind = absorbable.Kind,

                            TriggerCooldown = true,
                            IntervalMax = 0.2f,
                            Countdown = 0,
                            
                            HitPosition = absorbableTranslation.Value,

                            AttachedEntity = entityA
                        };
                        entityCommandBuffer.SetComponent(entityA, la);

                        var eventEntity = entityCommandBuffer.CreateEntity(eventEntityArchetype);
                        entityCommandBuffer.SetComponent(eventEntity, la);
                    }
                }
            }
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            // CollisionWorld collisionWorld = _buildPhysicsWorld.PhysicsWorld.CollisionWorld;
            // Dependency = JobHandle.CombineDependencies(Dependency, _endFramePhysicsSystem.GetOutputDependency());

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            Dependency = JobHandle.CombineDependencies(Dependency, _buildPhysicsWorld.GetOutputDependency());

            Dependency = new CollisionAbsorbableJob
                {
                    eventEntityArchetype = _eventEntityArchetype,
                    entityCommandBuffer = commandBuffer,
                    LevelAbsorbableGroup = GetComponentDataFromEntity<LevelAbsorbable>(true),
                    LevelAbosrbableGroupTranslation = GetComponentDataFromEntity<Translation>(true),
                    // LevelAbsorbablePropertyGroup = GetComponentDataFromEntity<LevelAbsorbableProperty>(true),
                    LevelAbsorberGroup = GetComponentDataFromEntity<LevelAbsorber>(),
                }
                .Schedule(
                    _stepPhysicsWorld.Simulation,
                    ref _buildPhysicsWorld.PhysicsWorld,
                    Dependency);

            Dependency.Complete();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
