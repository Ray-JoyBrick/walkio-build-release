namespace JoyBrick.Walkio.Game.GameFlowControl
{
    using System;
    using Command;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;

    using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class CleanupStageUseEntitySystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _removeStageUseEventQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
            _removeStageUseEventQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(GameCommon.RemoveStageUse) }
            });
            
            RequireForUpdate(_removeStageUseEventQuery);
        }

        protected override void OnUpdate()
        {
             var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
             var removeStageUseEventQuery = _removeStageUseEventQuery.GetSingletonEntity();
             
             Entities
                 .WithAll<GameCommon.StageUse>()
                 .ForEach((Entity entity) =>
                 {
                     commandBuffer.DestroyEntity(entity);
                 })
                 .WithoutBurst()
                 .Run();

             Entities
                 .WithAll<GameCommon.StageUse, Disabled>()
                 .ForEach((Entity entity) =>
                 {
                     commandBuffer.DestroyEntity(entity);
                 })
                 .WithoutBurst()
                 .Run();
             
             _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
