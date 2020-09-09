namespace JoyBrick.Walkio.Game.Level
{
    using Unity.Entities;
    using UnityEngine;

    public interface IGridWorldProvider
    {
        // This entity should have the following components
        // - GridWorld
        // - GridWorldProperty
        Entity GridWorldEntity { get; set; }

        ScriptableObject GridWorldData { get; }
    }
}
