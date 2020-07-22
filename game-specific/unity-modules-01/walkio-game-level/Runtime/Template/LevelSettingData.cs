namespace JoyBrick.Walkio.Game.Level.Template
{
    using System.Collections.Generic;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    [CreateAssetMenu(fileName = "Level Setting Data", menuName = "Walkio/Game/Level/Template/Level Setting Data")]
    public class LevelSettingData : ScriptableObject
    {
        [Header("General Settings")]
        public string title;
        
        [Header("Waypoint Path Settings")]
        public List<GameCommon.Template.WaypointPath> waypointPaths;
        public GameObject WaypointPathBlobAssetAuthoringPrefab;
    }
}
