namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
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

        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            _logger.Debug($"SetupMoveSpecificSystem - Construct");

            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"SetupMoveSpecificSystem - Construct - Receive DoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
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
