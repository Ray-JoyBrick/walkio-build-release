namespace JoyBrick.Walkio.Game.Battle
{
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public class NeutralForceAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public int startPathIndex;
        public int endPathIndex;

        public float3 startingPosition;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<NeutralForce>(entity, new NeutralForce());

            dstManager.AddComponentData(entity, new MoveOnPath
            {
                StartIndex = startPathIndex,
                EndIndex = endPathIndex,

                AtIndex = startPathIndex
            });

            // var hasTranslationComp = dstManager.HasComponent<Translation>(entity);
            // if (hasTranslationComp)
            // {
            //     dstManager.SetComponentData(entity, new Translation
            //     {
            //         Value = startingPosition
            //     });
            // }
            // else
            // {
            //     dstManager.AddComponentData(entity, new Translation
            //     {
            //         Value = startingPosition
            //     });
            // }
            
            transform.position = new Vector3(startingPosition.x, startingPosition.y, startingPosition.z);
            
            
            //
            dstManager.AddComponentData<GameCommon.StageUse>(entity, new GameCommon.StageUse());
        }
    }
}
