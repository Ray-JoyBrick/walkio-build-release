namespace JoyBrick.Walkio.Game.Ranking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Cinemachine;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCreature = JoyBrick.Walkio.Game.Creature;

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    public class RankContext
    {
        public int Id { get; set; }
        public int PreviousScore { get; set; }
        public int CurrentScore { get; set; }
    }

    //
    [DisableAutoCreation]
    public class SystemR01 : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SystemR01));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        //
        private bool _canUpdate;

        private readonly Dictionary<int, RankContext>  _rankContextTable = new Dictionary<int, RankContext>();

        //
        // public GameCreature.ICreatureProvider CreatureProvider { get; set; }
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        public void Construct()
        {
            _logger.Debug($"Module - Ranking - SystemR01 - Construct");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Ranking - SystemR01 - Construct - Receive FlowReadyToStart");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);

            //
            FlowControl?.AssetUnloadingStarted
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Ranking - SystemR01 - Construct - Receive AssetUnloadingStarted");

                    _rankContextTable.Clear();

                    _canUpdate = false;
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            _logger.Debug($"Module - Ranking - SystemR01 - OnCreate");

            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            Entities
                .WithAll<AdjustScore>()
                .ForEach((Entity entity, AdjustScoreProperty adjustScoreProperty) =>
                {
                    RankContext rankContext = null;
                    var gotRankContext = _rankContextTable.TryGetValue(adjustScoreProperty.Id, out rankContext);
                    if (!gotRankContext)
                    {
                        _rankContextTable.Add(adjustScoreProperty.Id, new RankContext
                        {
                            Id = adjustScoreProperty.Id,
                            PreviousScore = 0,
                            CurrentScore = 0
                        });
                    }

                    _logger.Debug($"Module - Ranking - SystemR01 - OnUpdate - add score: {adjustScoreProperty.Score} to id: {adjustScoreProperty.Id}");

                    _rankContextTable[adjustScoreProperty.Id].PreviousScore =
                        _rankContextTable[adjustScoreProperty.Id].CurrentScore;
                    _rankContextTable[adjustScoreProperty.Id].CurrentScore = adjustScoreProperty.Score;

                    commandBuffer.DestroyEntity(entity);
                })
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
