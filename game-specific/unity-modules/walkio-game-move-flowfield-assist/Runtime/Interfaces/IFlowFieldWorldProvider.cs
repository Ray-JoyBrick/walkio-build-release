namespace JoyBrick.Walkio.Game.Move.FlowField.Assist
{
    using System.Collections.Generic;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    public interface IFlowFieldWorldProvider
    {
        ScriptableObject FlowFieldWorldData { get; }
    }
}
