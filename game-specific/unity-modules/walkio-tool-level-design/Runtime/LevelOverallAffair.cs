namespace JoyBrick.Walkio.Tool.LevelDesign
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Level Overall Affair", menuName = "Walkio/Build/Level Design/Level Overall Affair")]
    public class LevelOverallAffair : ScriptableObject
    {
        public bool doGeneration;

        public List<string> masterSceneNames;
        public List<string> masterSceneToLevelNames;
    }
}
