namespace JoyBrick.Walkio.Game.Move.Waypoint
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;
    
    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public class WaypointPathBlobAssetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public List<GameCommon.Template.WaypointPath> waypointPaths;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
        }
    }
}
