namespace JoyBrick.Walkio.Game.Battle
{
    using Unity.Entities;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public class PickupAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public bool canBeAbsorbed;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Pickup());

            if (canBeAbsorbed)
            {
                dstManager.AddComponentData(entity, new AbsorbablePickup());
            }

            // For cleanup use
            dstManager.AddComponentData<GameCommon.StageUse>(entity, new GameCommon.StageUse());

        }
    }
}
