namespace JoyBrick.Walkio.Build.LevelDesignExport
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [CreateAssetMenu(fileName = "Level Data", menuName = "Walkio/Build/Level Design/Level Data")]
    public class LevelData : ScriptableObject
    {
        public List<SceneAsset> processingScenes;
    }
}
