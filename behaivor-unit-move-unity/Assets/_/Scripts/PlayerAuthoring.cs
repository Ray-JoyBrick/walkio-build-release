namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;
    using UnityEngine;

    public struct Player : IComponentData
    {
    }

    public class PlayerAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<Player>(entity);
        }
    }
}
