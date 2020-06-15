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

    // [System.Serializable]
    public struct Pickup : IComponentData
    {
        public int Id;
        public bool Interacted;
    }

    public struct UnitInteractWithPickup : IComponentData
    {
    }
    
    // [DisableAutoCreation]
    // [UpdateAfter(typeof(EndFramePhysicsSystem))]
    // public class PickupHitCheckSystem : SystemBase
    // {
    //     private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(PickupHitCheckSystem));
    //
    //     //
    //     private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
    //
    //     //
    //     private BuildPhysicsWorld _buildPhysicsWorldSystem;
    //     private StepPhysicsWorld _stepPhysicsWorldSystem;
    //     private EndFramePhysicsSystem _endFramePhysicsSystem;
    //     private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
    //
    //     private EntityArchetype _eventEntityArchetype;
    //
    //     //
    //     private bool _canUpdate;
    //
    //     public GameCommon.IFlowControl FlowControl { get; set; }
    //
    //     public void Construct()
    //     {
    //         //
    //         FlowControl.AllDoneSettingAsset
    //             .Where(x => x.Name.Contains("Stage"))
    //             .Subscribe(x =>
    //             {
    //                 _logger.Debug($"PickupHitCheckSystem - Construct - Receive AllDoneSettingAsset");
    //                 _canUpdate = true;
    //             })
    //             .AddTo(_compositeDisposable);
    //         
    //         FlowControl.CleaningAsset
    //             .Where(x => x.Name.Contains("Stage"))
    //             .Subscribe(x =>
    //             {
    //                 _canUpdate = false;
    //             })
    //             .AddTo(_compositeDisposable);
    //     }
    //     
    //     // [BurstCompile]
    //     struct PickupHitCheckSystemJob : ITriggerEventsJob
    //     {
    //         //
    //         public EntityArchetype eventEntityArchetype;
    //         public EntityCommandBuffer entityCommandBuffer;
    //
    //         //
    //         public ComponentDataFromEntity<AbsorbablePickup> absorbablePickupGroup;
    //         [ReadOnly] public ComponentDataFromEntity<AbsorbPickup> absorbPickupGroup;
    //
    //         public void Execute(TriggerEvent triggerEvent)
    //         {
    //             var entityA = triggerEvent.Entities.EntityA;
    //             var entityB = triggerEvent.Entities.EntityB;
    //
    //             var entityAIsAbsorbablePickup = absorbablePickupGroup.Exists(entityA);
    //             var entityBIsAbsorbPickup = absorbPickupGroup.Exists(entityB);
    //             
    //             if (entityAIsAbsorbablePickup && entityBIsAbsorbPickup)
    //             {
    //                 _logger.Debug($"entityAIsAbsorbablePickup && entityBIsAbsorbPickup");
    //
    //                 var pickupUnit = absorbablePickupGroup[entityA];
    //
    //                 if (!pickupUnit.Interacted)
    //                 {
    //                     var absorbPickup = absorbPickupGroup[entityB];
    //
    //                     pickupUnit.Interacted = true;
    //
    //                     entityCommandBuffer.CreateEntity(eventEntityArchetype);
    //                 }
    //             }
    //             else
    //             {
    //                 var entityAIsAbsorbPickup = absorbPickupGroup.Exists(entityA);
    //                 var entityBIsAbsorbablePickup = absorbablePickupGroup.Exists(entityB);
    //                 if (entityAIsAbsorbPickup && entityBIsAbsorbablePickup)
    //                 {
    //                     _logger.Debug($"entityAIsAbsorbPickup && entityBIsAbsorbablePickup");
    //
    //                     var pickupUnit = absorbablePickupGroup[entityB];
    //
    //                     if (!pickupUnit.Interacted)
    //                     {
    //                         var absorbPickup = absorbPickupGroup[entityA];
    //
    //                         pickupUnit.Interacted = true;
    //
    //                         entityCommandBuffer.CreateEntity(eventEntityArchetype);
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //     
    //     protected override void OnCreate()
    //     {
    //         base.OnCreate();
    //
    //         _buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
    //         _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
    //         _endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
    //         _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    //
    //         _eventEntityArchetype = EntityManager.CreateArchetype(
    //             typeof(UnitInteractWithPickup));
    //     }        
    //
    //     protected override void OnUpdate()
    //     {
    //         if (!_canUpdate) return;
    //
    //         var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
    //         var concurrentCommandBuffer = commandBuffer.ToConcurrent();
    //         
    //         var job = new PickupHitCheckSystemJob
    //         {
    //             eventEntityArchetype = _eventEntityArchetype,
    //             entityCommandBuffer = commandBuffer,
    //             absorbablePickupGroup = GetComponentDataFromEntity<AbsorbablePickup>(),
    //             absorbPickupGroup = GetComponentDataFromEntity<AbsorbPickup>(true)
    //         };
    //
    //         Dependency = job.Schedule(
    //             _stepPhysicsWorldSystem.Simulation,
    //             ref _buildPhysicsWorldSystem.PhysicsWorld,
    //             Dependency);
    //
    //         _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    //     }
    //
    //     protected override void OnDestroy()
    //     {
    //         base.OnDestroy();
    //
    //         _compositeDisposable?.Dispose();
    //     }
    // }
}
