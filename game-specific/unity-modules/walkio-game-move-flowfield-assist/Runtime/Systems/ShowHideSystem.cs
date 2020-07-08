namespace JoyBrick.Walkio.Game.Move.FlowField.Assist
{
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    //
    [DisableAutoCreation]
    public class ShowHideSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(ShowHideSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _entityQuery;

        private bool _canUpdate;

        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            _logger.Debug($"ShowHideSystem - Construct");

            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"ShowHideSystem - Construct - Receive DoneSettingAsset");
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
                    typeof(FlowFieldReactToShowHide),
                },
            });
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();
            
            // var slowFieldReactToShowHides = _entityQuery.ToComponentDataArray<FlowFieldReactToShowHide>(Allocator.TempJob)
            var slowFieldReactToShowHideEntities = _entityQuery.ToEntityArray(Allocator.Temp);
            
            Entities
                .WithAll<FlowFieldTileShowHide>()
                .ForEach((Entity entity, FlowFieldTileShowHideProperty flowFieldTileShowHideProperty) =>
                {
                    for (var i = 0; i < slowFieldReactToShowHideEntities.Length; ++i)
                    {
                        var ent = slowFieldReactToShowHideEntities[i];
                        if (ent != Entity.Null)
                        {
                            if (EntityManager.HasComponent<MeshRenderer>(ent))
                            {
                                var meshRenderer = EntityManager.GetComponentObject<MeshRenderer>(ent);
                                meshRenderer.enabled = !flowFieldTileShowHideProperty.Hide;
                            }
                        }
                    }
                    
                    commandBuffer.DestroyEntity(entity);
                })
                // .Schedule();
                .WithoutBurst()
                .Run();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

            if (slowFieldReactToShowHideEntities.IsCreated)
            {
                slowFieldReactToShowHideEntities.Dispose();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
