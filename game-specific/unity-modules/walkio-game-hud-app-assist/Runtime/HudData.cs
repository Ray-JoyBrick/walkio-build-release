namespace JoyBrick.Walkio.Game.Hud.App.Assist
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Hud Data", menuName = "Walkio/Game/Hud - App Assist/Hud Data")]
    public class HudData : ScriptableObject
    {
        public GameObject canvasPrefab;

        public List<GameObject> usedPrefabs;
        public List<ScriptableObject> usedAssets;
    }  
}
