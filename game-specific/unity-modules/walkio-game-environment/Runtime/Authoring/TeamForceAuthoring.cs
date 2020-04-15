namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using Unity.Entities;
    using Unity.Transforms;
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

            // dstManager.RemoveComponent<LocalToWorld>(entity);
            // dstManager.RemoveComponent<Translation>(entity);
            // dstManager.RemoveComponent<Rotation>(entity);

            dstManager.AddComponentData(entity, new Team
            {
                Id = id
            });
            dstManager.AddComponentData(entity, new TeamLeader
            {
                Id = id
            });
            dstManager.AddComponentData(entity, new Unit
            {
                Index = 0
            });
            dstManager.AddComponentData(entity, new PathfindGroup
            {
                Index = 0
            });
            // dstManager.SetName(entity, "Team Entity");
        }
    }
}
