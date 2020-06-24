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
    public class TeamUnitToPathSystem : SystemBase
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
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"TeamUnitToPathSystem - Construct - Receive DoneSettingAsset");
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

            FlowFieldChangeStream
                .Subscribe(teamTileContext =>
                {
                    //
                    // _logger.Debug($"TeamUnitToPathSystem - Construct - Handle FlowFieldChangeStream");

                    //
                    UpdateGroupAtiTiles(teamTileContext);
                    RequestAstarPathToSearch(teamTileContext);
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            // _theEnvironmentQuery = GetEntityQuery(new EntityQueryDesc
            // {
            //     All = new ComponentType[] { typeof(GameEnvironment.TheEnvironment) }
            // });
            //
            // RequireForUpdate(_theEnvironmentQuery);
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            Entities
                .WithAll<FlowFieldTileChange>()
                .ForEach((Entity entity, FlowFieldTileChangeProperty flowFieldTileChangeProperty) =>
                {
                    var teamId = flowFieldTileChangeProperty.TeamId;
                    
                    ResetCachedFlowFieldEntities(commandBuffer, teamId);
                    
                    _notifyTileChange.OnNext(new TeamTileContext
                    {
                        TeamId = flowFieldTileChangeProperty.TeamId,
                        TimeTick = flowFieldTileChangeProperty.TimeTick,
                        TargetPosition = flowFieldTileChangeProperty.TargetPosition
                    });
                    // var units = GetComponentDataFromEntity<Unit>();

                    commandBuffer.DestroyEntity(entity);
                })
                // .Schedule();
                .WithoutBurst()
                .Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
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
                commandBuffer.AddComponent(entity, new DiscardedFlowFieldTile());
            }
            
            table.Clear();
        }

        private void UpdateGroupAtiTiles(TeamTileContext teamTileContext)
        {
            //
            // List<int> teamList = null;
            TeamAtTileInfo teamAtTileInfo = null;
            var hasTeamList = _teamAtTiles.TryGetValue(teamTileContext.TeamId, out teamAtTileInfo);
            if (!hasTeamList)
            {
                _teamAtTiles.Add(teamTileContext.TeamId, new TeamAtTileInfo());
            }
            
            _teamAtTiles[teamTileContext.TeamId].Reset();
            
            //
            Entities
                .WithAll<MoveOnFlowFieldTile>()
                // .WithNone<GameEnvironment.TeamLeader>()
                .ForEach((Entity entity, Translation translation, FlowFieldGroup flowFieldGroup) =>
                {
                    var groupdId = flowFieldGroup.GroupId;
                    // _logger.Debug($"TeamUnitToPathSystem - UpdateTeamAtiTiles - teamId: {teamId} inTeamId {inTeamId}");
                    if (groupdId == teamTileContext.TeamId)
                    {
                        var tileIndex = GetTileIndex(translation.Value);
                        
                        var existed = _teamAtTiles[groupdId].TileIndices.Exists(x => x == tileIndex);
                        if (!existed)
                        {
                            _teamAtTiles[groupdId].TileIndices.Add(tileIndex);
                        }                            
                    }
                })
                .WithoutBurst()
                .Run();
            
            
            
            // foreach (var pair in _teamAtTiles)
            // {
            //     var teamId = pair.Key;
            //     var tileIndices = pair.Value;
            //
            //     var desc = tileIndices.Aggregate("", (acc, next) => $"{acc}, {next}");
            //     _logger.Debug($"TeamUnitToPathSystem - UpdateTeamAtiTiles - teamId: {teamId} at {desc} tiles");
            // }
        }

        private void RequestAstarPathToSearch(TeamTileContext teamTileContext)
        {
            // No need to check if key existed, as it should at this moment
            var teamAtTileInfo = _teamAtTiles[teamTileContext.TeamId];
            var startPoints = MapTileIndicesToPositions(teamAtTileInfo.TileIndices);

            //
            AStarPathService.CalculatePath(teamTileContext.TeamId, teamTileContext.TimeTick, startPoints, teamTileContext.TargetPosition, HandleFoundPaths);
        }

        private void HandleFoundPaths(int teamId, int timeTick, Vector3 targetPosition, List<List<Vector3>> paths)
        {
            _logger.Debug($"TeamUnitToPathSystem - HandleFoundPaths - For teamId: {teamId} timeTick: {timeTick} targetPosition: {targetPosition}");

            var hGridCellCount = 128;
            var vGridCellCount = 192;
             paths.ForEach(path =>
             {
                 var tileIndices = GetTileIndicesFromPath(hGridCellCount, vGridCellCount, path);
                 var entities = FromTileIndicesToFlowFieldTileEntities(
                     EntityManager,
                     _cachedEntities,
                     teamId,
                     timeTick,
                     targetPosition, tileIndices);
             });
        }

        private static List<Vector3> MapTileIndicesToPositions(List<int> tileIndices)
        {
            // TODO: Remove fake calculation
            var result = tileIndices.Select(tileIndex =>
            {
                var v = tileIndex / 5;
                var h = tileIndex % 5;

                return new Vector3(h * 10 + 5, 0, v * 10 + 5);
            }).ToList();

            return result;
        }

        private static List<int> GetTileIndicesFromPath(
            int hGridCellCount, int vGridCellCount,
            List<Vector3> path)
        {
            // TODO: Remove fake calculation
            var indices = new List<int>();
            // return new List<int> { };
            path.ForEach(position =>
            {
                var tileIndex =
                    Utility.PathTileHelper.TileIndex1d(
                        hGridCellCount, vGridCellCount, 1.0f, 1.0f, 
                        10, 10, 1.0f, 1.0f, 
                        position.x, position.z);

                var existed = indices.Exists(x => x == tileIndex);
                if (!existed)
                {
                    indices.Add(tileIndex);
                }
            });

            return indices;
        }

        private static List<Entity> FromTileIndicesToFlowFieldTileEntities(
            EntityManager entityManager,
            Dictionary<int, Dictionary<int, Entity>> cachedEntities,
            int teamId, int timeTick,
            Vector3 targetPosition, List<int> tileIndices)
        {
            // var entityArchetype = entityManager.CreateArchetype(
            //     typeof(GameEnvironment.FlowFieldTile));
            var entityArchetype = entityManager.CreateArchetype(
                typeof(FlowFieldTile),
                typeof(FlowFieldTileCellBuffer),
                typeof(FlowFieldTileInCellBuffer),
                typeof(FlowFieldTileOutCellBuffer));

            // This is the part where flow field tile entity is created, should avoid duplication for
            // the same tile entity that is created previously
            var flowFieldEntities = tileIndices.Select(tileIndex =>
            {
                var entity = Entity.Null;
                
                var table = cachedEntities[teamId];
                if (table.ContainsKey(tileIndex))
                {
                    // Cached, just use it
                    // May have some issue if just use previously defined entity
                    entity = table[tileIndex];
                }
                else
                {
                    entity = entityManager.CreateEntity(entityArchetype);

                    SetupFlowFieldTileEntity(entityManager, teamId, timeTick, targetPosition, table, tileIndex, entity);

                    table.Add(tileIndex, entity);
                }
            
                return entity;
            }).ToList();

            flowFieldEntities.Reverse();
            
            for (var i = 0; i < flowFieldEntities.Count; ++i)
            {
                if (i != flowFieldEntities.Count - 1)
                {
                    var flowFieldEntity = flowFieldEntities[i];
                    var nextFlowFieldEntity = flowFieldEntities[i + 1];
                
                    entityManager.SetComponentData(flowFieldEntity, new FlowFieldTile
                    {
                        HorizontalCount = 10,
                        VerticalCount = 10,
                        TimeTick = timeTick,
                        NextFlowFieldTile = nextFlowFieldEntity
                    });
                }
            }

            return flowFieldEntities;
        }

        public enum EDirection
        {
            Top,
            Right,
            Down,
            Left
        }
        
        private static void SetupFlowFieldTileEntity(EntityManager entityManager, int teamId, int timeTick, Vector3 targetPosition, Dictionary<int, Entity> cachedEntities, int tileIndex, Entity entity)
        {
            _logger.Debug($"TeamUnitToPathSystem - SetupFlowFieldTileEntity - for teamId: {teamId} targetPosition: {targetPosition} entity: {entity}");
            
            // Actual flow field direction setup here

            entityManager.SetComponentData(entity, new FlowFieldTile
            {
                Index = tileIndex,
                            
                HorizontalCount = 10,
                VerticalCount = 10,
                            
                TimeTick = timeTick                        
            });
                    
            var tileBuffer = entityManager.AddBuffer<FlowFieldTileCellBuffer>(entity);
            var tileCellInBuffer = entityManager.AddBuffer<FlowFieldTileInCellBuffer>(entity);
            var tileCellOutBuffer = entityManager.AddBuffer<FlowFieldTileOutCellBuffer>(entity);
        
            var totalTileCellCount = 10 * 10;
            var tileCellInCount = 10 * 4;
            var tileCellOutCount = 10 * 4;
                    
            tileBuffer.ResizeUninitialized(totalTileCellCount);
            tileCellInBuffer.ResizeUninitialized(tileCellInCount);
            tileCellOutBuffer.ResizeUninitialized(tileCellOutCount);

            // For now, just random the direction for each cell
            
            var seed = new System.Random();
            var rnd = new Unity.Mathematics.Random((uint)seed.Next());

            for (var tv = 0; tv < 10; ++tv)
            {
                for (var th = 0; th < 10; ++th)
                {
                    var tileCellIndex = tv * 10 + th;

                    tileBuffer[tileCellIndex] = rnd.NextInt(0, 8);
                }
            }

            for (var i = 0; i < 10 * 4; ++i)
            {
                tileCellInBuffer[i] = -1;
            }

            for (var i = 0; i < 10 * 4; ++i)
            {
                tileCellOutBuffer[i] = -1;
            }
            


            //
            var hTileCount = 10;
            var vTileCount = 10;

            var topTileIndex = GetAdjacentTileIndex(hTileCount, vTileCount, tileIndex, EDirection.Top);
            var rightTileIndex = GetAdjacentTileIndex(hTileCount, vTileCount, tileIndex, EDirection.Right);
            var downTileIndex = GetAdjacentTileIndex(hTileCount, vTileCount, tileIndex, EDirection.Down);
            var leftTileIndex = GetAdjacentTileIndex(hTileCount, vTileCount, tileIndex, EDirection.Left);

            if (topTileIndex != -1)
            {
                // Has top tile
                var hasTopTile = cachedEntities.ContainsKey(topTileIndex);

                if (hasTopTile)
                {
                    var outTile = cachedEntities[topTileIndex];
                    ModifyFlowFieldTileInOutCell(outTile, entity);
                }
            }

            if (rightTileIndex != -1)
            {
                // Has right tile
                var hasRightTile = cachedEntities.ContainsKey(rightTileIndex);

                if (hasRightTile)
                {
                    var outTile = cachedEntities[rightTileIndex];
                    ModifyFlowFieldTileInOutCell(outTile, entity);
                }
            }

            if (downTileIndex != -1)
            {
                // Has down tile
                var hasDownTile = cachedEntities.ContainsKey(downTileIndex);

                if (hasDownTile)
                {
                    var outTile = cachedEntities[downTileIndex];
                    ModifyFlowFieldTileInOutCell(outTile, entity);
                }
            }

            if (leftTileIndex != -1)
            {
                // Has left tile
                var hasLeftTile = cachedEntities.ContainsKey(leftTileIndex);

                if (hasLeftTile)
                {
                    var outTile = cachedEntities[leftTileIndex];
                    ModifyFlowFieldTileInOutCell(outTile, entity);
                }
            }
        }

        // This is post process for flow field tile
        private static void ModifyFlowFieldTileInOutCell(Entity outTile, Entity inTile)
        {
            
        }

        private static int GetAdjacentTileIndex(
            int hTileCount, int vTileCount,
            int comparedIndex, EDirection direction)
        {
            var index = -1;

            var hComparedIndex = comparedIndex % hTileCount;
            var vComparedIndex = comparedIndex / vTileCount;

            if (direction == EDirection.Top)
            {
                var topIndex = vComparedIndex + 1;
                index = (topIndex < vTileCount) ? topIndex : -1;
            }
            else if (direction == EDirection.Right)
            {
                var rightIndex = hComparedIndex + 1;
                index = (rightIndex < hTileCount) ? rightIndex : -1;
            }
            else if (direction == EDirection.Down)
            {
                var downIndex = vComparedIndex - 1;
                index = (downIndex >= 0) ? downIndex : -1;
            }
            else if (direction == EDirection.Left)
            {
                var leftIndex = hComparedIndex - 1;
                index = (leftIndex >= 0) ? leftIndex : -1;
            }

            return index;
        }
        
        // protected override void OnUpdate()
        // {
        //     if (!_canUpdate) return;
        //     
        //     var theEnvironmentEntity = _theEnvironmentQuery.GetSingletonEntity();
        //     var levelWaypointPathLookup = EntityManager.GetComponentData<GameEnvironment.LevelWaypointPathLookup>(theEnvironmentEntity);
        //
        //     var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
        //     var concurrentCommandBuffer = commandBuffer.ToConcurrent();
        //
        //     var deltaTime = Time.DeltaTime;
        //     
        //     _teamAtTiles.Clear();
        //
        //     Entities
        //         .WithAll<GameEnvironment.MoveOnFlowFieldTile>()
        //         .WithNone<GameEnvironment.TeamLeader>()
        //         .ForEach((Entity entity, Translation translation, TeamForce teamForce) =>
        //         {
        //             var tileIndex = GetTileIndex(translation.Value);
        //
        //             List<int> teamList = null;
        //             var hasTeamList = _teamAtTiles.TryGetValue(teamForce.TeamId, out teamList);
        //             if (!hasTeamList)
        //             {
        //                 _teamAtTiles.Add(tileIndex, new List<int>());
        //             }
        //
        //             var existed = _teamAtTiles[tileIndex].Exists(x => x == tileIndex);
        //             if (!existed)
        //             {
        //                 _teamAtTiles[tileIndex].Add(tileIndex);
        //             }
        //
        //             //
        //         })
        //         // .Schedule();
        //         .WithoutBurst()
        //         .Run();
        //
        //     foreach (var pair in _teamAtTiles)
        //     {
        //         var teamId = pair.Key;
        //         var tileIndices = pair.Value;
        //
        //         var desc = tileIndices.Aggregate("", (acc, next) => $"{acc}, {next}");
        //         // _logger.Debug($"teamId: {teamId} at {desc} tiles");
        //     }
        //     
        //     _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        // }
        
        private static int GetTileIndex(float3 pos)
        {
            var a =
                Utility.PathTileHelper.PositionToTileAndTileCellIndex2d(
                    128, 192,
                    1.0f, 1.0f,
                    10, 10,
                    1.0f, 1.0f,
                    pos.x, pos.z);
                
            return a.x;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
