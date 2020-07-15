namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    // using Unity.Physics;
    // using Unity.Physics.Extensions;
    // using Unity.Physics.Systems;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;
    
    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameMove = JoyBrick.Walkio.Game.Move;
    
    [DisableAutoCreation]
    // [UpdateBefore(typeof(FixedUpdate))]
    // [UpdateAfter(typeof(StepPhysicsWorld))]
    [UpdateAfter(typeof(AssignFlowFieldTileToTeamUnitSystem))]
    // [UpdateAfter(typeof(ExportPhysicsWorld))]
    public class MoveOnFlowFieldTileSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(MoveOnFlowFieldTileSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _entityQuery;

        //
        private bool _canUpdate;

        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            _logger.Debug($"MoveOnFlowFieldTileSystem - Construct");

            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"MoveOnFlowFieldTileSystem - Construct - Receive DoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            
            _entityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(MonitorTileChange), typeof(MoveToTarget)
                }
            });
            
            RequireForUpdate(_entityQuery);
        }
        
        // protected override void OnUpdate()
        // {
        //     if (!_canUpdate) return;
        //     
        //     var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
        //     var concurrentCommandBuffer = commandBuffer.ToConcurrent();
        //
        //     var deltaTime = Time.DeltaTime;
        //
        //     using (var moveToTargetEntities = _entityQuery.ToEntityArray(Allocator.TempJob))
        //     {
        //         for (var i = 0; i < moveToTargetEntities.Length; ++i)
        //         {
        //             var moveToTargetEntity = moveToTargetEntities[i];
        //             var moveToTarget = EntityManager.GetComponentData<MoveToTarget>(moveToTargetEntity);
        //
        //             var baseFlowFieldGroup = EntityManager.GetComponentData<FlowFieldGroup>(moveToTargetEntity);
        //
        //             var atTileEntity = moveToTarget.AtTile;
        //             if (atTileEntity != Entity.Null)
        //             {
        //                 var atTile = EntityManager.GetComponentData<FlowFieldTile>(atTileEntity);
        //                 var tileIndex = atTile.Index;
        //             
        //                 Entities
        //                     .WithAll<MoveOnFlowFieldTile>()
        //                     .ForEach((Entity entity, LocalToWorld localToWorld, FlowFieldGroup flowFieldGroup, 
        //                         ref Translation translation, ref Rotation rotation, ref MoveOnFlowFieldTileProperty moveOnFlowFieldTileProperty) =>
        //                     {
        //                         if (flowFieldGroup.GroupId == baseFlowFieldGroup.GroupId)
        //                         {
        //                             var xBase = tileIndex % 4;
        //                             var zBase = tileIndex / 4;
        //
        //                             var xPos = (xBase * 8.0f) - 16.0f;
        //                             var zPos = (zBase * 8.0f) - 16.0f;
        //                         
        //                             var targetPos = new float3(xPos, 0, zPos);
        //                             var direction = targetPos - localToWorld.Position;
        //                             var normalizedDirection = math.normalize(direction);
        //                             moveOnFlowFieldTileProperty.Direction = normalizedDirection;
        //                             
        //                             // _logger.Debug($"MoveOnFlowFieldTileSystem - OnUpdate - normalizedDirection: {normalizedDirection}");
        //
        //                             translation.Value += (normalizedDirection * deltaTime * 1.0f);
        //                             
        //                             var smoothedRotation = math.slerp(
        //                                 rotation.Value,
        //                                 quaternion.LookRotationSafe(normalizedDirection, math.up()), 1f - math.exp(-deltaTime));
        //                             rotation.Value = smoothedRotation;                                    
        //                         }
        //                     })
        //                     // .Schedule();
        //                     .WithoutBurst()
        //                     .Run();
        //             }
        //         }
        //     
        //         _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        //     }
        // }        

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();
        
            Entities
                .WithAll<MoveOnFlowFieldTile>()
                // .ForEach((Entity entity, MoveOnFlowFieldTileProperty moveOnFlowFieldTileProperty, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass, ref Translation translation) =>
                .ForEach((Entity entity, Translation translation, ref MoveOnFlowFieldTileProperty moveOnFlowFieldTileProperty) =>
                {
                    var tileEntity = moveOnFlowFieldTileProperty.OnTile;
                    if (tileEntity != Entity.Null)
                    {
                        // _logger.Debug($"MoveOnFlowFieldTileSystem - OnUpdate - {entity} can move on {tileEntity}");
        
                        // This should be a collection of FlowFieldTileCellBuffer in the whole world

                        var hasComp = EntityManager.HasComponent<FlowFieldTileCellBuffer>(entity);
                        if (hasComp)
                        {
                            _logger.Debug($"MoveOnFlowFieldTileSystem - {entity} hasComp: FlowFieldTileCellBuffer");
                        }
                        
                        var lookupBuffer = GetBufferFromEntity<FlowFieldTileCellBuffer>();
                        var flowFieldTileCellBuffers = lookupBuffer[tileEntity];
                        
                        
                        var cellIndex = GetCellIndexFromPosition(translation.Value);
                        var cellValue = flowFieldTileCellBuffers[cellIndex];
                        
                        // _logger.Debug($"MoveOnFlowFieldTileSystem - OnUpdate - {entity} can move on {tileEntity} using {(int)v}");
                        
                        var direction = GameMove.FlowField.Utility.FlowFieldTileHelper.GetDirectionFromIndex(cellValue);
                        // Store the direction to some component and use this direction later when doing crowd sim
                        
                        // physicsVelocity.Linear = direction;
                        
                        moveOnFlowFieldTileProperty.Direction = direction;
                    }
                })
                // .Schedule();
                .WithoutBurst()
                .Run();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        private static int GetCellIndexFromPosition(float3 position)
        {
            // Need a way to get the cell index instead of hard code it to 0 here

            return 0;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
