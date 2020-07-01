namespace JoyBrick.Walkio.Game.Move.CrowdSim
{
    using UniRx;
    using Unity.Entities;
    using Unity.Physics.Systems;
    
    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    //
    [DisableAutoCreation]
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class CrowdSimSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(CrowdSimSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        //
        private bool _canUpdate;
        
        //
        public GameCommon.IFlowControl FlowControl { get; set; }
        public bool UseComputeShader { get; set; }
        
        public void Construct()
        {
            _logger.Debug($"CrowdSimSystem - Construct");

            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"CrowdSimSystem - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        private void UpdateUsingComputeShader()
        {
            
        }

        private void UpdateUsingEcs()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            Entities
                .WithAll<Particle>()
                // .ForEach((Entity entity, MoveOnFlowFieldTileProperty moveOnFlowFieldTileProperty, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass, ref Translation translation) =>
                .ForEach((Entity entity, ParticleProperty particleProperty) =>
                    // .ForEach((Entity entity, int entityInQueryIndex, ParticleProperty particleProperty) =>
                {
                })
                // .Schedule();
                .WithoutBurst()
                .Run();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            if (UseComputeShader)
            {
                UpdateUsingComputeShader();
            }
            else
            {
                UpdateUsingEcs();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
}
