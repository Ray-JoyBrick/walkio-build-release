namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;

    [RequiresEntityConversion]
    [ConverterVersion("ray", 1)]
    public class UnitSpawnerAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity,
        IDeclareReferencedPrefabs
    {
        public GameObject prefab;
        public int countX;
        public int countY;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var unitSpawner = new UnitSpawner
            {
                prefab = conversionSystem.GetPrimaryEntity(prefab),
                countX = countX,
                countY = countY
            };

            dstManager.AddComponentData(entity, unitSpawner);
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(prefab);
        }
    }
}