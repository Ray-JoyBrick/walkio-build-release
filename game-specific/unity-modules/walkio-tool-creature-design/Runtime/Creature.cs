namespace JoyBrick.Walkio.Build.CreatureDesign
{
    using System.Collections.Generic;
    using System.Linq;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    using Sirenix.Utilities;
#endif

#if UNITY_EDITOR
    using System.IO;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;
#endif
    using UnityEngine;

    [CreateAssetMenu(fileName = "Creature Data", menuName = "Walkio/Build/Creature Design/Creature")]
//     public partial class Creature :
// #if ODIN_INSPECTOR
//         SerializedScriptableObject
// #else
//         ScriptableObject
// #endif
    public abstract class Creature : ScriptableObject
    {

        public Texture icon;


        public string title;


        public string Name;
    }
}
