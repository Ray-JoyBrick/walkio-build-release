namespace JoyBrick.Walkio.Game.Move.Waypoint.Assist
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class WaypointPathRenderSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(WaypointPathRenderSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        private EntityQuery _waypointPathLookupAttachmentQuery;

        //
        private bool _canUpdate;
        
        //
        public GameCommon.IFlowControl FlowControl { get; set; }
        
        public void Construct()
        {
            _logger.Debug($"WaypointPathRenderSystem - Construct");

            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"WaypointPathRenderSystem - Construct - Receive AllDoneSettingAsset");
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
