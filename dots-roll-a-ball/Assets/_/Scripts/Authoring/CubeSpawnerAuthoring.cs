using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
public class CubeSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField] private GameObject cubePrefab = default;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(cubePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new CubeSpawnerData
        {
            number = 24,
            radius = 7.5f,
            cubePrefabEntity = conversionSystem.GetPrimaryEntity(cubePrefab)
        };

        dstManager.AddComponentData(entity, spawnerData);
    }
}
