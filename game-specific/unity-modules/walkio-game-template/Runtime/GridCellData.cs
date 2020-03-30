namespace JoyBrick.Walkio.Game.Template
{
    using System.Collections.Generic;
    using UnityEngine;

    // [System.Serializable]
    // public class GridCellDetail
    // {
    //     public int Kind;
    //     public int Cost;
    // }
    
    public class GridCellData : ScriptableObject
    {
        public List<GridCellDetail> gridCellDetails;
    }
}
