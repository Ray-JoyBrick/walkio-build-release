namespace JoyBrick.Walkio.Build.LevelDesignExport.Editor
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SomeWindow : OdinEditorWindow
    {
        [MenuItem("My Game/My Window")]
        private static void OpenWindow()
        {
            GetWindow<SomeWindow>().Show();
        }

        [PropertyOrder(-10)]
        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        public void SomeButton1()
        {
            var directoryPath = Path.Combine("Assets", "_", "1 - Game - Level Design");
            var environmentDirectoryPath = Path.Combine(directoryPath, "Module - Environment - Level");
            var level001EnvDirectoryPath = Path.Combine(environmentDirectoryPath, "Level 001");
            var level001SceneDirectoryPath = Path.Combine(level001EnvDirectoryPath, "Scenes");
            var level001ScenePath = Path.Combine(level001SceneDirectoryPath, "Level 001 - Master.unity");

            // AssetDatabase.LoadAssetAtPath<Scene>(level001ScenePath);

            var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(level001ScenePath);
            // UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Level 001 - Master");
            // EditorSceneManager.LoadSceneAsync()

            var waypointData = ScriptableObject.CreateInstance<WaypointData>();


            var rootGameObjects = scene.GetRootGameObjects();
            rootGameObjects.ToList().ForEach(x =>
            {
                //
                if (string.Compare(x.name, "Curvy") == 0)
                {
                    waypointData.waypointPaths = new List<WaypointPath>();
                    foreach (Transform curvy in x.transform)
                    {
                        // curvy.GetComponent<CurvySpline>()
                        
                        var wp = new WaypointPath();
                        wp.waypoints = new List<Waypoint>();
                        waypointData.waypointPaths.Add(wp);
                        foreach (Transform waypoint in curvy)
                        {
                            wp.waypoints.Add(new Waypoint
                            {
                                location = waypoint.position
                            });
                            Debug.Log(waypoint);
                        }
                    }
                }
            });

            //
            var generatedDirectoryPath = Path.Combine(Application.dataPath, "_", "1 - Game - Level Design - Generated");
            var levelDirectoryPath = Path.Combine(generatedDirectoryPath, "Levels");
            CreateDirectoryIfNotExisted(generatedDirectoryPath);
            CreateDirectoryIfNotExisted(levelDirectoryPath);
            
            var level001DirectoryPath = Path.Combine(levelDirectoryPath, "level001");
            CreateDirectoryIfNotExisted(level001DirectoryPath);
            
            var relativeWaypointDataAssetPath = Path.Combine("Assets", "_", "1 - Game - Level Design - Generated",
                "Levels", "level001");
            var waypointDataAssetPath = Path.Combine(relativeWaypointDataAssetPath, "Waypoint Data.asset");
            
            AssetDatabase.CreateAsset(waypointData, waypointDataAssetPath);
            AssetDatabase.SaveAssets();
        }

        private static void CreateDirectoryIfNotExisted(string directoryPath)
        {
            var existed = Directory.Exists(directoryPath);
            if (!existed)
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        public void SomeButton2() { }

        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        public void SomeButton3() { }

        [HorizontalGroup]
        [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        public void SomeButton4() { }

        [HorizontalGroup]
        [Button(ButtonSizes.Large), GUIColor(1, 0.5f, 0)]
        public void SomeButton5() { }

        [TableList]
        public List<SomeType> SomeTableData;
    }

    public class SomeType
    {
        [TableColumnWidth(50)]
        public bool Toggle;

        [AssetsOnly]
        public GameObject SomePrefab;

        public string Message;

        [TableColumnWidth(160)]
        [HorizontalGroup("Actions")]
        public void Test1() { }

        [HorizontalGroup("Actions")]
        public void Test2() { }
    }
}
