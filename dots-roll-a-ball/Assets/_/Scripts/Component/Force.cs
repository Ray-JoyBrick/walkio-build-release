using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Force : IComponentData
{
    public float3 direction;
    public float magnitude;
}