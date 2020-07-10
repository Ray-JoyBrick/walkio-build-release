namespace JoyBrick.Walkio.Game.Move.FlowField.Assist
{
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Physics;
    using Unity.Transforms;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameMove = JoyBrick.Walkio.Game.Move;

    [DisableAutoCreation]
    public class AttachFlowFieldTileIndicationSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(AttachFlowFieldTileIndicationSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _entityQuery;

        public GameCommon.IFlowControl FlowControl { get; set; }
        public GameObject NeutralUnitPrefab { get; set; }

        protected override void OnCreate()
        {
            _logger.Debug($"AttachFlowFieldTileIndicationSystem - OnCreate");
    
            base.OnCreate();

            //
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            
            _entityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(GameMove.FlowField.FlowFieldTile),
                    typeof(GameMove.FlowField.FlowFieldTileGroupUse)
                },
                None = new ComponentType[] { typeof(FlowFieldTileIndication) }
            });
            
            RequireForUpdate(_entityQuery);
        }

        protected override void OnUpdate()
        {
            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();
            
            //
            Entities
                .WithAll<GameMove.FlowField.FlowFieldTile>()
                .WithNone<FlowFieldTileIndication>()
                .ForEach((Entity entity) =>
                {
                    // _logger.Debug($"AttachFlowFieldTileIndicationSystem - OnUpdate - Add component tile indication to entity: {entity}");
            
                    commandBuffer.AddComponent(entity, new FlowFieldTileIndication());
                })
                .WithoutBurst()
                .Run();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);


            // var tileEntities = _entityQuery.ToEntityArray(Allocator.TempJob);
            //
            // _logger.Debug($"AttachFlowFieldTileIndicationSystem - OnUpdate - to add comp tiles count: {tileEntities.Length}");
            // for (var i = 0; i < tileEntities.Length; ++i)
            // {
            //     var tileEntity = tileEntities[i];
            //     EntityManager.AddComponentData(tileEntity, new FlowFieldTileIndication());
            // }
            //
            // if (tileEntities.IsCreated)
            // {
            //     tileEntities.Dispose();
            // }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
}
