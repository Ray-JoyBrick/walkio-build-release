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
        public int startPathIndex;
        public int endPathIndex;

        public Vector3 startingPosition;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<NeutralForce>(entity, new NeutralForce());

            transform.position = new Vector3(startingPosition.x, startingPosition.y, startingPosition.z);


            //
            dstManager.AddComponentData(entity, new GameCommon.StageUse());
        }
    }
}
