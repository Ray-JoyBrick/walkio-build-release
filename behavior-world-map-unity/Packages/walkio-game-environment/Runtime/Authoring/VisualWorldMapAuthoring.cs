namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Entities;
    using UnityEngine;

    public class VisualWorldMapAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
        }
    }
}
