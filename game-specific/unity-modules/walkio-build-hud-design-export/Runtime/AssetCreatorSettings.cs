namespace JoyBrick.Walkio.Build.HudDesignExport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    [System.Serializable]
    public class AssetEntry
    {
        public string name;
        public UnityEngine.Object asset;
    }
    
    [System.Serializable]
    public class Group
    {
        public string name;
        public List<AssetEntry> entries;
    }
    
    [CreateAssetMenu(fileName = "Asset Creator Settings", menuName = "Walkio/Game/Asset/Hud/Asset Creator Settings")]
    public class AssetCreatorSettings : ScriptableObject
    {
        public List<Group> groups;
    } 
} 
