namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    
    using GameTemplate = JoyBrick.Walkio.Game.Template;
    using GameEnvironmentBridge = JoyBrick.Walkio.Game.Environment.Bridge;

    [System.Serializable]
    public class TeamTargetRef : IComponentData
    {
        public GameEnvironmentBridge.TeamForce teamForce;
    }

    public class TeamTargetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public GameEnvironmentBridge.TeamForce teamForce; 
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // dstManager.AddComponent<RemoveAfterConversion>(entity);
            // dstManager.AddComponentObject(entity, tileDetailAsset);

            dstManager.AddComponentData<TeamTargetRef>(entity, new TeamTargetRef
            {
                teamForce = teamForce
            });
        }
    }
}
