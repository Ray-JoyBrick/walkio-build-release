namespace JoyBrick.Walkio.Game.Assist
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
#if UNITY_EDITOR
        public List<SceneAsset> subScenes;
#endif
    }
}
