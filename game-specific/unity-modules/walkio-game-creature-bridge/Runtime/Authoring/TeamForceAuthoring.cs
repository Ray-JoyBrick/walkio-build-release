namespace JoyBrick.Walkio.Game.Creature
{
    using Unity.Entities;
    using UnityEngine;

    public class TeamForceAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public int teamId;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TeamForce
            {
                TeamId = teamId
            });
        }
    }
}

