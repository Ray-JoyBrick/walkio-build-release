namespace JoyBrick.Walkio.Build.LevelDesignExport
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.ProBuilder;
    using UnityEngine.ProBuilder.MeshOperations;
    using UnityEngine.SceneManagement;

    public partial class Level
    {
#if UNITY_EDITOR

#if ODIN_INSPECTOR
        [Button("Setup Master Scene"), GUIColor(0, 1, 0)]
        [ShowInInspector]
#endif
        public void SetupMasterScene()
        {
            if (masterScene == null) return;
            if (tileCount.x <= 0 || tileCount.y <= 0) return;

            var masterScenePath = AssetDatabase.GetAssetPath(masterScene);

            var mScene = EditorSceneManager.OpenScene(masterScenePath);

            var levelOperator = GetComponentAtScene<LevelOperator>(mScene);

            var absoluteMasterScenePath = masterScenePath.Replace("Assets", Application.dataPath);
            var absoluteScenesFolder = Directory.GetParent(absoluteMasterScenePath);

            var relativeScenesFolder = absoluteScenesFolder.FullName.Replace(Application.dataPath, "Assets");

            Debug.Log($"masterScenePath: {masterScenePath}");
            Debug.Log($"absoluteMasterScenePath: {absoluteMasterScenePath}");
            Debug.Log($"absoluteScenesFolder: {absoluteScenesFolder}");
            Debug.Log($"relativeScenesFolder: {relativeScenesFolder}");

            var subScenes = new List<Scene>();
            for (var z = 0; z < tileCount.y; ++z)
            {
                for (var x = 0; x < tileCount.x; ++x)
                {
                    var subSceneName = $"{this.name} - X{x:000} - Z{z:000}";
                    Debug.Log($"subSceneName: {subSceneName}");
                    
                    var combinedSubSceneName = Path.Combine(relativeScenesFolder, $"{subSceneName}.unity");

                    var subScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);

                    var tilePosition = new Vector3(x * tileCellCount.x, 0, z * tileCellCount.y);
                    SetupSubScene(subScene, tileCellCount, tilePosition);

                    EditorSceneManager.SaveScene(subScene, combinedSubSceneName);
                    subScenes.Add(subScene);
                }
            }

            if (subScenes.Any())
            {
                var sceneAssets =
                    subScenes.Select(subScene =>
                        {
                            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(subScene.path);

                            return sceneAsset;
                        })
                        .ToList();

                levelOperator.subScenes = sceneAssets;
                includedSubScenes = sceneAssets;
            }

            AssetDatabase.SaveAssets();
        }

        
        private static T GetComponentAtScene<T>(Scene scene) where T : UnityEngine.Component
        {
            T comp = default;

            if (!scene.IsValid()) return comp;

            var foundGOs =
                scene.GetRootGameObjects()
                    .Where(x => x.GetComponent<T>() != null)
                    .ToList();

            if (foundGOs != null && foundGOs.Any())
            {
                var foundGO = foundGOs.First();
                comp = foundGO.GetComponent<T>();
            }

            return comp;
        }

        private static void SetupSubScene(Scene subScene, Vector2Int cellCount, Vector3 tilePosition)
        {
            var groundBaseRootGameObject = new GameObject("Ground Base Root");
            groundBaseRootGameObject.AddComponent<GroundBaseRoot>();

            groundBaseRootGameObject.transform.position = tilePosition;

            var environmentRootGameObject = new GameObject("Environment Root");
            environmentRootGameObject.AddComponent<EnvironmentRoot>();

            environmentRootGameObject.transform.position = tilePosition;

            var areaRootGameObject = new GameObject("Area Root");
            areaRootGameObject.AddComponent<AreaRoot>();

            areaRootGameObject.transform.position = tilePosition;

            // ProBuilderMesh.Cre

            // var groundBaseGameObject = new GameObject("Ground Base");
            // var mesh = ProBuilderMesh.Create();
            var mesh = ShapeGenerator.GeneratePlane(PivotLocation.Center, cellCount.x, cellCount.y, cellCount.x / 2, cellCount.y / 2, Axis.Up);
            // mesh.CreateShapeFromPolygon();
            // mesh.
            
            mesh.ToMesh();
            mesh.Refresh();

            //
            mesh.name = $"Ground Base";
            mesh.gameObject.layer = LayerMask.NameToLayer("Ground Base");

            mesh.transform.SetParent(groundBaseRootGameObject.transform);
            mesh.transform.localPosition = Vector3.zero;

            // var meshFilter = groundBaseGameObject.AddComponent<MeshFilter>();
            var meshRenderer = mesh.gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                var materialPath = Path.Combine(
                    "Assets", "_", "1 - Game - Level Design", "Module - Environment - Level", "Common",
                    "Material - Ground Base.mat");
                var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                if (material != null)
                {
                    meshRenderer.material = material;
                }
            }

            
            EditorSceneManager.MoveGameObjectToScene(groundBaseRootGameObject, subScene);
            EditorSceneManager.MoveGameObjectToScene(environmentRootGameObject, subScene);
            EditorSceneManager.MoveGameObjectToScene(areaRootGameObject, subScene);
        }

#if ODIN_INSPECTOR
        [Button("Remove Sub Scene"), GUIColor(1, 0, 0)]
        [ShowInInspector]
#endif
        public void RemoveSubScene()
        {
            if (masterScene == null) return;

            var masterScenePath = AssetDatabase.GetAssetPath(masterScene);
            var mScene = EditorSceneManager.OpenScene(masterScenePath);

            var levelOperator = GetComponentAtScene<LevelOperator>(mScene);

            var subScenePaths =
                includedSubScenes
                    .Select(subSceneAsset => AssetDatabase.GetAssetPath(subSceneAsset))
                    .ToList();
            
            subScenePaths.ForEach(subScenePath =>
            {
                var subScene = EditorSceneManager.GetSceneByPath(subScenePath);
                EditorSceneManager.CloseScene(subScene, true);
            });

            var deleted = AssetDatabase.DeleteAssets(subScenePaths.ToArray(), new List<string>());
            if (deleted)
            {
                levelOperator.subScenes.Clear();
                includedSubScenes.Clear();
            }
        }
#endif
    }
}
