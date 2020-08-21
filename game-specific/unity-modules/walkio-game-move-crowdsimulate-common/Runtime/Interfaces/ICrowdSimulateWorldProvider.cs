namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using UnityEngine;

    public interface ICrowdSimulateWorldProvider
    {
        ScriptableObject CrowdSimulateWorldData { get; }
    }
}
