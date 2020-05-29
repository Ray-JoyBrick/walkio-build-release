namespace JoyBrick.Walkio.Game.Environment.Creature
{
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public class TeamForceAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        // public Vector3 startingPosition;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TeamForce());

            // transform.position = new Vector3(startingPosition.x, startingPosition.y, startingPosition.z);


            //
            dstManager.AddComponentData(entity, new GameCommon.StageUse());

#if UNITY_EDITOR
            var entityName = dstManager.GetName(entity);
            dstManager.SetName(entity, $"{entityName} Team");
#endif

        }
    }
}
