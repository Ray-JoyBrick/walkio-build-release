namespace JoyBrick.Walkio.Game.Creature
{
    using Unity.Entities;
    using UnityEngine;

    public class NeutralUnitSpawnAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public float intervalMax;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new NeutralUnitSpawn());
            dstManager.AddComponentData(entity, new NeutralUnitSpawnProperty
            {
                IntervalMax = intervalMax,
                CountDown = 0
            });
        }
    }
}
