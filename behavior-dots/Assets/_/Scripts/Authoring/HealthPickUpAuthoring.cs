namespace JoyBrick.Walkio.Game.Main
{
    using Unity.Entities;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class HealthPickUpAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float restoredAmount;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new HealthPickup {restoredAmount = restoredAmount});
        }
    }
}