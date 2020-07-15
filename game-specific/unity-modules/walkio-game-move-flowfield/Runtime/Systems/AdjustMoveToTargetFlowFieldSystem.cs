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
        
        public Common.IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

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
            _logger.Debug($"AdjustMoveToTargetFlowFieldSystem - AssignToMoveToTarget - tileIndex: {tileIndex} timeTick: {timeTick}");

            // Create flow field tile entity
            var flowFieldTileEntity = commandBuffer.CreateEntity(flowFieldTileArchetype);
            var tileBuffer = commandBuffer.AddBuffer<FlowFieldTileCellBuffer>(flowFieldTileEntity);
            var tileCellInBuffer = commandBuffer.AddBuffer<FlowFieldTileInCellBuffer>(flowFieldTileEntity);
            var tileCellOutBuffer = commandBuffer.AddBuffer<FlowFieldTileOutCellBuffer>(flowFieldTileEntity);
                      
#if UNITY_EDITOR
            // This won't work as it is in command buffer stage the sync point won't change name at this time.
            World.DefaultGameObjectInjectionWorld.EntityManager.SetName(flowFieldTileEntity, $"Tile Entity - Group Use");
#endif  

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

            // Assign created flow field tile entity directly back into MoveToTarget AtTile
            commandBuffer.SetComponent(entity, new MoveToTarget
            {
                AtTile = flowFieldTileEntity
            });

            return flowFieldTileEntity;
        }

        private void SendFlowFieldChangeEvent(
            EntityCommandBuffer commandBuffer, EntityArchetype flowFieldTileChangeEventArchetype,
            int tileIndex, int timeTick, int teamId, float3 targetPosition,
            Entity toTileEntity)
        {
            _logger.Debug($"AdjustMoveToTargetFlowFieldSystem - SendFlowFieldChangeEvent - teamId: {teamId} at targetPosition: {targetPosition}");
            
            // Create event entity
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
                typeof(FlowFieldTile),
                typeof(FlowFieldTileGroupUse));
            var flowFieldTileChangeEventArchetype = EntityManager.CreateArchetype(
                typeof(FlowFieldTileChange),
                typeof(FlowFieldTileChangeProperty));

            //
            var flowFieldTileComps = GetComponentDataFromEntity<FlowFieldTile>();

            //
            Entities
                .WithAll<MonitorTileChange>()
                .ForEach((Entity entity, LocalToWorld localToWorld, FlowFieldGroup flowFieldGroup, MonitorTileChangeProperty monitorTileChangeProperty, ref MoveToTarget moveToTarget) =>
                {
                    if (monitorTileChangeProperty.CanMonitor)
                    {
                        // From entity position to get tile index
                        var tileIndex = GetTileIndex(localToWorld.Position);
                        var uniformSize = 10;
                        var atTileEntity = moveToTarget.AtTile;

                        // This is the case where there is not tile entity assigned
                        if (atTileEntity == Entity.Null)
                        {
                            // This is the case where team leader is just created, no entity assigned
                            var flowFieldTileEntity = AssignToMoveToTarget(
                                commandBuffer, flowFieldTileArchetype,
                                tileIndex, timeTick, uniformSize,
                                entity);
                            
                            _logger.Debug($"AdjustMoveToTargetFlowFieldSystem - OnUpdate - flowFieldTileEntity: {flowFieldTileEntity}");

                            // Using groupId as teamId, how is this designed back then?
                            SendFlowFieldChangeEvent(
                                commandBuffer, flowFieldTileChangeEventArchetype,
                                tileIndex, timeTick, flowFieldGroup.GroupId, localToWorld.Position,
                                flowFieldTileEntity);
                        }
                        else
                        {
                            // Check to see if team leader is moving to another tile, update at tile
                            // and signal
                            var flowFieldTileComp = flowFieldTileComps[atTileEntity];
                        
                            if (tileIndex != flowFieldTileComp.Index)
                            {
                                // _logger.Debug($"AdjustMoveToTargetFlowFieldSystem - OnUpdate - tileIndex: {tileIndex} flowFieldTileComp Index: {flowFieldTileComp.Index}");
                        
                                var flowFieldTileEntity = AssignToMoveToTarget(
                                    commandBuffer, flowFieldTileArchetype,
                                    tileIndex, timeTick, uniformSize,
                                    entity);
                        
                                //
                                SendFlowFieldChangeEvent(
                                    commandBuffer, flowFieldTileChangeEventArchetype,
                                    tileIndex, timeTick, flowFieldGroup.GroupId, localToWorld.Position,
                                    flowFieldTileEntity);
                        
                                var previousEntity = atTileEntity;
                                commandBuffer.AddComponent<DiscardedFlowFieldTile>(previousEntity);
                            }
                            else
                            {
                                // Still in the same tile, does not need to create new tile nor send change event
                                // However even in the same tile, the position might not be the same as the last update
                            }
                        }
                    }
                })
                // .Schedule();
                .WithoutBurst()
                .Run();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        private static int GetGridCellIndex(float3 pos)
        {
            return 0;
        }

        private static int GetTileIndex(float3 pos)
        {
            // Merge helper utility function
            var a =
                Utility.PathTileHelper.PositionToTileAndTileCellIndex2d(
                    32, 32,
                    1.0f, 1.0f,
                    -16.0f, -16.0f,
                    10, 10,
                    1.0f, 1.0f,
                    pos.x, pos.z);

            // Debug.Log($"GetTileIndex - a: {a}");
            
            return a.x;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
