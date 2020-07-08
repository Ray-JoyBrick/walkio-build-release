namespace JoyBrick.Walkio.Game.Common.Template
{
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
}
