namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class LevelSetting : ScriptableObject
    {
        //
        public int hGridCount;
        public int vGridCount;

        public int gridCellCount;

        //
        public int aiControlCount;

        public WaypointPath waypointPath;
        
        //
        public List<Vector3> spawnPoints;
        
        //
        public List<Texture2D> gridTextures;

        public List<TextAsset> astarGraphDatas;
    }
}
