namespace JoyBrick.Walkio.Game.Environment.Bridge
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;

    [System.Serializable]
    public class TileData
    {
        public int kind;
        public int cost;
    }
    
    [CreateAssetMenu(menuName = "Walkio/Environment/Tile Data Asset")]
    public class TileDataAsset :
        ScriptableObject,
        IComponentData
    {
        public List<TileData> tileDatas;
    }
}
