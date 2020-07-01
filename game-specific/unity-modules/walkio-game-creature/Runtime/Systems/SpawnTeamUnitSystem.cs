namespace JoyBrick.Walkio.Game.Creature
{
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class SpawnTeamUnitSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SpawnNeutralUnitSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _theEnvironmentQuery;
        
        //
        private float _countDown = 5.0f;

        private List<Entity> _prefabEntities = new List<Entity>();
        
        //
        private bool _canUpdate;
        
        public GameCommon.IFlowControl FlowControl { get; set; }
        public GameCommon.IEcsSettingProvider EcsSettingProvider { get; set; }

        public List<GameObject> TeamUnitPrefabs { get; set; }
        
        public void Construct()
        {
            _logger.Debug($"SpawnTeamUnitSystem - Construct");            
            // var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            // _prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(teamUnitPrefab, settings);
            
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"SpawnTeamUnitSystem - Construct - Receive AllDoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            // _theEnvironmentQuery = GetEntityQuery(new EntityQueryDesc
            // {
            //     All = new ComponentType[] { typeof(TheEnvironment) }
            // });
            
            // RequireForUpdate(_theEnvironmentQuery);
        }
        
        // private void CreateTeamForceUnit(GameObject prefab)
        private void CreateTeamForceUnit(
            EntityCommandBuffer entityCommandBuffer,
            Entity prefab,
            int teamId,
            int kind,
            float3 position)
        {
            var createdEntity = entityCommandBuffer.Instantiate(prefab);
            // var startingPosition = new float3(
            //     UnityEngine.Random.Range(-14.0f, 14.0f),
            //     0,
            //     UnityEngine.Random.Range(-14.0f, 14.0f));
            
            entityCommandBuffer.SetComponent(createdEntity, new Translation
            {
                Value = position
            });
            
            entityCommandBuffer.SetComponent(createdEntity, new TeamForce
            {
                TeamId = teamId
            });
            
            // entityCommandBuffer.SetComponent(ent, new PhysicsMass
            // {
            //     InverseMass = new float3(0, 0, 0)
            // });
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            //
            // var environmentEntity = _theEnvironmentQuery.GetSingletonEntity();
            // var levelWaypointPathLookup = EntityManager.GetComponentData<LevelWaypointPathLookup>(environmentEntity);
            // var neutralUnitSpawn = EntityManager.GetComponentData<NeutralUnitSpawn>(environmentEntity);

            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            //
            var deltaTime = Time.DeltaTime;

            // if (_prefabEntity == Entity.Null)
            if (!_prefabEntities.Any())
            {
                TeamUnitPrefabs.ForEach(x =>
                {
                    var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(x, EcsSettingProvider.GameObjectConversionSettings);
                    _prefabEntities.Add(entity);
                });
            }

            var prefabEntities = _prefabEntities;
            // var prefabEntity = _prefabEntity;

            //
            Entities
                .WithAll<CreateTeamUnit>()
                .ForEach((Entity entity, CreateTeamUnitProperty createTeamUnitProperty) =>
                {
                    var teamId = createTeamUnitProperty.TeamId;
                    var kind = createTeamUnitProperty.Kind;
                    var position = createTeamUnitProperty.AtPosition;

                    var prefabEntity = prefabEntities[kind];
                    
                    CreateTeamForceUnit(commandBuffer, prefabEntity, teamId, kind, position);

                    //
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
