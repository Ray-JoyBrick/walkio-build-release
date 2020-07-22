namespace JoyBrick.Walkio.Game.Move.Waypoint
{
    using Unity.Entities;
    using UnityEngine;

    public class WaypointMoveAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new WaypointMoveIndication());
        }        
    }
}
