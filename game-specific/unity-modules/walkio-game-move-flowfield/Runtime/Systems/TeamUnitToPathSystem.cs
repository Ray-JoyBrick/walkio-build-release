namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;
    
    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public class TeamTileContext
    {
        public int TeamId { get; set; } 
        public Vector3 TargetPosition { get; set; }
        public int TimeTick { get; set; }
    }

    public class TeamAtTileInfo
    {
        public List<int> TileIndices { get; set; }
        public Vector3 TargetPosition { get; set; }
        public int TimeTick { get; set; }

        public TeamAtTileInfo()
        {
            TileIndices = new List<int>();
        }

        public void Reset()
        {
            TileIndices.Clear();
            TargetPosition = Vector3.zero;
            TimeTick = 0;
        }
    }
    
    [DisableAutoCreation]
    public partial class TeamUnitToPathSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(TeamUnitToPathSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _theEnvironmentQuery;

        //
        private bool _canUpdate;
        
        //
        private bool _needToDoTileDistribution;

        private readonly Dictionary<int, TeamAtTileInfo> _teamAtTiles = new Dictionary<int, TeamAtTileInfo>();
        
        //
        private readonly Dictionary<int, Dictionary<int, Entity>> _cachedEntities = new Dictionary<int, Dictionary<int, Entity>>();
        
        //
        private IObservable<TeamTileContext> FlowFieldChangeStream => _notifyTileChange.AsObservable();
        private readonly Subject<TeamTileContext> _notifyTileChange = new Subject<TeamTileContext>();
        
        public GameCommon.IFlowControl FlowControl { get; set; }
        public GameCommon.IAStarPathService AStarPathService { get; set; }

        public Dictionary<int, Dictionary<int, Entity>> CachedEntities => _cachedEntities;

        public void Construct()
        {
            _logger.Debug($"TeamUnitToPathSystem - Construct");

            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"TeamUnitToPathSystem - Construct - Receive DoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            _logger.Debug($"TeamUnitToPathSystem - OnCreate");
            
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        private void ResetCachedFlowFieldEntities(EntityCommandBuffer commandBuffer, int teamId)
        {
            Dictionary<int, Entity> entityTable = null;
            var hasValue = _cachedEntities.TryGetValue(teamId, out entityTable);
            if (!hasValue)
            {
                _cachedEntities.Add(teamId, new Dictionary<int, Entity>());
            }

            var table = _cachedEntities[teamId];
            
            // There are cached entities, need to discard these entities
            foreach (var pair in table)
            {
                var entity = pair.Value;
                if (entity != Entity.Null)
                {
                    commandBuffer.AddComponent(entity, new DiscardedFlowFieldTile());
                }
                else
                {
                    _logger.Warning($"TeamUnitToPathSystem - ResetCachedFlowFieldEntities - teamId: {teamId} has null entity in cache");
                }
            }
            
            table.Clear();
        }

        private void UpdateGroupAtiTiles(TeamTileContext teamTileContext)
        {
            //
            TeamAtTileInfo teamAtTileInfo = null;
            var hasTeamList = _teamAtTiles.TryGetValue(teamTileContext.TeamId, out teamAtTileInfo);
            if (!hasTeamList)
            {
                _teamAtTiles.Add(teamTileContext.TeamId, new TeamAtTileInfo());
            }
            
            _teamAtTiles[teamTileContext.TeamId].Reset();
            
            Entities
                .WithAll<MoveOnFlowFieldTile>()
                // .WithNone<GameEnvironment.TeamLeader>()
                .ForEach((Entity entity, Translation translation, FlowFieldGroup flowFieldGroup) =>
                {
                    var groupdId = flowFieldGroup.GroupId;
                    // _logger.Debug($"TeamUnitToPathSystem - UpdateTeamAtiTiles - teamId: {teamId} inTeamId {inTeamId}");
                    // Why is groupId checked with teamId?
                    // Is groupId equal to teamId at some context?
                    if (groupdId == teamTileContext.TeamId)
                    {
                        var tileIndex = GetTileIndex(translation.Value);
                        
                        var existed = _teamAtTiles[groupdId].TileIndices.Exists(x => x == tileIndex);
                        if (!existed)
                        {
                            // This assigns newly discovered tile index into _teamAtTiles cache table
                            _teamAtTiles[groupdId].TileIndices.Add(tileIndex);
                        }                            
                    }
                })
                // Have to be in main thread to avoid issue
                .WithoutBurst()
                .Run();
        }

        private void HandleFoundPaths(int teamId, int timeTick, Vector3 targetPosition, List<List<Vector3>> paths)
        {
            // _logger.Debug($"TeamUnitToPathSystem - HandleFoundPaths - For teamId: {teamId} timeTick: {timeTick} targetPosition: {targetPosition}");

            // TODO: Avoid the hard code value here
            var hGridCellCount = 32;
            var vGridCellCount = 32;
            var uniformSize = 10;

            // There are several paths, each path has several points
            paths.ForEach(path =>
            {
                var tileIndices = GetTileIndicesFromPath(hGridCellCount, vGridCellCount, path);
                var entities = FromTileIndicesToFlowFieldTileEntities(
                    EntityManager,
                    _cachedEntities,
                    teamId, timeTick, targetPosition,
                    tileIndices,
                    uniformSize);
            });
        }
        
        private void RequestAstarPathToSearch(TeamTileContext teamTileContext)
        {
            // No need to check if key existed, as it should at this moment
            var teamAtTileInfo = _teamAtTiles[teamTileContext.TeamId];
            var startPoints = MapTileIndicesToPositions(teamAtTileInfo.TileIndices);

            // No need to use async here, just use sync one
            AStarPathService.CalculatePath(
                teamTileContext.TeamId, teamTileContext.TimeTick,
                startPoints,
                teamTileContext.TargetPosition,
                HandleFoundPaths);
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            // Receive flow field tile change event entity
            Entities
                .WithAll<FlowFieldTileChange>()
                .ForEach((Entity entity, FlowFieldTileChangeProperty flowFieldTileChangeProperty) =>
                {
                    // Whenever the flow field tile changes, clean the cache first
                    var teamId = flowFieldTileChangeProperty.TeamId;
                    ResetCachedFlowFieldEntities(commandBuffer, teamId);

                    //
                    var teamTileContext = new TeamTileContext
                    {
                        TeamId = flowFieldTileChangeProperty.TeamId,
                        TimeTick = flowFieldTileChangeProperty.TimeTick,
                        TargetPosition = flowFieldTileChangeProperty.TargetPosition
                    };

                    //
                    UpdateGroupAtiTiles(teamTileContext);
                    RequestAstarPathToSearch(teamTileContext);

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
