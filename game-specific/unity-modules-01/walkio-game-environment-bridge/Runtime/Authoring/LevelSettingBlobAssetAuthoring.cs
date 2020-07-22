namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;
    
    public class LevelSettingBlobAssetAuthoring :
        MonoBehaviour
       
#if !IGNORE_RUNTIME_BEHAVIOR
        
        , IConvertGameObjectToEntity

#endif

    {
        public LevelSetting levelSettingAsset;
        
#if !IGNORE_RUNTIME_BEHAVIOR

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // dstManager.AddComponent<RemoveAfterConversion>(entity);
            // // dstManager.AddComponentObject(entity, tileDetailAsset);
        }

#endif
    }
}
