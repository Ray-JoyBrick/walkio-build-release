namespace JoyBrick.Walkio.Game.Level
{
    using System.Collections.Generic;
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
