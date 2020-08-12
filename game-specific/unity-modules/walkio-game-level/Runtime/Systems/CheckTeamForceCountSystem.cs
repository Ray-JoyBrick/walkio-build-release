namespace JoyBrick.Walkio.Game.Level
{
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    // using Unity.Physics;
    using Unity.Transforms;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCreature = JoyBrick.Walkio.Game.Creature;
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;

    [DisableAutoCreation]
    public class CheckTeamForceCountSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(CheckTeamForceCountSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _teamUnityQuery;
        
        //
        private bool _canUpdate;
        
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif
        public ILevelPropProvider LevelPropProvider { get; set; }

        public void Construct()
        {
            _logger.Debug($"Module - Creature - CheckTeamForceCountSystem - Construct");            
            
            //
#if WALKIO_FLOWCONTROL
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Creature - CheckTeamForceCountSystem - Construct - Receive FlowReadyToStart");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            _teamUnityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(GameCreature.TeamForce),
                    typeof(GameCreature.Unit)
                }
            });

            RequireForUpdate(_teamUnityQuery);
        }
        
        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.AsParallelWriter();

            //
            var deltaTime = Time.DeltaTime;
            
            var teamForceCounts = new Dictionary<int, int>();

            //
            Entities
                .WithStoreEntityQueryInField(ref _teamUnityQuery)
                .ForEach((Entity entity, GameCreature.TeamForce teamForce, LocalToWorld localToWorld) =>
                {
                    var v = 0;
                    var hasKey = teamForceCounts.TryGetValue(teamForce.TeamId, out v);
                    if (!hasKey)
                    {
                        teamForceCounts.Add(teamForce.TeamId, 0);
                    }

                    teamForceCounts[teamForce.TeamId] += 1;
                })
                // .WithDeallocateOnJobCompletion(entities)
                .WithoutBurst()
                .Run();
            
            // LevelPropProvider.TeamForceUnitCounts.Clear();
            foreach (var pair in teamForceCounts)
            {
                // LevelPropProvider.TeamForceUnitCounts.Add((pair.Key, pair.Value));
                // _logger.Debug($"Module - Creature - CheckTeamForceCountSystem - Update - team[{pair.Key}]: {pair.Value}");
                var hasKey = LevelPropProvider.TeamForceUnitCounts.ContainsKey(pair.Key);
                if (hasKey)
                {
                    LevelPropProvider.TeamForceUnitCounts[pair.Key] = pair.Key;
                }
                else
                {
                    LevelPropProvider.TeamForceUnitCounts.Add(pair.Key, pair.Value);
                }
            }

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency); 
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
}
