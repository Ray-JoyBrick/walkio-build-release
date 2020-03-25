using System;
using Unity.Entities;

[Serializable]
public struct CubeRotationSpeed : IComponentData
{
    public float value;
}