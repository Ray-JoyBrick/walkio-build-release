namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;

    //
    // using GameCommon = JoyBrick.Walkio.Game.Common;

    public class FlowFieldMoveAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        // public bool inInCharge;
        public bool toBeChased;
        public int belongToGroup;
        public bool syncFromMonoBehaviorWorld;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new FlowFieldMoveIndication());
            // // Need to assign group id to the component at this time or later.
            // dstManager.AddComponentData(entity, new FlowFieldGroup());
            //
            // if (inInCharge)
            // {
            //     var groupingProvider = GetComponent<GameCommon.IGroupingProvider>();
            //
            //     var groupId = groupingProvider?.GroupingId ?? 0;
            //     //
            //     dstManager.AddComponentData(entity, new GameCommon.MakeMoveSpecificSetup());
            //     dstManager.AddComponentData(entity, new GameCommon.MakeMoveSpecificSetupProperty
            //     {
            //         FlowFieldMoveSetup = false,
            //
            //         TeamId = groupId
            //     });
            //
            //     //
            //     dstManager.AddComponentData(entity, new MonitorTileChange());
            //     dstManager.AddComponentData(entity, new MonitorTileChangeProperty
            //     {
            //         CanMonitor = false
            //     });
            //     dstManager.AddComponentData(entity, new MoveToTarget());
            // }

            if (toBeChased)
            {
                dstManager.AddComponentData(entity, new ToBeChasedTarget());
                dstManager.AddComponentData(entity, new ToBeChasedTargetProperty
                {
                    BelongToGroup = belongToGroup
                });
            }
            else
            {
                dstManager.AddComponentData(entity, new ChaseTarget());
                dstManager.AddComponentData(entity, new ChaseTargetProperty
                {
                    BelongToGroup = belongToGroup
                });
            }

            if (syncFromMonoBehaviorWorld)
            {
                dstManager.AddComponentData(entity, new CopyTransformFromGameObject());
            }
        }
    }
}
