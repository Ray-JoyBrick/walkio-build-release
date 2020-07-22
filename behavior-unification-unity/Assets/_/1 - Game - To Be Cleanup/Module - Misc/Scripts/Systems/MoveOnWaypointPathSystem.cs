namespace Game
{
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;

    [DisableAutoCreation]
    public class MoveOnWaypointPathSystem : SystemBase
    {
        private EntityQuery _theEnvironmentQuery;

        public bool CanUpdate { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            // _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            _theEnvironmentQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(TheEnvironment) }
            });
            
            RequireForUpdate(_theEnvironmentQuery);
        }

        protected override void OnUpdate()
        {
            if (!CanUpdate) return;
            
            //
            var deltaTime = Time.DeltaTime;
            
            var environmentEntity = _theEnvironmentQuery.GetSingletonEntity();
            var levelWaypointPathLookup = EntityManager.GetComponentData<LevelWaypointPathLookup>(environmentEntity);

            Entities
                .WithAll<NeutralForce, Unit>()
                .ForEach((Entity entity, ref MoveOnWaypointPath moveOnWaypointPath, ref PhysicsVelocity physicsVelocity,
                    ref Translation translation, ref Rotation rotation) =>
                {
                    if (moveOnWaypointPath.AtIndex != moveOnWaypointPath.EndPathIndex)
                    {
                        var targetIndex = (moveOnWaypointPath.AtIndex + 1);
                        var targetPosition =
                            levelWaypointPathLookup.WaypointPathBlobAssetRef.Value.Waypoints[targetIndex];

                        var endPathPosition =
                            levelWaypointPathLookup.WaypointPathBlobAssetRef.Value.Waypoints[
                                moveOnWaypointPath.EndPathIndex];

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
                            moveOnWaypointPath.AtIndex = targetIndex;
                        }

                        xDiff = math.abs(newPosition.x - endPathPosition.x);
                        yDiff = math.abs(newPosition.y - endPathPosition.y);
                        zDiff = math.abs(newPosition.z - endPathPosition.z);

                        var closeToEndPathPosition = (xDiff <= 0.05f && yDiff <= 0.05f && zDiff <= 0.05f);
                        if (closeToEndPathPosition && targetIndex == moveOnWaypointPath.EndPathIndex)
                        {
                            moveOnWaypointPath.AtIndex = moveOnWaypointPath.EndPathIndex;
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
    }
}
