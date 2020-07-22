namespace JoyBrick.Walkio.Game.Hud.Stage
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Hud Data", menuName = "Walkio/Game/Hud - Stage/Hud Data")]
    public class HudData : ScriptableObject
    {
        public GameObject canvasPrefab;

        public List<GameObject> usedPrefabs;
        public List<ScriptableObject> usedAssets;
    }
}
