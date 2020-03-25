using System;
using Unity.Entities;

[Serializable]
public struct CubeSpawnerData : IComponentData
{
    public float radius;
    public int number;
    public Entity cubePrefabEntity;
}