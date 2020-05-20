#if UNITY_EDITOR
namespace JoyBrick.Walkio.Build.LevelDesignExport
{
    using System.Linq;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
#endif
    using UnityEditor;

#if ODIN_INSPECTOR
    [GlobalConfig("_/1 - Game - Level Design/Module - Environment - Level", UseAsset = true)]
#endif
    public class LevelOverview :
#if ODIN_INSPECTOR
        GlobalConfig<LevelOverview>
#else
        ScriptableObject
#endif
    {
#if ODIN_INSPECTOR
        [ReadOnly]
        [ListDrawerSettings(Expanded = true)]
#endif
        public Level[] allLevels;

#if UNITY_EDITOR
        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
#endif
        public void UpdateLevelOverview()
        {
            // Finds and assigns all scriptable objects of type Character
            var found =
                AssetDatabase.FindAssets("t:Level")
                    .Select(guid => AssetDatabase.LoadAssetAtPath<Level>(AssetDatabase.GUIDToAssetPath(guid)))
                    .Where(x => x != null)
                    .OrderBy(x => x.name)
                    .ToArray();

            if (found.Any())
            {
                this.allLevels = found;
            }
            else
            {
                this.allLevels = default;
            }
        }
    }
}
#endif
