namespace JoyBrick.Walkio.Game.Battle
{
    using Unity.Entities;
    using UnityEngine;

    public class UnitAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<Unit>(entity, new Unit());
        }
    }
}