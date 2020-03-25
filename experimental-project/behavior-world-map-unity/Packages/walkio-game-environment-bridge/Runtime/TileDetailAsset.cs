namespace JoyBrick.Walkio.Game.Environment.Bridge
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;

    [System.Serializable]
    public class TileDetail
    {
        public int kind;
        public int cost;
    }
    
    [CreateAssetMenu(menuName = "Walkio/Environment/Tile Data Asset")]
    public class TileDetailAsset : ScriptableObject
    {
        public List<TileDetail> tileDetails;
    }
}
