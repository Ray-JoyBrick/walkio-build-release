namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    
    using GameTemplate = JoyBrick.Walkio.Game.Template;

    public class FreeUnitAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public int id;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Unit
            {
                Index = 0
            });
            dstManager.AddComponentData(entity, new PathfindGroup
            {
                Index = 0
            });
            // dstManager.SetName(entity, "Free Unit Entity");
        }
    }
}
