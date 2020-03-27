namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;

    // [RequiresEntityConversion]
    // [ConverterVersion("ray", 1)]
    public class UnitAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // dstManager.AddComponentData(entity, new Translation
            // {
            //     Value = float3.zero
            // });
        }
    }
}