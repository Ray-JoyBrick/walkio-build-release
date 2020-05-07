namespace JoyBrick.Walkio.Game.ForceHeatmap
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;

    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameBattle = JoyBrick.Walkio.Game.Battle;
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;
    
    [DisableAutoCreation]
    // [UpdateBefore(typeof(FixedUpdate))]
    // [UpdateAfter(typeof(ExportPhysicsWorld))]
    public class CreateUnitHeatmapSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(CreateUnitHeatmapSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
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
                    _logger.Debug($"CreateUnitHeatmapSystem - Construct - Receive DoneSettingAsset");
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

            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var deltaTime = Time.DeltaTime;

            Entities
                .WithAll<GameBattle.Unit>()
                .ForEach((Entity entity, Translation translation) =>
                {
                    // _logger.Debug($"CreateUnitHeatmapSystem - OnUpdate - physicsVelocity: {physicsVelocity.Linear}");
                })
                // .Schedule();
                .WithoutBurst()
                .Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
