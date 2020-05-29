namespace JoyBrick.Walkio.Game.Battle
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;
    
    //
    public struct FlowFieldTileChange : IComponentData
    {
    }

    public struct FlowFieldTileChangeInfo : IComponentData
    {
        public int TeamId;
        public int ToTileIndex;
        public Entity ToTileEntity;
        public int TimeTick;
        public float3 TargetPosition;
    }
    
    [DisableAutoCreation]
    public class AdjustMoveToTargetFlowFieldSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(AdjustMoveToTargetFlowFieldSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _theEnvironmentQuery;
        private EntityArchetype _flowFieldTileArchetype;

        //
        private bool _canUpdate;

        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"AdjustMoveToTargetFlowFieldSystem - Construct - Receive DoneSettingAsset");
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
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            _theEnvironmentQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(GameEnvironment.TheEnvironment) }
            });

            _flowFieldTileArchetype = EntityManager.CreateArchetype(
                typeof(GameEnvironment.FlowFieldTile));

            
            RequireForUpdate(_theEnvironmentQuery);
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            // var theEnvironmentEntity = _theEnvironmentQuery.GetSingletonEntity();
            // var levelWaypointPathLookup = EntityManager.GetComponentData<GameEnvironment.LevelWaypointPathLookup>(theEnvironmentEntity);

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var deltaTime = Time.DeltaTime;
            
            // Need to get time tick from some system
            var timeTick = (int) (Time.ElapsedTime * 100);

            var flowFieldTileComps = GetComponentDataFromEntity<GameEnvironment.FlowFieldTile>();
            
            var flowFieldTileArchetype = EntityManager.CreateArchetype(
                typeof(GameEnvironment.FlowFieldTile));
            
            var flowFieldTileChangeEventArchetype = EntityManager.CreateArchetype(
                typeof(FlowFieldTileChange),
                typeof(FlowFieldTileChangeInfo)
            );

            Entities
                .WithAll<GameEnvironment.TeamLeader, MonitorTileChange>()
                .ForEach((Entity entity, LocalToWorld localToWorld, TeamForce teamForce,  ref MoveToTarget moveToTarget) =>
                {
                    if (moveToTarget.AtTile == Entity.Null)
                    {
                        // This is the case where team leader is just created, no entity assigned

                        var tileIndex = GetTileIndex(localToWorld.Position);
                        var flowFieldTileEntity = AssignToMoveToTarget(commandBuffer, flowFieldTileArchetype, tileIndex, timeTick, entity);

                        //
                        SendFlowFieldChangeEvent(commandBuffer, flowFieldTileChangeEventArchetype,
                            tileIndex, timeTick, teamForce.TeamId, localToWorld.Position, flowFieldTileEntity);
                    }
                    else
                    {
                        // Check to see if team leader is moving to another tile, update at tile
                        // and signal

                        var tileIndex = GetTileIndex(localToWorld.Position);
                        var flowFieldTileComp = flowFieldTileComps[moveToTarget.AtTile];

                        if (tileIndex != flowFieldTileComp.Index)
                        {
                            // _logger.Debug($"AdjustMoveToTargetFlowFieldSystem - OnUpdate - tileIndex: {tileIndex} flowFieldTileComp Index: {flowFieldTileComp.Index}");

                            var previousEntity = moveToTarget.AtTile;
                            var flowFieldTileEntity = AssignToMoveToTarget(commandBuffer, flowFieldTileArchetype, tileIndex, timeTick, entity);

                            //
                            SendFlowFieldChangeEvent(commandBuffer, flowFieldTileChangeEventArchetype,
                                tileIndex, timeTick, teamForce.TeamId, localToWorld.Position, flowFieldTileEntity);

                            commandBuffer.AddComponent<GameEnvironment.DiscardedFlowFieldTile>(previousEntity);
                        }
                        else
                        {
                            // Still in the same tile, does not need to create new tile nor send change event
                        }
                    }
                })
                // .Schedule();
                .WithoutBurst()
                .Run();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        private Entity AssignToMoveToTarget(EntityCommandBuffer commandBuffer, EntityArchetype flowFieldTileArchetype,
            int tileIndex, int timeTick, Entity entity)
        {
            var flowFieldTileEntity = commandBuffer.CreateEntity(flowFieldTileArchetype);
            var tileBuffer = commandBuffer.AddBuffer<GameEnvironment.FlowFieldTileCellBuffer>(flowFieldTileEntity);
            var tileCellInBuffer = commandBuffer.AddBuffer<GameEnvironment.FlowFieldTileInCellBuffer>(flowFieldTileEntity);
            var tileCellOutBuffer = commandBuffer.AddBuffer<GameEnvironment.FlowFieldTileOutCellBuffer>(flowFieldTileEntity);
                        
            //
            commandBuffer.SetComponent(flowFieldTileEntity, new GameEnvironment.FlowFieldTile
            {
                Index = tileIndex,
                            
                HorizontalCount = 10,
                VerticalCount = 10,
                            
                TimeTick = timeTick
            });

            var totalTileCellCount = 10 * 10;
            var tileCellInCount = 10 * 4;
            var tileCellOutCount = 10 * 4;
                        
            tileBuffer.ResizeUninitialized(totalTileCellCount);
            tileCellInBuffer.ResizeUninitialized(tileCellInCount);
            tileCellOutBuffer.ResizeUninitialized(tileCellOutCount);
                        
            for (var tv = 0; tv < 10; ++tv)
            {
                for (var th = 0; th < 10; ++th)
                {
                    var tileCellIndex = tv * 10 + th;

                    tileBuffer[tileCellIndex] = tileCellIndex;
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
            // moveToTarget.AtTile = flowFieldTileEntity;
            commandBuffer.SetComponent(entity, new MoveToTarget
            {
                AtTile = flowFieldTileEntity
            });

            return flowFieldTileEntity;
        }

        private void SendFlowFieldChangeEvent(EntityCommandBuffer commandBuffer, EntityArchetype flowFieldTileChangeEventArchetype,
            int tileIndex, int timeTick, int teamId, float3 targetPosition, Entity toTileEntity)
        {
            _logger.Debug($"AdjustMoveToTargetFlowFieldSystem - SendFlowFieldChangeEvent - targetPosition: {targetPosition}");
            
            var flowFieldTileChangeEvent = commandBuffer.CreateEntity(flowFieldTileChangeEventArchetype);
            commandBuffer.SetComponent(flowFieldTileChangeEvent, new FlowFieldTileChangeInfo
            {
                TeamId = teamId,
                ToTileIndex = tileIndex,
                ToTileEntity = toTileEntity,
                TimeTick = timeTick,
                TargetPosition = targetPosition
            });            
        }

        private static int GetGridCellIndex(float3 pos)
        {
            return 0;
        }

        private static int GetTileIndex(float3 pos)
        {
            var a =
                GameEnvironment.Utility.PathTileHelper.PositionToTileAndTileCellIndex2d(
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
