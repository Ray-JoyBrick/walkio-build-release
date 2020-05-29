namespace JoyBrick.Walkio.Game.Template
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class GridCellDetail
    {
        public int Kind;
        public int Cost;
    }
    
    [CreateAssetMenu(fileName = "Environment Data", menuName = "Walkio/Game/Template/Environment Data")]
    public class EnvironmentData : ScriptableObject
    {
        public Vector2 gridCellSize;
        
        public Vector2 tileCellSize;
        public Vector2 tileCellCount;
        
        // TODO: Rename it to more meaningful name later
        public GameObject prefab;
        
        //
        public List<GridCellDetail> gridCellDetails;
    }
}
