namespace DefaultNamespace
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Hud Data", menuName = "Walkio/Game/Hud - Stage/Hud Data")]
    public class HudData : ScriptableObject
    {
        public GameObject canvas;

        public List<GameObject> usedPrefabs;
        public List<ScriptableObject> usedAssets;
    }
}
