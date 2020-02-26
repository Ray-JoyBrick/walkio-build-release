namespace JoyBrick.Walkio.Game.Main
{
    using Unity.Entities;
    using UnityEngine;

    public class CharacterAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            throw new System.NotImplementedException();
        }
    }
}