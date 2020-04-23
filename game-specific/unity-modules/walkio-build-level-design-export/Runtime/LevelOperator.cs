namespace JoyBrick.Walkio.Build.LevelDesignExport
{
    using System.Collections.Generic;
    #if UNITY_EDITOR
    using UnityEditor;
    #endif
    using UnityEngine;

    public class LevelOperator : MonoBehaviour
    {
        public GameObject curvy;
        
        public int xSubSceneCount;
        public int zSubSceneCount;
        
        #if UNITY_EDITOR
        public List<SceneAsset> subScenes;
        #endif
    }

    // [InitializeOnLoad]
    // public class LevelOpreator
    // {
    //     
    // }
}
