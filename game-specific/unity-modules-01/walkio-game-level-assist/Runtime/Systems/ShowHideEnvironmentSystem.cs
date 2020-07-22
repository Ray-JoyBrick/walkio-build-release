namespace JoyBrick.Walkio.Game.Level.Assist
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pathfinding;
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Rendering;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;

    //
    [DisableAutoCreation]
    public class ShowHideEnvironmentSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(ShowHideEnvironmentSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _entityQuery;

        //
        private bool _canUpdate;

        //
        public GameCommand.ICommandService CommandService { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }
        
        //
        public void Construct()
        {
            _logger.Debug($"ShowHideEnvironmentSystem - Construct");

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
            _logger.Debug($"ShowHideEnvironmentSystem - OnCreate");

            base.OnCreate();
            
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            
            _entityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    // typeof(ReactToShowHide),
                    // This component is defined to use with auto converted game object
                    typeof(SceneTag)
                },
                Options = EntityQueryOptions.IncludeDisabled
            });
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            // using (var reactToShowHideEntities = _entityQuery.ToEntityArray(Allocator.TempJob))
            // All converted game objects have SceneTag component attached. Use this info to process
            using (var sceneTagEntities = _entityQuery.ToEntityArray(Allocator.TempJob))
            {
                // _logger.Debug($"ShowHideEnvironmentSystem - OnUpdate - reactToShowHideEntities count: {reactToShowHideEntities.Length}");
                var gotShowHideRequest = false;
                var showHide = false;
                var showHideCategory = 0;

                Entities
                    .WithAll<ShowHideRequest>()
                    .ForEach((Entity entity, ShowHideRequestProperty showHideRequestProperty) =>
                    {
                        gotShowHideRequest = true;
                        showHideCategory = showHideRequestProperty.Category;
                        showHide = showHideRequestProperty.Hide;
                    
                        commandBuffer.DestroyEntity(entity);
                    })
                    // .Schedule();
                    .WithoutBurst()
                    .Run();

                if (gotShowHideRequest)
                {
                    // _logger.Debug($"ShowHideEnvironmentSystem - OnUpdate - reactToShowHideEntities.Length: {reactToShowHideEntities.Length} gotShowHideRequest: {gotShowHideRequest}, showHide: {showHide}");
                    _logger.Debug($"ShowHideEnvironmentSystem - OnUpdate - reactToShowHideEntities.Length: {sceneTagEntities.Length} gotShowHideRequest: {gotShowHideRequest}, showHide: {showHide}");

                    // for (var i = 0; i < reactToShowHideEntities.Length; ++i)
                    for (var i = 0; i < sceneTagEntities.Length; ++i)
                    {
                        // var reactToShowHideEntity = reactToShowHideEntities[i];
                        var sceneTagEntity = sceneTagEntities[i];

                        var hasReactToShowHide = EntityManager.HasComponent<ReactToShowHide>(sceneTagEntity);
                        if (!hasReactToShowHide) continue;

                        var reactToShowHide = EntityManager.GetComponentData<ReactToShowHide>(sceneTagEntity);

                        if (reactToShowHide.Category != showHideCategory) continue;

                        var hasRenderMesh = EntityManager.HasComponent<RenderMesh>(sceneTagEntity);
                        // EntityManager.SetEnabled(reactToShowHideEntity, !showHide);
                        if (hasRenderMesh)
                        {
                            // var meshRenderer = EntityManager.GetComponentObject<RenderMesh>(sceneTagEntity);
                            EntityManager.SetEnabled(sceneTagEntity, !showHide);
                        }
                    }
                }
            
                _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
