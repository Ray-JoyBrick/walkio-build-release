namespace JoyBrick.Walkio.Game.Battle
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;
    
    [DisableAutoCreation]
    [UpdateAfter(typeof(AdjustMoveToTargetFlowFieldSystem))]
    public class CleanUpFlowFieldSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(CleanUpFlowFieldSystem));

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
                    _logger.Debug($"CleanUpFlowFieldSystem - Construct - Receive DoneSettingAsset");
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
                .WithAll<GameEnvironment.DiscardedFlowFieldTile>()
                .ForEach((Entity entity) =>
                {
                    commandBuffer.DestroyEntity(entity);
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
