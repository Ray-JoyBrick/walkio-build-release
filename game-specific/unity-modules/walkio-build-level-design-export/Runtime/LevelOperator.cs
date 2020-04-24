namespace JoyBrick.Walkio.Build.LevelDesignExport
{
    using System.Collections.Generic;
#if ODIN_INSPECTOR    
    using Sirenix.OdinInspector;
#endif
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;

    public partial class LevelOperator : MonoBehaviour
    {
        public int xSubSceneCount;
        public int zSubSceneCount;

        public int gridCount;
        
        public GameObject curvy;
    }
}
