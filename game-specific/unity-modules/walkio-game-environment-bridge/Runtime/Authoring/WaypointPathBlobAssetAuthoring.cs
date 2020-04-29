namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;
    
    public class WaypointPathBlobAssetAuthoring :
        MonoBehaviour
       
#if !IGNORE_RUNTIME_BEHAVIOR
        
        , IConvertGameObjectToEntity

#endif

    {
        public WaypointData waypointDataAsset;
        
#if !IGNORE_RUNTIME_BEHAVIOR

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // dstManager.AddComponent<RemoveAfterConversion>(entity);
            // // dstManager.AddComponentObject(entity, tileDetailAsset);
            dstManager.AddComponentData<GameCommon.StageUse>(entity, new GameCommon.StageUse());
        }

#endif
    }
}
