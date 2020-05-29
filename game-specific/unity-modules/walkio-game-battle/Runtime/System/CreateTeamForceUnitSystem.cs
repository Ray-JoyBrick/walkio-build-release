namespace JoyBrick.Walkio.Game.Battle
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;

    //
    using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;

    //
    public struct CreateTeamForceUnitEvent : IComponentData
    {
    }

    public struct TeamForceUnitCreationContext : IComponentData
    {
        public int TeamId;
        public float3 CreateAtLocation;
        public float3 FaceDirection;
    }

    [DisableAutoCreation]
    public class CreateTeamForceUnitSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(CreateTeamForceUnitSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityArchetype _eventEntityArchetype;
        private EntityArchetype _entityArchetype;

        //
        private bool _canUpdate;

        public GameCommand.ICommandService CommandService { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"CreateTeamForceUnitSystem - Construct - Receive DoneSettingAsset");
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

            //
            CommandService.CommandStream
                .Where(x => (x as GameCommand.CreateTeamForceUnit) != null)
                .Subscribe(x =>
                {
                    _logger.Debug($"CreateTeamForceUnitSystem - Construct - Receive CreateTeamForceUnit");
                    var createTeamForceUnit = (x as GameCommand.CreateTeamForceUnit);

                    CreateEventEntity(createTeamForceUnit.TeamId);

                })
                .AddTo(_compositeDisposable);
        }

        private void CreateEventEntity(int teamId)
        {
            //
            var entity = EntityManager.CreateEntity(_eventEntityArchetype);
            EntityManager.SetComponentData(entity, new TeamForceUnitCreationContext
            {
                TeamId = teamId
            });
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            _eventEntityArchetype = EntityManager.CreateArchetype(
                typeof(CreateTeamForceUnitEvent),
                typeof(TeamForceUnitCreationContext));
            _entityArchetype = EntityManager.CreateArchetype(
                typeof(TeamForce),
                typeof(GameEnvironment.Unit));
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var loadBattleSystem = World.GetExistingSystem<LoadBattleSystem>();
            var teamForceUnitPrefab = loadBattleSystem.TeamForceUnitPrefab;

            Entities
                .WithAll<CreateTeamForceUnitEvent>()
                .ForEach((Entity entity, TeamForceUnitCreationContext teamForceUnitCreationContext) =>
                {
                    // var createdEntity = commandBuffer.CreateEntity(_entityArchetype);
                    //
                    // commandBuffer.SetComponent(createdEntity, new TeamForce
                    // {
                    //     TeamId = teamForceUnitCreationContext.TeamId
                    // });
                    
                    // commandBuffer.Instantiate(teamForceUnitPrefab)
                    var teamForceAuthoring = teamForceUnitPrefab.GetComponent<TeamForceAuthoring>();
                    if (teamForceAuthoring != null)
                    {
                        teamForceAuthoring.teamId = teamForceUnitCreationContext.TeamId;
                    }
                    GameObject.Instantiate(teamForceUnitPrefab);
                    
                    //
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
