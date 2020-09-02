namespace JoyBrick.Walkio.Tool.LevelDesign
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

    [CreateAssetMenu(fileName = "Level Data", menuName = "Walkio/Build/Level Design/Level")]
    public partial class Level :
#if ODIN_INSPECTOR
        SerializedScriptableObject
#else
        ScriptableObject
#endif
    {
        private const string LeftVerticalGroup = "Split/Left";
        private const string StatsBoxGroup = "Split/Left/Stats";
        private const string CreatureBoxGroup = "Split/Left/Creatures";
        private const string GeneralSettingsVerticalGroup = "Split/Left/General Settings/Split/Right";


#if ODIN_INSPECTOR
        [HideLabel, PreviewField(55)]
        [VerticalGroup(LeftVerticalGroup)]
        [HorizontalGroup(LeftVerticalGroup + "/General Settings/Split", 55, LabelWidth = 67)]
        // [HorizontalGroup("Split", 55, LabelWidth = 70)]
        // [HideLabel, PreviewField(55, ObjectFieldAlignment.Left)]
#endif
        public Texture icon;

#if ODIN_INSPECTOR
        // [VerticalGroup("Split/Meta")]
        [BoxGroup(LeftVerticalGroup + "/General Settings")]
        [VerticalGroup(GeneralSettingsVerticalGroup)]
#endif
        public string title;

#if ODIN_INSPECTOR
        // [VerticalGroup("Split/Meta")]
        [BoxGroup(LeftVerticalGroup + "/General Settings")]
        [VerticalGroup(GeneralSettingsVerticalGroup)]
#endif
        public string localizedTitleId;

#if ODIN_INSPECTOR
        [BoxGroup("Split/Right/Description")]
        [HideLabel, TextArea(4, 14)]
#endif
        public string description;

#if ODIN_INSPECTOR
        [HorizontalGroup("Split", 0.5f, MarginLeft = 5, LabelWidth = 130)]
        [BoxGroup("Split/Right/Notes")]
        [HideLabel, TextArea(4, 9)]
#endif
        public string notes;

// #if ODIN_INSPECTOR
//         [AssetsOnly]
//         [VerticalGroup(GeneralSettingsVerticalGroup)]
// #endif
//         public GameObject prefab;

#if ODIN_INSPECTOR
        [BoxGroup(StatsBoxGroup)]
#endif
        public Vector2Int tileCount = new Vector2Int(1, 1);

#if ODIN_INSPECTOR
        [BoxGroup(StatsBoxGroup)]
#endif
        public Vector2Int tileCellCount = new Vector2Int(64, 64);

#if ODIN_INSPECTOR
        [BoxGroup(CreatureBoxGroup)]
        [ProgressBar(0, 30)]
#endif
        public int aiControl = 0;
    }
}
