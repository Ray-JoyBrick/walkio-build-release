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
    using GameMove = JoyBrick.Walkio.Game.Move;
    
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

        //
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _theEnvironmentQuery;

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
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            Entities
                .WithAll<MoveOnFlowFieldTile>()
                // .ForEach((Entity entity, MoveOnFlowFieldTileProperty moveOnFlowFieldTileProperty, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass, ref Translation translation) =>
                .ForEach((Entity entity, ref MoveOnFlowFieldTileProperty moveOnFlowFieldTileProperty) =>
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
                        // Need a way to get the cell index instead of hard code it to 0 here
                        var cellValue = buffer[0];

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

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
