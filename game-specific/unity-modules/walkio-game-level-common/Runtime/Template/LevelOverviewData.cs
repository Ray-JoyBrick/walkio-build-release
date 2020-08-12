namespace JoyBrick.Walkio.Game.Level.Template
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class LevelOverviewDetail
    {
        public int id;
        public string nameId;
    }

    [CreateAssetMenu(fileName = "Level Overview Data", menuName = "Walkio/Game/Level/Level Overview Data")]
    public class LevelOverviewData : ScriptableObject
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Basic")]
#endif
        // Spawn at grid cell index instead of position
        public List<LevelOverviewDetail> levelOverviewDetails;        
    }
}
