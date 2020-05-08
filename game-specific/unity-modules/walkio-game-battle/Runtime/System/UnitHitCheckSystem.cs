namespace JoyBrick.Walkio.Game.Battle
{
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Physics.Systems;
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;

    [System.Serializable]
    public struct AbsorbableUnit : IComponentData
    {
        public float restoredAmount;
    }
    
    [System.Serializable]
    public struct Power : IComponentData
    {
        public float Value;
        public float MaxValue;
    }

    [System.Serializable]
    public struct PlayerCharacter : IComponentData
    {
    }
    
    [DisableAutoCreation]
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class UnitHitCheckSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(MoveOnPathSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        private BuildPhysicsWorld _buildPhysicsWorldSystem;
        private StepPhysicsWorld _stepPhysicsWorldSystem;
        private EndFramePhysicsSystem _endFramePhysicsSystem;
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        private bool _canUpdate;
        
        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"UnitHitCheckSystem - Construct - Receive DoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
            
            FlowControl.CleaningAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _canUpdate = false;
                })
                .AddTo(_compositeDisposable);
        }
        
        // [BurstCompile]
        struct AbsorbNpcSystemJob : ITriggerEventsJob
        {
            public EntityCommandBuffer entityCommandBuffer;

            public ComponentDataFromEntity<AbsorbableUnit> absorbableUnitGroup;
            public ComponentDataFromEntity<Power> powerGroup;
            [ReadOnly] public ComponentDataFromEntity<PlayerCharacter> playerCharacterGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                var entityA = triggerEvent.Entities.EntityA;
                var entityB = triggerEvent.Entities.EntityB;

                var entityAIsAbsorbableUnit = absorbableUnitGroup.Exists(entityA);
                var entityBIsPlayerCharacter = playerCharacterGroup.Exists(entityB);
                
                if (entityAIsAbsorbableUnit && entityBIsPlayerCharacter)
                {
                    _logger.Debug($"entityAIsAbsorbableUnit && entityBIsPlayerCharacter");

                    var absorbableUnit = absorbableUnitGroup[entityA];
                    var power = powerGroup[entityB];

                    power.Value += absorbableUnit.restoredAmount;
                    powerGroup[entityB] = power;

                    entityCommandBuffer.DestroyEntity(entityA);
                }
                else
                {
                    // Debug.Log($"not entityAIsHealthPickup && entityBIsPlayerCharacter");

                    var entityAIsPlayerCharacter = playerCharacterGroup.Exists(entityA);
                    var entityBIsAbsorbableUnit = absorbableUnitGroup.Exists(entityB);
                    if (entityAIsPlayerCharacter && entityBIsAbsorbableUnit)
                    {
                        _logger.Debug($"entityAIsPlayerCharacter && entityBIsAbsorbableUnit");

                        var absorbableUnit = absorbableUnitGroup[entityB];
                        var power = powerGroup[entityA];

                        power.Value = math.clamp(power.Value + absorbableUnit.restoredAmount, 0, power.MaxValue);
                        powerGroup[entityA] = power;

                        entityCommandBuffer.DestroyEntity(entityB);
                    }
                }
            }
        }
        
        protected override void OnCreate()
        {
            base.OnCreate();

            _buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
            _endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }        

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();
            
            var job = new AbsorbNpcSystemJob
            {
                entityCommandBuffer = commandBuffer,
                absorbableUnitGroup = GetComponentDataFromEntity<AbsorbableUnit>(),
                powerGroup = GetComponentDataFromEntity<Power>(),
                playerCharacterGroup = GetComponentDataFromEntity<PlayerCharacter>(true)
            };

            Dependency = job.Schedule(
                _stepPhysicsWorldSystem.Simulation,
                ref _buildPhysicsWorldSystem.PhysicsWorld,
                Dependency);

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
