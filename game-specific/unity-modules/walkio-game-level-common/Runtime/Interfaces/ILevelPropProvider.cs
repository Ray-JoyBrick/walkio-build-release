namespace JoyBrick.Walkio.Game.Level
{
    using UniRx;
    using UnityEngine;

    public interface ILevelPropProvider
    {
        Camera LevelCamera { get; set; }
        
        //
        ReactiveDictionary<int, int> TeamForceUnitCounts { get; }
    }
}
