namespace JoyBrick.Walkio.Game.Main
{
    using Unity.Entities;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class CharacterAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        //
        public Character CharacterData;
        public Health HealthData;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            HealthData.Value = HealthData.MaxValue;
            
            //
            dstManager.AddComponentData(entity, CharacterData);
            dstManager.AddComponentData(entity, HealthData);
            dstManager.AddComponentData(entity, new CharacterInputs());            
        }
    }
}