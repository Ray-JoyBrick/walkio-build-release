namespace JoyBrick.Walkio.Game.Creature.Assist
{
    using System.Collections.Generic;
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    // using Unity.Physics;
    using Unity.Transforms;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
    using GameCreature = JoyBrick.Walkio.Game.Creature;

#if WALKIO_FLOWCONTROL && WALKIO_CREATURE_ASSIST
    [GameFlowControl.DoneSettingAssetWait("Stage")]
#endif
    [DisableAutoCreation]
    public class TimedSpawnTeamUnitSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(TimedSpawnTeamUnitSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _theEnvironmentQuery;

        //
        private float _countDown = 5.0f;

        private Entity _prefabEntity;

        //
        private bool _canUpdate;
        private EntityQuery _teamUnityQuery;

        //
        public GameFlowControl.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"Module Assist - Creature - TimedSpawnTeamUnitSystem - Construct");

            FlowControl?.AssetSettingStarted
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Creature - PresentUnitIndicationSystem - Construct - Receive SettingAsset");

                    var archetype = EntityManager.CreateArchetype(
                        typeof(TeamUnitSpawnTimer),
                        typeof(TeamUnitSpawnTimerProperty));


                    var entity = EntityManager.CreateEntity(archetype);
                    EntityManager.SetComponentData(entity, new TeamUnitSpawnTimerProperty
                    {
                        IntervalMax = 0.24f,
                        CountDown = 0
                    });
                })
                .AddTo(_compositeDisposable);

            //
            FlowControl?.FlowReadyToStart
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module Assist - Creature - TimedSpawnTeamUnitSystem - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            //
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            _teamUnityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(TeamForce),
                    typeof(JoyBrick.Walkio.Game.Creature.Unit)
                }
            });

            RequireForUpdate(_teamUnityQuery);
        }

        private void CreateEventEntity(
            EntityCommandBuffer commandBuffer,
            EntityArchetype eventEntityArchetype,
            int teamId, float3 atPosition)
        {
            var eventEntity = commandBuffer.CreateEntity(eventEntityArchetype);

            // var teamId = GetTeamId();
            // var kind = UnityEngine.Random.Range(1, 4);
            var kind = 0;

            // _logger.Debug($"Module Assist - Creature - TimedSpawnTeamUnitSystem - CreateEventEntity - teamId: {teamId} kind: {kind}");

            commandBuffer.SetComponent(eventEntity, new GameCreature.CreateTeamUnitProperty
            {
                TeamId = teamId,
                Kind = kind,
                AtPosition = new float3(atPosition.x + 1.0f, atPosition.y, atPosition.z)
            });
        }

        private void CheckForEachTeamForce(
            EntityCommandBuffer commandBuffer,
            EntityArchetype eventEntityArchetype)
        {
            int GetRandomTeamId()
            {
                var v = UnityEngine.Random.Range(0, 2);

                return v;
            }

            var spawnForTeamId = GetRandomTeamId();

            // // var aaa = _teamUnityQuery.CreateArchetypeChunkArray(Allocator.TempJob);
            // // aaa[0].GetChunkComponentData<TeamForce>(aaa);
            //
            // var entities = _teamUnityQuery.ToEntityArray(Allocator.TempJob);
            //
            // entities
            
            // _teamUnityQuery.ToComponentDataArray<LocalToWorld>()

            // using (var inSameTeamUnits = new NativeArray<float3>())
            // {
            var positions = new List<float3>();
            Entities
                .WithStoreEntityQueryInField(ref _teamUnityQuery)
                .ForEach((Entity entity, TeamForce teamForce, LocalToWorld localToWorld) =>
                {
                    // _logger.Debug($"Module Assist - Creature - TimedSpawnTeamUnitSystem - CheckForEachTeamForce - entity: {entity}");
                    // inSameTeamUnits.AddTo(localToWorld.Position);
                    // inSameTeamUnits.
                    
                    positions.Add(localToWorld.Position);
                })
                // .WithDeallocateOnJobCompletion(entities)
                .WithoutBurst()
                .Run();

                
            // }

            var spawnPosition = UnityEngine.Random.Range(0, positions.Count);
            
            CreateEventEntity(
                commandBuffer, eventEntityArchetype,
                spawnForTeamId, spawnPosition);

            // Entities
            //     .WithAll<TeamForce>()
            //     .ForEach((Entity entity, LocalToWorld localToWorld) =>
            //     {
            //         
            //         // var spawnPosition = localToWorld.Position;
            //         //
            //         // CreateEventEntity(
            //         //     commandBuffer, eventEntityArchetype,
            //         //     spawnForTeamId, spawnPosition);
            //     })
            //     .WithoutBurst()
            //     .Run();
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            //
            var deltaTime = Time.DeltaTime;

            var eventEntityArchetype = EntityManager.CreateArchetype(
                typeof(GameCreature.CreateTeamUnit),
                typeof(GameCreature.CreateTeamUnitProperty));

            //
            Entities
                .WithAll<TeamUnitSpawnTimer>()
                // .ForEach((Entity entity, ref TeamUnitSpawnTimerProperty teamUnitSpawnTimerProperty) =>
                .ForEach((Entity entity, TeamUnitSpawnTimerProperty teamUnitSpawnTimerProperty) =>
                {
                    var elapsedTime = teamUnitSpawnTimerProperty.CountDown + deltaTime;

                    teamUnitSpawnTimerProperty.CountDown = elapsedTime;

                    if (elapsedTime >= teamUnitSpawnTimerProperty.IntervalMax)
                    {
                        teamUnitSpawnTimerProperty.CountDown = 0;

                        CheckForEachTeamForce(commandBuffer, eventEntityArchetype);
                    }

                    commandBuffer.SetComponent(entity, teamUnitSpawnTimerProperty);
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
