namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class LevelSetting : ScriptableObject
    {
        //

        //
        public int aiControlCount;
        public GameObject levelSettingAuthoringPrefab;

        [Header("Waypoint Settings")]
        public List<WaypointPath> waypointPaths;
        // public WaypointPath waypointPath;
        public GameObject waypointPathAuthoringPrefab;
        
        //
        public List<Vector3> spawnPoints;
        
        //
        [Header("Obstacle Grid Settings")]
        public int hGridCount;
        public int vGridCount;

        public int gridCellCount;

        public List<Texture2D> gridTextures;
        public GameObject gridMapAuthoringPrefab;

        [Header("AStar Pathfind Settings")]
        public List<TextAsset> astarGraphDatas;

        //
    }
}
