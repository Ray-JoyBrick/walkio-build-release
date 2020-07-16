namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using UnityEngine;

    public interface IFlowFieldWorldProvider
    {
        // This entity should have the following components
        // - FlowFieldWorld
        // - FlowFieldWorldProperty
        Entity FlowFieldWorldEntity { get; set; }
        
        GameObject FlowFieldTileBlobAssetAuthoringPrefab { get; set; }
    }
}
