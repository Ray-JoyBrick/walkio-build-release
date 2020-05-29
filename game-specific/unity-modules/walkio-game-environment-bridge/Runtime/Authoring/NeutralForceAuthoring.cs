namespace JoyBrick.Walkio.Game.Environment.Creature
{
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public class NeutralForceAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<NeutralForce>(entity, new NeutralForce());

            //
            dstManager.AddComponentData(entity, new GameCommon.StageUse());

#if UNITY_EDITOR
            var entityName = dstManager.GetName(entity);
            dstManager.SetName(entity, $"{entityName} Neutral");
#endif
        }
    }
}
