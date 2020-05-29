#if UNITY_EDITOR
namespace JoyBrick.Walkio.Build.LevelDesignExport.EditorPart
{
    using System.Linq;
#if ODIN_INSPECTOR
    using System.IO;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
#endif
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class LevelEditorWindow :
#if ODIN_INSPECTOR
        OdinMenuEditorWindow
#else
        EditorWindow
#endif
    {
        [MenuItem("Tools/Walkio/Environment/Level Editor")]
        private static void Open()
        {
            var window = GetWindow<LevelEditorWindow>();
#if ODIN_INSPECTOR
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
#endif
        }
        
#if ODIN_INSPECTOR
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true)
            {
                DefaultMenuStyle =
                {
                    IconSize = 28.00f
                },
                Config =
                {
                    DrawSearchToolbar = true
                }
            };

            // Adds the level overview table.
            LevelOverview.Instance.UpdateLevelOverview();
            tree.Add("Levels", new LevelTable(LevelOverview.Instance.allLevels));

            // Adds all level.
            var menuItems =
                tree.AddAllAssetsAtPath(
                    "Levels", 
                    "Assets/_/1 - Game - Level Design/Module - Environment - Level",
                    typeof(Level), true, true);

            menuItems.ToList().SortMenuItemsByName();

            // Add icons to levels.
            tree.EnumerateTree().AddIcons<Level>(x => x.icon);

            return tree;
        }
#endif

#if ODIN_INSPECTOR
        protected override void OnBeginDrawEditors()
        {
            if (this.MenuTree == null || this.MenuTree.Selection == null) return;
            
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            var absolutePathStart = Application.dataPath;
            var relativePathStart = "Assets";

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }

                // if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Item")))
                // {
                //     ScriptableObjectCreator.ShowDialog<Item>("Assets/Plugins/Sirenix/Demos/Sample - RPG Editor/Items", obj =>
                //     {
                //         obj.Name = obj.name;
                //         base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                //     });
                // }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Related Setup")))
                {
                    CreateLayer(8, "Ground Base");
                    CreateLayer(9, "Ground");
                    CreateLayer(10, "Obstacle");
                    CreateLayer(11, "Decoration");
                    CreateLayer(12, "Area");
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Level")))
                {
                    
                    var absoluteLevelPath = Path.Combine(absolutePathStart, "_", "1 - Game - Level Design", "Module - Environment - Level");
                    var relativeLevelPath = Path.Combine(relativePathStart, "_", "1 - Game - Level Design", "Module - Environment - Level");

                    var absoluteSceneFolderPath = string.Empty;
                    var relativeSceneFolderPath = string.Empty;
                    
                    ScriptableObjectCreator.ShowDialog<Level>(
                        // "Assets/_/1 - Game - Level Design/Module - Environment - Level/Data Assets/Level Editor/Level", obj =>
                        relativeLevelPath,
                        "Data Assets",
                        obj =>
                    {
                        obj.title = obj.name;

                        absoluteSceneFolderPath = Path.Combine(absoluteLevelPath, $"{obj.name}");
                        
                        // var sceneName = $"{obj.name} - Master.unity";
                        var sceneName = $"{obj.name}";
                        relativeSceneFolderPath = Path.Combine(relativeLevelPath, $"{obj.name}");

                        CrateScene(absoluteSceneFolderPath, relativeSceneFolderPath, sceneName, obj);

                        // Selects the newly created item in the editor
                        base.TrySelectMenuItemWithObject(obj);
                    });
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
#endif
        
        private static void CreateLayer(int index, string layerName)
        {
            // Open tag manager
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            // Layers Property
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            if (!PropertyExists(layersProp, 0, 31, layerName))
            {
                SerializedProperty sp = layersProp.GetArrayElementAtIndex(index);
                sp.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
            }
        }
        
        /// <summary>
        /// Checks if the value exists in the property.
        /// </summary>
        /// <returns><c>true</c>, if exists was propertyed, <c>false</c> otherwise.</returns>
        /// <param name="property">Property.</param>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="value">Value.</param>
        private static bool PropertyExists(SerializedProperty property, int start, int end, string value)
        {
            for (int i = start; i < end; i++)
            {
                SerializedProperty t = property.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        private static void CrateScene(
            string absoluteSceneFolderPath, string relativeSceneFolderPath, string sceneName, Level level)
        {
            var masterSceneName = $"{sceneName} - Master.unity";
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            SetupMasterScene(scene);

            // var subScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);

            var absoluteSceneFolderPathExisted = Directory.Exists(absoluteSceneFolderPath);
            if (!absoluteSceneFolderPathExisted)
            {
                Directory.CreateDirectory(absoluteSceneFolderPath);
            }

            var scenesFolderPath = Path.Combine(absoluteSceneFolderPath, $"Scenes");
            var scenesFolderPathExisted = Directory.Exists(scenesFolderPath);
            if (!scenesFolderPathExisted)
            {
                Directory.CreateDirectory(scenesFolderPath);
            }

            var relativeScenesFolderPath = Path.Combine(relativeSceneFolderPath, $"Scenes");
            var relativeScenePath = Path.Combine(relativeScenesFolderPath, masterSceneName);
            
            var saved = EditorSceneManager.SaveScene(scene, relativeScenePath);
            if (saved)
            {
                Debug.Log($"scene is saved at {relativeScenePath}");
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(relativeScenePath);

                if (sceneAsset != null)
                {
                    Debug.Log($"{relativeScenePath} is loaded as scene asset");
                    level.masterScene = sceneAsset;
                }
            }

            AssetDatabase.SaveAssets();
        }

        private static void SetupMasterScene(Scene scene)
        {
            var containerCurvyPathGameObject = new GameObject("Container - Curvy Path");
            EditorSceneManager.MoveGameObjectToScene(containerCurvyPathGameObject, scene);
            
            var containerSpawnPointGameObject = new GameObject("Container - Spawn Point");
            EditorSceneManager.MoveGameObjectToScene(containerSpawnPointGameObject, scene);

            //
            var levelOperatorGameObject = new GameObject("Level Operator");
            var levelOperatorComp = levelOperatorGameObject.AddComponent<LevelOperator>();

            levelOperatorComp.curvyPathContainer = containerCurvyPathGameObject;
            levelOperatorComp.spawnPointContainer = containerSpawnPointGameObject;

            EditorSceneManager.MoveGameObjectToScene(levelOperatorGameObject, scene);

            var pathfinderGameObject = new GameObject("Pathfinder");
            pathfinderGameObject.AddComponent<AstarPath>();

            EditorSceneManager.MoveGameObjectToScene(pathfinderGameObject, scene);
        }
    }
}
#endif
