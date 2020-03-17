namespace JoyBrick.Walkio.Game.Environment.Bridge
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class TileData
    {
        
    }
    
    [CreateAssetMenu(menuName = "Walkio/Environment/Tile Data Asset")]
    public class TileDataAsset : ScriptableObject
    {
        public List<TileData> tileDatas;
    }
}
