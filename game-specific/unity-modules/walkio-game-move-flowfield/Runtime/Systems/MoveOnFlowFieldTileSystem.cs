namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Physics.Extensions;
    using Unity.Physics.Systems;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;
    
    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    
    [DisableAutoCreation]
    // [UpdateBefore(typeof(FixedUpdate))]
    // [UpdateAfter(typeof(StepPhysicsWorld))]
    [UpdateAfter(typeof(AssignFlowFieldTileToTeamUnitSystem))]
    [UpdateAfter(typeof(ExportPhysicsWorld))]
    public class MoveOnFlowFieldTileSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(MoveOnFlowFieldTileSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _theEnvironmentQuery;

        private bool _canUpdate;
        
        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"MoveOnFlowFieldTileSystem - Construct - Receive DoneSettingAsset");
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
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            var deltaTime = Time.DeltaTime;

            Entities
                .WithAll<MoveOnFlowFieldTile>()
                .ForEach((Entity entity, MoveOnFlowFieldTileProperty moveOnFlowFieldTileProperty, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass, ref Translation translation) =>
                {
                    var tileEntity = moveOnFlowFieldTileProperty.OnTile;
                    if (tileEntity != Entity.Null)
                    {
                        // On some tile, use it
                        // translation.Value
                        // _logger.Debug($"MoveOnFlowFieldTileSystem - OnUpdate - {entity} can move on {tileEntity}");

                        // var flowFieldTile = flowFieldTileComps[tileEntity];
                        // GetBufferFromEntity<GameEnvironment.FlowFieldTileBuffer>();
                        
                        var lookup = GetBufferFromEntity<FlowFieldTileCellBuffer>();
                        var buffer = lookup[tileEntity];
                        var v = buffer[0];

                        // _logger.Debug($"MoveOnFlowFieldTileSystem - OnUpdate - {entity} can move on {tileEntity} using {(int)v}");

                        var direction = GetDirectionFromIndex(v);
                        physicsVelocity.Linear = direction;
                    }
                })
                // .Schedule();
                .WithoutBurst()
                .Run();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        private static float3 GetDirectionFromIndex(int index)
        {
            var direction = float3.zero;
            if (index == 0)
            {
                direction = new float3(0, 0, 1.0f);
            }
            else if (index == 1)
            {
                direction = new float3(0.5f, 0, 0.5f);
            }
            else if (index == 2)
            {
                direction = new float3(1.0f, 0, 0f);
            }
            else if (index == 3)
            {
                direction = new float3(0.5f, 0, -0.5f);
            }
            else if (index == 4)
            {
                direction = new float3(0, 0, -1.0f);
            }
            else if (index == 5)
            {
                direction = new float3(-0.5f, 0, -0.5f);
            }
            else if (index == 6)
            {
                direction = new float3(-1.0f, 0, 0);
            }
            else if (index == 7)
            {
                direction = new float3(-0.5f, 0, 0.5f);
            }

            return direction;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
