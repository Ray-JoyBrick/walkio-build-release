namespace JoyBrick.Walkio.Game.Battle
{
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;

    using GameCommon = JoyBrick.Walkio.Game.Common;

    public class TeamLeaderAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new CopyTransformFromGameObject());
            dstManager.AddComponentData<GameCommon.StageUse>(entity, new GameCommon.StageUse());
        }
    }
}
