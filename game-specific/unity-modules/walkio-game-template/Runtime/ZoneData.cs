namespace JoyBrick.Walkio.Game.Template
{
    using System.Collections.Generic;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "Zone Data", menuName = "Walkio/Game/Template/Zone Data")]
    public class ZoneData : ScriptableObject
    {
        public int Id;

        public TextAsset pathfindingData;
        public Texture2D gridImage;

        public GameObject prefab;

        public List<GridCellDetail> gridCellDetails;
    }
}
