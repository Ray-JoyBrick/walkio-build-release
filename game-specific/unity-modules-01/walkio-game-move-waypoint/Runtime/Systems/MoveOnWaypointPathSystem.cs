namespace JoyBrick.Walkio.Game.Move.Waypoint
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class MoveOnWaypointPathSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(MoveOnWaypointPathSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        private EntityQuery _waypointPathLookupAttachmentQuery;

        //
        private bool _canUpdate;
        
        //
        public GameCommon.IFlowControl FlowControl { get; set; }
        
        public void Construct()
        {
            _logger.Debug($"MoveOnWaypointPathSystem - Construct");

            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"MoveOnWaypointPathSystem - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
        }        

        protected override void OnCreate()
        {
            base.OnCreate();
            // _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            _waypointPathLookupAttachmentQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(GameCommon.WaypointPathLookupAttachment) }
            });
            
            RequireForUpdate(_waypointPathLookupAttachmentQuery);
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            //
            var deltaTime = Time.DeltaTime;
            
            //
            var environmentEntity = _waypointPathLookupAttachmentQuery.GetSingletonEntity();
            var waypointPathLookup = EntityManager.GetComponentData<WaypointPathLookup>(environmentEntity);

            //
            Entities
                .WithAll<MoveOnWaypointPath>()
                .ForEach((Entity entity, ref MoveOnWaypointPathProperty moveOnWaypointPathProperty, ref PhysicsVelocity physicsVelocity,
                    ref Translation translation, ref Rotation rotation) =>
                {
                    if (moveOnWaypointPathProperty.AtIndex != moveOnWaypointPathProperty.EndPathIndex)
                    {
                        var targetIndex = (moveOnWaypointPathProperty.AtIndex + 1);
                        var targetPosition =
                            waypointPathLookup.WaypointPathBlobAssetRef.Value.Waypoints[targetIndex];

                        var endPathPosition =
                            waypointPathLookup.WaypointPathBlobAssetRef.Value.Waypoints[
                                moveOnWaypointPathProperty.EndPathIndex];

                        var direction = targetPosition - translation.Value;
                        var normalizedDirection = math.normalize(direction);

                        var newPosition = new float3(
                            translation.Value.x + normalizedDirection.x * deltaTime,
                            translation.Value.y + normalizedDirection.y * deltaTime,
                            translation.Value.z + normalizedDirection.z * deltaTime);

                        var xDiff = math.abs(newPosition.x - targetPosition.x);
                        var yDiff = math.abs(newPosition.y - targetPosition.y);
                        var zDiff = math.abs(newPosition.z - targetPosition.z);

                        var closeToTargetPathPosition = (xDiff <= 0.05f && yDiff <= 0.05f && zDiff <= 0.05f);

                        if (closeToTargetPathPosition)
                        {
                            moveOnWaypointPathProperty.AtIndex = targetIndex;
                        }

                        xDiff = math.abs(newPosition.x - endPathPosition.x);
                        yDiff = math.abs(newPosition.y - endPathPosition.y);
                        zDiff = math.abs(newPosition.z - endPathPosition.z);

                        var closeToEndPathPosition = (xDiff <= 0.05f && yDiff <= 0.05f && zDiff <= 0.05f);
                        if (closeToEndPathPosition && targetIndex == moveOnWaypointPathProperty.EndPathIndex)
                        {
                            moveOnWaypointPathProperty.AtIndex = moveOnWaypointPathProperty.EndPathIndex;
                        }

                        physicsVelocity.Linear =
                            new float3(normalizedDirection.x, 0, normalizedDirection.z) * 1.0f;

                        var smoothedRotation = math.slerp(
                            rotation.Value,
                            quaternion.LookRotationSafe(normalizedDirection, math.up()), 1f - math.exp(-deltaTime));
                        rotation.Value = smoothedRotation;
                    }
                })
                // .WithoutBurst()
                // .Schedule();
                .ScheduleParallel();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
}
