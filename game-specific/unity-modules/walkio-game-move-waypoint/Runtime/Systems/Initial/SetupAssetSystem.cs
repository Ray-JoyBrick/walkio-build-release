namespace JoyBrick.Walkio.Game.Move.Waypoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

#if WALKIO_LEVEL
    using GameLevel = JoyBrick.Walkio.Game.Level;
#endif

    //
#if WALKIO_FLOWCONTROL
    [GameFlowControl.DoneSettingAssetWait("Stage")]
#endif
    [DisableAutoCreation]
    public class SetupAssetSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupAssetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _gridWorldEntityQuery;
        // private EntityQuery _levelSettingEntityQuery;
        private EntityArchetype _tileEntityArchetype;

        //
        private bool _canSetup;
        private bool _doingSetup;

        //
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        private async Task Setup()
        {
            _logger.Debug($"Module - Move - Waypoint - SetupAssetSystem - Setup");
        }

        private void SettingAsset()
        {
            //
            Setup().ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    _canSetup = false;

#if WALKIO_FLOWCONTROL
                    FlowControl?.FinishIndividualSettingAsset(new GameFlowControl.FlowControlContext
                    {
                        Name = "Stage"
                    });
#endif
                })
                .AddTo(_compositeDisposable);
        }

        //
        public void Construct()
        {
            _logger.Debug($"Module - Move - Waypoint - SetupAssetSystem - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.AssetSettingStarted
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Move - Waypoint - SetupAssetSystem - Construct - Receive SettingAsset");

                    _canSetup = true;
                    _doingSetup = true;

                    SettingAsset();
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            _logger.Debug($"Module - Move - Waypoint - SetupAssetSystem - OnCreate");

            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            _gridWorldEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
#if WALKIO_LEVEL
                    ComponentType.ReadOnly<GameLevel.GridWorld>(),
                    ComponentType.ReadOnly<GameLevel.GridWorldProperty>()
#endif
                }
            });

            RequireForUpdate(_gridWorldEntityQuery);
        }

        protected override void OnUpdate()
        {
            if (!_canSetup) return;

            if (!_doingSetup) return;

            // var levelSettingEntity = _levelSettingEntityQuery.GetSingletonEntity();
            // var buffer = EntityManager.GetBuffer<GameCommon.FlowFieldTileBuffer>(levelSettingEntity);

            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
