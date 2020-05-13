namespace JoyBrick.Walkio.Game.Battle
{
    using Unity.Entities;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public class UnitAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<Unit>(entity, new Unit());

            // For cleanup use
            dstManager.AddComponentData<GameCommon.StageUse>(entity, new GameCommon.StageUse());
            
#if UNITY_EDITOR

            var neutralForceAuthoring = GetComponent<NeutralForceAuthoring>();
            var teamForceAuthoring = GetComponent<TeamForceAuthoring>();
            var teamLeaderAuthoring = GetComponent<TeamLeaderAuthoring>();

            if (teamLeaderAuthoring != null)
            {
                // Let Team Leader Authoring do the naming
            }
            else if (teamForceAuthoring != null)
            {
                dstManager.SetName(entity, "Team Unit");
            }
            else if (neutralForceAuthoring != null)
            {
                dstManager.SetName(entity, "Neutral Unit");
            }

#endif
        }
    }
}
