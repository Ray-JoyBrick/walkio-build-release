namespace JoyBrick.Walkio.Build.LevelDesignExport
{
    using System.Collections.Generic;
    #if UNITY_EDITOR
    using UnityEditor;
    #endif
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [CreateAssetMenu(fileName = "Level Data", menuName = "Walkio/Build/Level Design/Level Data")]
    public class LevelData : ScriptableObject
    {
        #if UNITY_EDITOR
        public List<SceneAsset> processingScenes;
        #endif
    }
}
