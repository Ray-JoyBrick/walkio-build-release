namespace Game.Template
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class Waypoint
    {
        public Vector3 location;
    }

    public enum WaypointPathKind
    {
        Linear
    }

    [System.Serializable]
    public class WaypointPath
    {
        public WaypointPathKind kind;
        public List<Waypoint> waypoints;
    }

    [CreateAssetMenu(fileName = "Waypoint Data", menuName = "Game/Template/Waypoint Data")]
    public class WaypointData : ScriptableObject
    {
        public List<WaypointPath> waypointPaths;
    }

}
