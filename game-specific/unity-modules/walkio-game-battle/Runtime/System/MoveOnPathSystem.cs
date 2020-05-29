namespace JoyBrick.Walkio.Game.Battle
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Physics.Extensions;
    using Unity.Physics.Systems;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;
    
    // https://forum.unity.com/threads/update-systems-at-fixedupdate-frequency-with-new-apis.657565/
    [DisableAutoCreation]
    // [UpdateBefore(typeof(FixedUpdate))]
    // [UpdateAfter(typeof(StepPhysicsWorld))]
    [UpdateAfter(typeof(ExportPhysicsWorld))]
    public class MoveOnPathSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(MoveOnPathSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _theEnvironmentQuery;

        private bool _canUpdate;
        
        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"MoveOnPathSystem - Construct - Receive DoneSettingAsset");
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

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            _theEnvironmentQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(GameEnvironment.TheEnvironment) }
            });
            
            RequireForUpdate(_theEnvironmentQuery);
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            var theEnvironmentEntity = _theEnvironmentQuery.GetSingletonEntity();
            var levelWaypointPathLookup = EntityManager.GetComponentData<GameEnvironment.LevelWaypointPathLookup>(theEnvironmentEntity);

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var deltaTime = Time.DeltaTime;

            Entities
                .WithAll<Unit, NeutralForce>()
                .ForEach((Entity entity, ref MoveOnPath moveOnPath, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass, ref Translation translation) =>
                {
                    // _logger.Debug($"MoveOnPathSystem - OnUpdate - physicsVelocity: {physicsVelocity.Linear}");
                    // if (physicsVelocity.Linear.x == 0)
                    // {
                    //     _logger.Debug($"MoveOnPathSystem - OnUpdate - Set velocity to physicsVelocity: {physicsVelocity.Linear}");
                    //     physicsVelocity.Linear.x = 100.0f;
                    // }
                    // physicsVelocity.Linear.x = 1.0f;
                    // physicsVelocity.
                    // physicsVelocity.ApplyLinearImpulse(physicsMass, new float3(1.0f, 0, 1.0f));
                    // physicsVelocity.ApplyLinearImpulse(physicsMass, new float3(100.0f, 0, 10.0f));
                    // _logger.Debug($"MoveOnPathSystem - OnUpdate - physicsVelocity: {physicsVelocity.Linear}");
                    // physicsVelocity.Linear = new float3(1.0f, 0, 1.0f);
                    // //
                    if (moveOnPath.AtIndex != moveOnPath.EndIndex)
                    {
                    
                        // Check if close to end of the path
                    
                        // Not at the end of the path, keep moving
                        var targetIndex = (moveOnPath.AtIndex + 1);
                        var targetPosition =
                            levelWaypointPathLookup.WaypointPathBlobAssetRef.Value.Waypoints[targetIndex];
                    
                        var endPathPosition =
                            levelWaypointPathLookup.WaypointPathBlobAssetRef.Value.Waypoints[moveOnPath.EndIndex];
                    
                        var direction = targetPosition - translation.Value;
                        var normalizedDirection = math.normalize(direction);
                    
                        // translation.Value.x = translation.Value.x + ((direction.x * Time.DeltaTime) * 5.0f);
                        // translation.Value.z = translation.Value.z + ((direction.z * Time.DeltaTime) * 5.0f);
                        // translation.Value = translation.Value + normalizedDirection;
                        // var newPosition = translation.Value + normalizedDirection;
                        var newPosition = new float3(
                            translation.Value.x + normalizedDirection.x * deltaTime,
                            translation.Value.y + normalizedDirection.y * deltaTime,
                            translation.Value.z + normalizedDirection.z * deltaTime);
                    
                        // Debug.Log($"MoveOnPathSystem - targetPosition: {targetPosition} direction: {normalizedDirection} translation: {newPosition}");
                    
                        // translation.Value = newPosition;
                    
                        // Checking rule has to adjusted, if speed is fast enough, will keep bouncing around but
                        // missing the in-range check
                        var xDiff = math.abs(newPosition.x - targetPosition.x);
                        var yDiff = math.abs(newPosition.y - targetPosition.y);
                        var zDiff = math.abs(newPosition.z - targetPosition.z);
                    
                        var closeToTargetPathPosition = (xDiff <= 0.05f && yDiff <= 0.05f && zDiff <= 0.05f);
                    
                        if (closeToTargetPathPosition)
                        {
                            moveOnPath.AtIndex = targetIndex;
                        }
                    
                        xDiff = math.abs(newPosition.x - endPathPosition.x);
                        yDiff = math.abs(newPosition.y - endPathPosition.y);
                        zDiff = math.abs(newPosition.z - endPathPosition.z);
                    
                        var closeToEndPathPosition = (xDiff <= 0.05f && yDiff <= 0.05f && zDiff <= 0.05f);
                        if (closeToEndPathPosition && targetIndex == moveOnPath.EndIndex)
                        {
                            moveOnPath.AtIndex = moveOnPath.EndIndex;
                        }
                    
                        // translation.Value = newPosition;

                        // physicsVelocity.Linear = newPosition;
                        physicsVelocity.Linear = 
                            new float3(normalizedDirection.x, 0, normalizedDirection.z) * 2.0f;

                        // var impulse = normalizedDirection * 1.0f;
                        // // physicsVelocity.ApplyLinearImpulse(physicsMass, impulse);
                        // physicsVelocity.Linear = impulse;
                        // commandBuffer.SetComponent(entity, new Translation
                        // {
                        //     Value = newPosition
                        // });
                    }
                })
                // .Schedule();
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
