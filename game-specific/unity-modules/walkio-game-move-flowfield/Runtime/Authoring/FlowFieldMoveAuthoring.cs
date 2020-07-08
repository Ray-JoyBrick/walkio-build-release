namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;

    public class FlowFieldMoveAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public bool inInCharge;
        public bool syncFromMonoBehaviorWorld;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new FlowFieldMoveIndication());
            //
            dstManager.AddComponentData(entity, new FlowFieldGroup());

            
            if (inInCharge)
            {
                //
                dstManager.AddComponentData(entity, new MonitorTileChange());
                dstManager.AddComponentData(entity, new MoveToTarget());
            }

            if (syncFromMonoBehaviorWorld)
            {
                dstManager.AddComponentData(entity, new CopyTransformFromGameObject());
            }
        }
    }
}
