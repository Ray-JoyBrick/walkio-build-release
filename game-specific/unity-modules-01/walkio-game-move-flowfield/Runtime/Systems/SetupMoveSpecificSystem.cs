namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;

    //
#if WALKIO_COMMON
    using GameCommon = JoyBrick.Walkio.Game.Common;
#endif
    // using GameEnvironment = JoyBrick.Walkio.Game.Environment;
    
    
    [DisableAutoCreation]
    public class SetupMoveSpecificSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupMoveSpecificSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        //
        private bool _canUpdate;

#if WALKIO_COMMON
        public GameCommon.IFlowControl FlowControl { get; set; }
#endif

        public void Construct()
        {
            _logger.Debug($"SetupMoveSpecificSystem - Construct");
#if WALKIO_COMMON
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"SetupMoveSpecificSystem - Construct - Receive DoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            //
            Entities
                .WithAll<GameCommon.MakeMoveSpecificSetup>()
                .ForEach((Entity entity, ref GameCommon.MakeMoveSpecificSetupProperty makeMoveSpecificSetupProperty, ref FlowFieldGroup flowFieldGroup) =>
                {
                    var teamId = makeMoveSpecificSetupProperty.TeamId;

                    // _logger.Debug($"SetupMoveSpecificSystem - OnUpdate - teamId: {teamId}");

                    if (!makeMoveSpecificSetupProperty.FlowFieldMoveSetup)
                    {
                        commandBuffer.SetComponent(entity, new GameCommon.MakeMoveSpecificSetupProperty
                        {
                            TeamId = teamId,
                            FlowFieldMoveSetup = true
                        });
                        // makeMoveSpecificSetupProperty.FlowFieldMoveSetup = true;
                        // flowFieldGroup.GroupId = teamId;
                        commandBuffer.SetComponent(entity, new FlowFieldGroup
                        {
                            GroupId = teamId
                        });
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
