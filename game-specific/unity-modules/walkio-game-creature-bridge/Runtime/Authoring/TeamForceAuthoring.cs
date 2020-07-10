namespace JoyBrick.Walkio.Game.Creature
{
    using Unity.Entities;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public class TeamForceAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity,
        
        GameCommon.IGroupingProvider
    {
        public int teamId;

        #region Section of realizatoin of IGroupingProvider

        public int GroupingId => teamId;

        #endregion

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TeamForce
            {
                TeamId = teamId
            });
        }
    }
}

