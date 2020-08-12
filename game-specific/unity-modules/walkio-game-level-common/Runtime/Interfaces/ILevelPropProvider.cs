namespace JoyBrick.Walkio.Game.Level
{
    using UniRx;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public interface ILevelPropProvider
    {
        Scene LevelAtScene { get; set; }

        Camera LevelCamera { get; set; }
        GameObject MainPlayerVirtualCamera { get; set; }
        
        GameObject HUDNavigationSystemGO { get; set; }
        GameObject HUDNavigationHudGO { get; set; }
        
        //
        ReactiveDictionary<int, int> TeamForceUnitCounts { get; }

        void SetupFollowingCamera(GameObject playerGo);

        void SetupPlayerToHUDNavigationSystem(GameObject playerGo);
    }
}
