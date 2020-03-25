using System;
using Unity.Entities;

[Serializable]
public struct Count : IComponentData
{
    public int value;
}