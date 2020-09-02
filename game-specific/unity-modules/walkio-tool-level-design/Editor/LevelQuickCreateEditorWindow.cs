namespace JoyBrick.Walkio.Tool.LevelDesign.EditorPart
{
    using System.IO;
    using Opsive.Shared.Game;
    using Opsive.UltimateCharacterController.Audio;
    using Opsive.UltimateCharacterController.Game;
    using Opsive.UltimateCharacterController.StateSystem;
    using Opsive.UltimateCharacterController.SurfaceSystem;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.SceneManagement;

    using GameCommon = JoyBrick.Walkio.Game.Common;

    public class LevelQuickCreateEditorWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Walkio/Level/Level Quick Create Tool")]
        private static void OpenWindow()
        {
            var window = GetWindow<LevelQuickCreateEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(600, 600);
            window.titleContent = new GUIContent("Quick Level Creation");
        }

        // [BoxGroup("Settings")]
        // [FolderPath(RequireExistingPath = true)]
        // public string OutputPath
        // {
        //     // Use EditorPrefs to hold persisntent user-variables.
        //     get { return EditorPrefs.GetString("SomeTextureToolWindow.OutputPath"); }
        //     set { EditorPrefs.SetString("SomeTextureToolWindow.OutputPath", value); }
        // }

        // [EnumToggleButtons]
        // [BoxGroup("Settings")]
        // public ScaleMode ScaleMode;
        //
        // [HorizontalGroup(0.5f, PaddingRight = 5, LabelWidth = 70)]
        // public Texture[] Textures = new Texture[8];
        //
        // [ReadOnly]
        // [HorizontalGroup]
        // [InlineEditor(InlineEditorModes.LargePreview)]
        // public Texture Preview;

        [BoxGroup("Settings")]
        public string levelName;
        [BoxGroup("Settings")]
        public Vector2Int tileCount;
        [BoxGroup("Settings")]
        public Vector2Int tileCellCount;

        private static void SetupDataAssets()
        {
            
        }
        
        private static void SetupMasterScene(Scene scene)
        {
            //
            var containerCurvyPathGameObject = new GameObject("Container - Curvy Path");
            EditorSceneManager.MoveGameObjectToScene(containerCurvyPathGameObject, scene);

            //
            var containerSpawnPointGameObject = new GameObject("Container - Spawn Point");
            EditorSceneManager.MoveGameObjectToScene(containerSpawnPointGameObject, scene);

            //
            var levelOperatorGameObject = new GameObject("Level Operator");
            var levelOperatorComp = levelOperatorGameObject.AddComponent<LevelOperator>();

            levelOperatorComp.curvyPathContainer = containerCurvyPathGameObject;
            levelOperatorComp.spawnPointContainer = containerSpawnPointGameObject;

            EditorSceneManager.MoveGameObjectToScene(levelOperatorGameObject, scene);

            //
            var pathfinderGameObject = new GameObject("Pathfinder");
            // pathfinderGameObject.AddComponent<AstarPath>();
            pathfinderGameObject.AddComponent(typeof(AstarPath));
            EditorSceneManager.MoveGameObjectToScene(pathfinderGameObject, scene);

            //
            var camera = GameCommon.Utility.SceneHelper.GetComponentAtScene<Camera>(scene);
            camera.gameObject.AddComponent<Cinemachine.CinemachineBrain>();

            //CM vcam1
            var cmVCam1 = new GameObject("CM vcam1");
            cmVCam1.AddComponent<Cinemachine.CinemachineVirtualCamera>();
            EditorSceneManager.MoveGameObjectToScene(cmVCam1, scene);
            
            var volume = new GameObject("Volume");
            volume.AddComponent<Volume>();
            EditorSceneManager.MoveGameObjectToScene(volume, scene);

            //
            var tpcManager = new GameObject("Third Person Controller Manager");
            tpcManager.AddComponent<SurfaceManager>();
            tpcManager.AddComponent<DecalManager>();
            tpcManager.AddComponent<KinematicObjectManager>();
            tpcManager.AddComponent<ObjectPool>();
            tpcManager.AddComponent<Scheduler>();
            tpcManager.AddComponent<AudioManager>();
            tpcManager.AddComponent<SpawnPointManager>();
            tpcManager.AddComponent<StateManager>();
            tpcManager.AddComponent<LayerManager>();
            EditorSceneManager.MoveGameObjectToScene(tpcManager, scene);
        }

        private static void CreateScene(
            string absoluteSceneFolderPath, string relativeSceneFolderPath,
            string sceneName, Vector2Int tileCount, Vector2Int tileCellCount)
            // Level level)
        {
            // var masterSceneName = $"{sceneName} - Master.unity";
            var masterSceneName = $"{sceneName} - Main.unity";
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            SetupDataAssets();
            SetupMasterScene(scene);
            //
            // // var subScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
            //
            var absoluteSceneFolderPathExisted = Directory.Exists(absoluteSceneFolderPath);
            if (!absoluteSceneFolderPathExisted)
            {
                Directory.CreateDirectory(absoluteSceneFolderPath);
            }

            var relativeScenePath = Path.Combine(relativeSceneFolderPath, masterSceneName);

            var saved = EditorSceneManager.SaveScene(scene, relativeScenePath);
            if (saved)
            {
                var level = ScriptableObject.CreateInstance<Level>();
                level.name = sceneName;

                Debug.Log($"scene is saved at {relativeScenePath}");
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(relativeScenePath);

                if (sceneAsset != null)
                {
                    Debug.Log($"{relativeScenePath} is loaded as scene asset");
                    level.masterScene = sceneAsset;
                    level.tileCount = tileCount;
                    level.tileCellCount = tileCellCount;
                }

                var levelPath = Path.Combine(relativeSceneFolderPath, "Level.asset");
                AssetDatabase.CreateAsset(level, levelPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
        public void PerformSomeAction()
        {
            Debug.Log($"Create scene: {levelName}");

            var absolutePathStart = Application.dataPath;
            var relativePathStart = "Assets";

            var absoluteLevelPath = Path.Combine(absolutePathStart, "_", "1 - Game", "Design - Level");
            var relativeLevelPath = Path.Combine(relativePathStart, "_", "1 - Game", "Design - Level");

            var absoluteSceneFolderPath = Path.Combine(absoluteLevelPath, levelName);
            var relativeSceneFolderPath = Path.Combine(relativeLevelPath, levelName);

            CreateScene(absoluteSceneFolderPath, relativeSceneFolderPath, levelName, tileCount, tileCellCount);
        }

    }
}
