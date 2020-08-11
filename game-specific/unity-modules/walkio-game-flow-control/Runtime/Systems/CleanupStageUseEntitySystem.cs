namespace JoyBrick.Walkio.Game.FlowControl
{
    using System;
    using Command;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;

    // using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class CleanupStageUseEntitySystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(CleanupStageUseEntitySystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private EndInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _removeStageUseEventQuery;

        public void Construct()
        {
            _logger.Debug($"Module - CleanupStageUseEntitySystem - Construct");
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
            _removeStageUseEventQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(RemoveStageUse) }
            });

            RequireForUpdate(_removeStageUseEventQuery);
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var removeStageUseEventQuery = _removeStageUseEventQuery.GetSingletonEntity();

            Entities
                .WithAll<StageUse>()
                .ForEach((Entity entity) =>
                {
                    commandBuffer.DestroyEntity(entity);
                })
                .WithoutBurst()
                .Run();

            Entities
                .WithAll<StageUse, Disabled>()
                .ForEach((Entity entity) =>
                {
                    commandBuffer.DestroyEntity(entity);
                })
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
