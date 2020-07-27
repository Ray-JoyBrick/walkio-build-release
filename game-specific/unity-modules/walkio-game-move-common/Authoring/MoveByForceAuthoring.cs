namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;

    //
    // using GameCommon = JoyBrick.Walkio.Game.Common;

    public class MoveByForceAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        // public bool inInCharge;
        public bool toBeChased;
        public int belongToGroup;
        public bool syncFromMonoBehaviorWorld;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new MoveByForce());
        }
    }
}
