namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using System.Collections.Generic;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    public interface IFlowFieldWorldProvider
    {
        ScriptableObject FlowFieldWorldData { get; }

        // This entity should have the following components
        // - FlowFieldWorld
        // - FlowFieldWorldProperty
        Entity FlowFieldWorldEntity { get; set; }

        GameObject FlowFieldTileBlobAssetAuthoringPrefab { get; set; }

        void CalculateLeadingTilePath(int forWhichGroup, Entity forWhichLeader, float3 targetPosition, List<float3> positions);
    }
}
