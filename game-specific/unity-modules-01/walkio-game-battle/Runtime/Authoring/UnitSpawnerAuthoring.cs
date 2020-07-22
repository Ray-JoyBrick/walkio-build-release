namespace JoyBrick.Walkio.Game.Battle
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;

    public class UnitSpawnerAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity,
        IDeclareReferencedPrefabs
    {
        public GameObject unitPrefab;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<UnitSpawner>(entity, new UnitSpawner
            {
                Unit = conversionSystem.GetPrimaryEntity(unitPrefab)
            });
            dstManager.AddComponentData<UnitSpawnStyle>(entity, new UnitSpawnStyle
            {
                IntervalMax = 3.0f,
                CountDown = 0
            });
            dstManager.AddBuffer<UnitBuffer>(entity);
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(unitPrefab);
        }
    }
}