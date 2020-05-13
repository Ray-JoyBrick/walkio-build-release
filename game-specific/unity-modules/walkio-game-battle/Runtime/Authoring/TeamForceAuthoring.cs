namespace JoyBrick.Walkio.Game.Battle
{
    using Unity.Entities;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public struct PickupAbsorbable : IComponentData
    {
    }

    public struct NeutralForceUnitAbsorbable : IComponentData
    {
    }

    public struct AbsorbPickup : IComponentData
    {
    }

    public struct AbsorbNeutralForceUnit : IComponentData
    {
    }

    public struct AbsorbablePickup : IComponentData
    {
        public int Id;
        public bool Interacted;
    }

    public struct AbsorbableNeutralForceUnit : IComponentData
    {
    }

    public class TeamForceAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TeamForce());

            // //
            // dstManager.AddComponentData(entity, new PickupAbsorbable());
            // dstManager.AddComponentData(entity, new NeutralForceUnitAbsorbable());

            //
            dstManager.AddComponentData(entity, new GameCommon.StageUse());
        }
    }
}
