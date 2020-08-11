namespace JoyBrick.Walkio.Game.Physics.Extension
{
    using System;
    using UniRx;
    using Unity.Entities;
    using Unity.Physics;
    using UnityEngine;

    // using GameCommon = JoyBrick.Walkio.Game.Common;

    //
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;

    [DisableAutoCreation]
    public class CleanupPhysicsConstrainedPairSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(CleanupPhysicsConstrainedPairSystem));

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
                All = new ComponentType[] { typeof(GameFlowControl.RemoveStageUse) }
            });

            RequireForUpdate(_removeStageUseEventQuery);
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var removeStageUseEventQuery = _removeStageUseEventQuery.GetSingletonEntity();

            Entities
                .WithAll<PhysicsConstrainedBodyPair>()
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
