namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    
    using GameTemplate = JoyBrick.Walkio.Game.Template;

    public class TeamForceAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public int id;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // dstManager.AddComponent<RemoveAfterConversion>(entity);
            // dstManager.AddComponentObject(entity, tileDetailAsset);

            dstManager.AddComponentData<Team>(entity, new Team
            {
                Id = id
            });
        }
    }
}
