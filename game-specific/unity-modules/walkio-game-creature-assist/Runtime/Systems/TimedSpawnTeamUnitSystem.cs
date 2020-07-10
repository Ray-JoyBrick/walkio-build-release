namespace JoyBrick.Walkio.Game.Creature.Assist
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCreature = JoyBrick.Walkio.Game.Creature;

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
        
        //
        public GameCommon.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"TimedSpawnTeamUnitSystem - Construct");
            
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"TimedSpawnTeamUnitSystem - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            //
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            
            // _theEnvironmentQuery = GetEntityQuery(new EntityQueryDesc
            // {
            //     All = new ComponentType[] { typeof(TheEnvironment) }
            // });
            //
            // // var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            // // GameObjectConversionUtility.ConvertGameObjectHierarchy(neutralUnitPrefab, settings);
            //
            // RequireForUpdate(_theEnvironmentQuery);
        }
        
        private void CreateEventEntity(
            EntityCommandBuffer entityCommandBuffer,
            EntityArchetype eventEntityArchetype)
        {
            var eventEntity = entityCommandBuffer.CreateEntity(eventEntityArchetype);

            // TODO: Remove this once the creation is timed but event
            int GetTeamId()
            {
                var v = UnityEngine.Random.Range(1, 7);
                var result = v + 10;
                if (result > 15)
                {
                    result = 1;
                }

                return result;
            }
            
            var teamId = GetTeamId();
            var kind = UnityEngine.Random.Range(1, 4);
            var atPosition = new float3(
                UnityEngine.Random.Range(-14.0f, 14.0f),
                0,
                UnityEngine.Random.Range(-14.0f, 14.0f));
            
            _logger.Debug($"TimedSpawnTeamUnitSystem - CreateEventEntity - teamId: {teamId} kind: {kind}");
            
            entityCommandBuffer.SetComponent(eventEntity, new GameCreature.CreateTeamUnitProperty
            {
                TeamId = teamId,
                Kind = kind,
                AtPosition = atPosition
            });
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

                        CreateEventEntity(commandBuffer, eventEntityArchetype);
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
