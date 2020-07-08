namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    // using GameEnvironment = JoyBrick.Walkio.Game.Environment;
    
    
    [DisableAutoCreation]
    public class AdjustMoveToTargetFlowFieldSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(AdjustMoveToTargetFlowFieldSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        //
        private bool _canUpdate;

        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            _logger.Debug($"AdjustMoveToTargetFlowFieldSystem - Construct");

            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"AdjustMoveToTargetFlowFieldSystem - Construct - Receive DoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        private Entity AssignToMoveToTarget(
            EntityCommandBuffer commandBuffer, EntityArchetype flowFieldTileArchetype,
            int tileIndex, int timeTick, int uniformSize,
            Entity entity)
        {
            var flowFieldTileEntity = commandBuffer.CreateEntity(flowFieldTileArchetype);
            var tileBuffer = commandBuffer.AddBuffer<FlowFieldTileCellBuffer>(flowFieldTileEntity);
            var tileCellInBuffer = commandBuffer.AddBuffer<FlowFieldTileInCellBuffer>(flowFieldTileEntity);
            var tileCellOutBuffer = commandBuffer.AddBuffer<FlowFieldTileOutCellBuffer>(flowFieldTileEntity);
                        
            //
            commandBuffer.SetComponent(flowFieldTileEntity, new FlowFieldTile
            {
                Index = tileIndex,
                            
                HorizontalCount = uniformSize,
                VerticalCount = uniformSize,
                            
                TimeTick = timeTick
            });

            var totalTileCellCount = uniformSize * uniformSize;
            var tileCellInCount = uniformSize * 4;
            var tileCellOutCount = uniformSize * 4;
                        
            tileBuffer.ResizeUninitialized(totalTileCellCount);
            tileCellInBuffer.ResizeUninitialized(tileCellInCount);
            tileCellOutBuffer.ResizeUninitialized(tileCellOutCount);

            //
            for (var tv = 0; tv < uniformSize; ++tv)
            {
                for (var th = 0; th < uniformSize; ++th)
                {
                    var tileCellIndex = tv * uniformSize + th;

                    tileBuffer[tileCellIndex] = tileCellIndex;
                }
            }

            for (var i = 0; i < uniformSize * 4; ++i)
            {
                tileCellInBuffer[i] = -1;
            }

            for (var i = 0; i < uniformSize * 4; ++i)
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

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;

            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            // Need to get time tick from some system
            var timeTick = (int) (Time.ElapsedTime * 100);

            //
            var flowFieldTileArchetype = EntityManager.CreateArchetype(
                typeof(FlowFieldTile));
            var flowFieldTileChangeEventArchetype = EntityManager.CreateArchetype(
                typeof(FlowFieldTileChange),
                typeof(FlowFieldTileChangeProperty));

            //
            var flowFieldTileComps = GetComponentDataFromEntity<FlowFieldTile>();

            //
            Entities
                .WithAll<MonitorTileChange>()
                .ForEach((Entity entity, LocalToWorld localToWorld, FlowFieldGroup flowFieldGroup, ref MoveToTarget moveToTarget) =>
                {
                    // From entity position to get tile index
                    var tileIndex = GetTileIndex(localToWorld.Position);
                    var uniformSize = 10;

                    if (moveToTarget.AtTile == Entity.Null)
                    {
                        // This is the case where team leader is just created, no entity assigned
                        var flowFieldTileEntity = AssignToMoveToTarget(
                            commandBuffer, flowFieldTileArchetype, tileIndex, timeTick, uniformSize, entity);
                        
                        _logger.Debug($"AdjustMoveToTargetFlowFieldSystem - Update - flowFieldTileEntity: {flowFieldTileEntity}");

                        //
                        SendFlowFieldChangeEvent(commandBuffer, flowFieldTileChangeEventArchetype,
                            tileIndex, timeTick, flowFieldGroup.GroupId, localToWorld.Position, flowFieldTileEntity);
                    }
                    else
                    {
                        // Check to see if team leader is moving to another tile, update at tile
                        // and signal
                        var flowFieldTileComp = flowFieldTileComps[moveToTarget.AtTile];

                        if (tileIndex != flowFieldTileComp.Index)
                        {
                            // _logger.Debug($"AdjustMoveToTargetFlowFieldSystem - OnUpdate - tileIndex: {tileIndex} flowFieldTileComp Index: {flowFieldTileComp.Index}");

                            var previousEntity = moveToTarget.AtTile;
                            var flowFieldTileEntity = AssignToMoveToTarget(commandBuffer, flowFieldTileArchetype, tileIndex, timeTick, uniformSize, entity);

                            //
                            SendFlowFieldChangeEvent(commandBuffer, flowFieldTileChangeEventArchetype,
                                tileIndex, timeTick, flowFieldGroup.GroupId, localToWorld.Position, flowFieldTileEntity);

                            commandBuffer.AddComponent<DiscardedFlowFieldTile>(previousEntity);
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

        private void SendFlowFieldChangeEvent(EntityCommandBuffer commandBuffer, EntityArchetype flowFieldTileChangeEventArchetype,
            int tileIndex, int timeTick, int teamId, float3 targetPosition, Entity toTileEntity)
        {
            // _logger.Debug($"AdjustMoveToTargetFlowFieldSystem - SendFlowFieldChangeEvent - targetPosition: {targetPosition}");
            
            var flowFieldTileChangeEvent = commandBuffer.CreateEntity(flowFieldTileChangeEventArchetype);
            commandBuffer.SetComponent(flowFieldTileChangeEvent, new FlowFieldTileChangeProperty
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
