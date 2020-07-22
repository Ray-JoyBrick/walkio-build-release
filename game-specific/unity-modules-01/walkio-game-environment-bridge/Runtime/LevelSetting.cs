namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class LevelSetting : ScriptableObject
    {
        //
        [Header("General Settings")]
        public string title;

        //
        public int aiControlCount;
        public GameObject levelSettingAuthoringPrefab;

        [Header("Waypoint Path Settings")]
        public List<WaypointPath> waypointPaths;
        public GameObject waypointPathAuthoringPrefab;
        
        //
        [Header("Spawn Point Settings")]
        public List<Vector3> spawnPoints;
        
        //
        [Header("Obstacle Grid Settings")]
        public int hGridCount;
        public int vGridCount;

        public int gridCellCount;

        public List<Texture2D> gridTextures;
        public GameObject gridMapAuthoringPrefab;

        //
        [Header("AStar Pathfind Settings")]
        public List<TextAsset> astarGraphDatas;

        //
        [Header("Loading Scene Settings")]
        public List<string> sceneNames;
    }
}
