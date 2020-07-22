namespace Game
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;

    public class WaypointPathBlobAssetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public Template.WaypointData waypointDataAsset;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
        }
    }
    
}
