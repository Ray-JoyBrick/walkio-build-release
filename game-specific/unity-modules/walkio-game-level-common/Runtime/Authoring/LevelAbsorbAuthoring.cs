namespace JoyBrick.Walkio.Game.Level
{
    using Unity.Entities;
    using UnityEngine;

    public class LevelAbsorbAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public bool isAbsorber;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (isAbsorber)
            {
                dstManager.AddComponentData(entity, new LevelAbsorber());
            }
            else
            {
                dstManager.AddComponentData(entity, new LevelAbsorbable());
                // dstManager.AddComponentData(entity, new LevelAbsorbableProperty
                // {
                //
                // });
            }
        }
    }
}
