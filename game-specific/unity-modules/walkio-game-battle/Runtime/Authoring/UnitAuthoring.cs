namespace JoyBrick.Walkio.Game.Battle
{
    using Unity.Entities;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;

    public class UnitAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<Unit>(entity, new Unit());

            // For cleanup use
            dstManager.AddComponentData<GameCommon.StageUse>(entity, new GameCommon.StageUse());
            
            var neutralForceAuthoring = GetComponent<NeutralForceAuthoring>();
            var teamForceAuthoring = GetComponent<TeamForceAuthoring>();
            var teamLeaderAuthoring = GetComponent<TeamLeaderAuthoring>();

            if (teamLeaderAuthoring != null)
            {
                // Let Team Leader Authoring do the naming
            }
            else if (teamForceAuthoring != null)
            {
                dstManager.AddComponentData(entity, new GameEnvironment.MoveOnFlowFieldTile());
                dstManager.AddComponentData(entity, new GameEnvironment.MoveOnFlowFieldTileInfo());
                
#if UNITY_EDITOR
                dstManager.SetName(entity, "Team Unit");
#endif
            }
            else if (neutralForceAuthoring != null)
            {
#if UNITY_EDITOR
                dstManager.SetName(entity, "Neutral Unit");
#endif
            }
        }
    }
}
