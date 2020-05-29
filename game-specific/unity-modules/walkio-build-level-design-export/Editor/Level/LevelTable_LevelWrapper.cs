namespace JoyBrick.Walkio.Build.LevelDesignExport.EditorPart
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Build.LevelDesignExport;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using Object = UnityEngine.Object;

    public partial class LevelTable
    {
        private class LevelWrapper
        {
            // Level is a ScriptableObject and would render a unity object
            // field if drawn in the inspector, which is not what we want.

            public Level Level { get; }

            public LevelWrapper(Level level)
            {
                this.Level = level;
            }

#if ODIN_INSPECTOR
            [TableColumnWidth(50, false)]
            [ShowInInspector, PreviewField(45, ObjectFieldAlignment.Center)]
#endif
            public Texture Icon
            {
                get => this.Level.icon;
                set
                {
                    this.Level.icon = value;
                    EditorUtility.SetDirty(this.Level);
                }
            }

#if ODIN_INSPECTOR
            [TableColumnWidth(120)]
            [ShowInInspector]
#endif
            public string Title
            {
                get => this.Level.title;
                set
                {
                    this.Level.title = value;
                    EditorUtility.SetDirty(this.Level);
                }
            }

#if ODIN_INSPECTOR
            [TableColumnWidth(120)]
            [ReadOnly]
            [ShowInInspector]
#endif
            public SceneAsset MasterScene
            {
                get => this.Level.masterScene;
                set
                {
                    this.Level.masterScene = value;
                    EditorUtility.SetDirty(this.Level);
                }
            }

#if ODIN_INSPECTOR
            [Button("Generate Scene"), GUIColor(0, 1, 0)]
            [ShowInInspector]
#endif
            public void GenerateExportScene()
            {
                GenerateScene();
            }

#if ODIN_INSPECTOR
            [Button("Generate"), GUIColor(0, 1, 0)]
            [ShowInInspector]
#endif
            public void GenerateExportAsset()
            {
                OpenLevel();
            }
            // public float Shooting { get { return this.character.Skills.Shooting; } set { this.character.Skills.Shooting = value; EditorUtility.SetDirty(this.character); } }

            // [ShowInInspector, ProgressBar(0, 100)]
            // public float Shooting { get { return this.character.Skills.Shooting; } set { this.character.Skills.Shooting = value; EditorUtility.SetDirty(this.character); } }

            public void GenerateScene()
            {
                var levelName = Level.name;
                
                var scenePaths = new List<string>();
                Level.includedSubScenes.ForEach(sceneAsset =>
                {
                    // var sourcePath = Path.Combine(relativeSceneFolder, $"{sceneAsset.name}.unity");
                    var sourcePath = Path.Combine("Assets", "_", "1 - Game - Level Design",
                        "Module - Environment - Level", levelName, "Scenes", $"{sceneAsset.name}.unity");
                    var targetPath = Path.Combine("Assets", "_", "1 - Game - Level Design - Generated",
                        "Module - Environment - Level",
                        "Levels", levelName, "scenes", $"{sceneAsset.name}.unity");
                
                    Debug.Log($"CreateScenes - sourcePath: {sourcePath} \n targetPath: {targetPath}");
    
                    //
                    AssetDatabase.CopyAsset(sourcePath, targetPath);
                    
                    scenePaths.Add(targetPath);
                    
                });

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Create master scene
                var masterScenePath = Path.Combine("Assets", "_", "1 - Game - Level Design - Generated",
                    "Module - Environment - Level",
                    "Levels", levelName, "scenes", $"{levelName}.unity");

                // Setup master scene
                var masterScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                var colliderContainer = new GameObject("Container - Collider");
                var pathfinder = new GameObject("Pathfinder");
                pathfinder.AddComponent<AstarPath>();

                EditorSceneManager.SaveScene(masterScene, masterScenePath);

                var colliderGameObjects = new List<GameObject>();
    
                scenePaths.ForEach(scenePath =>
                {
                    var (sceneGameObjects, scene) = LoadAndHandleSubScene(scenePath);
                    
                    sceneGameObjects.ForEach(sceneGameObject =>
                    {
                        Debug.Log($"CreateScenes - {sceneGameObject.name} before moving pos: {sceneGameObject.transform.position}");
                        EditorSceneManager.MoveGameObjectToScene(sceneGameObject, masterScene);
                        Debug.Log($"CreateScenes - {sceneGameObject} after moving pos: {sceneGameObject.transform.position}");
                        
                        colliderGameObjects.Add(sceneGameObject);
                    });
                    
                    EditorSceneManager.SaveScene(scene);
                    EditorSceneManager.SaveScene(masterScene, masterScenePath);

                    EditorSceneManager.CloseScene(scene, true);
                    EditorSceneManager.SaveScene(masterScene, masterScenePath);
                });

                // var colliderContainer = new GameObject("Container - Collider");

                // Group one having no collider
                var noColliderGameObjects =
                    colliderGameObjects
                        .Where(cgo => cgo.GetComponent<Collider>() == null)
                        .ToList();
                // Group another having collider
                var filteredColliderGameObjects =
                    colliderGameObjects
                        .Where(cgo => cgo.GetComponent<Collider>() != null)
                        .ToList();

                // Remove game objects that have no collider
                noColliderGameObjects.ForEach(cgo => GameObject.DestroyImmediate(cgo));
                noColliderGameObjects.Clear();

                //
                filteredColliderGameObjects.ForEach(cgo =>
                {
                    var meshFilter = cgo.GetComponent<MeshFilter>();
                    if (meshFilter != null)
                    {
                        GameObject.DestroyImmediate(meshFilter);
                    }

                    var proBuilderMesh = cgo.GetComponent<UnityEngine.ProBuilder.ProBuilderMesh>();
                    if (proBuilderMesh != null)
                    {
                        GameObject.DestroyImmediate(proBuilderMesh);
                    }

                    var meshRenderer = cgo.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        GameObject.DestroyImmediate(meshRenderer);
                    }

                    cgo.transform.SetParent(colliderContainer.transform);
                });

                EditorSceneManager.SaveScene(masterScene, masterScenePath);

                scenePaths.ForEach(scenePath =>
                {
                    
                    
                    var gameObject = new GameObject("sub scene");
                    var subScene = gameObject.AddComponent<Unity.Scenes.SubScene>();

                    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

                    subScene.SceneAsset = sceneAsset;
                });
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorSceneManager.SaveScene(masterScene, masterScenePath);
            }

            private (int, int) GetTileIndex(string scenePath)
            {
                var sceneName = string.Empty;
                Level.includedSubScenes.ForEach(sceneAsset =>
                {
                    if (scenePath.Contains(sceneAsset.name))
                    {
                        sceneName = sceneAsset.name;
                    }
                });

                Debug.Log($"LoadAndHandleSubScene - sceneName: {sceneName}");
                
                var removeLevelString = sceneName.Replace($"{Level.name} - ", "");
                var separators = new List<string>
                {
                    " - "
                };
                var parts = removeLevelString.Split(separators.ToArray(), StringSplitOptions.None);
                var xPart = parts[0];
                var zPart = parts[1];

                var xTileIndex = int.Parse(xPart.Replace("X", ""));
                var zTileIndex = int.Parse(zPart.Replace("Z", ""));

                return (xTileIndex, zTileIndex);
            }

            private (List<GameObject>, Scene) LoadAndHandleSubScene(string scenePath)
            {
                Debug.Log($"LoadAndHandleSubScene - scenePath: {scenePath}");

                var (xTileIndex, zTileIndex) = GetTileIndex(scenePath);

                var offsetPosition = new Vector3(xTileIndex * Level.tileCellCount.x, 0,
                    zTileIndex * Level.tileCellCount.y);

                Debug.Log($"LoadAndHandleSubScene - offsetPosition: {offsetPosition}");

                // var scene = EditorSceneManager.OpenScene(scenePath);
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                var camera = GameCommon.Utility.GetComponentAtScene<Camera>(scene);
                var light = GameCommon.Utility.GetComponentAtScene<Light>(scene);

                if (camera != null)
                {
                    GameObject.DestroyImmediate(camera.gameObject);
                }

                if (light != null)
                {
                    GameObject.DestroyImmediate(light.gameObject);
                }
                
                // var gameObjects = GameObject.FindObjectsOfType<GameObject>();
                var gameObjects = GetSceneGameObjects(scene);
                Debug.Log($"LoadAndHandleSubScene - gameObjects count: {gameObjects.Count}");

                var colliderGameObjects = new List<GameObject>();
                
                gameObjects.ToList()
                    .ForEach(gameObject =>
                    {
                        if (gameObject.layer == LayerMask.NameToLayer("Ground Base"))
                        {
                            Debug.Log($"LoadAndHandleSubScene - Ground Base: {gameObject.name}");
                            var createdGameObject = GameObject.Instantiate(gameObject);
                            createdGameObject.transform.position += offsetPosition;
                            Debug.Log($"LoadAndHandleSubScene - {createdGameObject} pos: {createdGameObject.transform.position}");
                            colliderGameObjects.Add(createdGameObject);

                            var collider = gameObject.GetComponent<Collider>();
                            if (collider != null)
                            {
                                Debug.Log($"LoadAndHandleSubScene - Ground Base: {gameObject.name} has collider");
                                
                                GameObject.DestroyImmediate(collider);
                            }
                        }
                        else if (gameObject.layer == LayerMask.NameToLayer("Ground"))
                        {
                            var collider = gameObject.GetComponent<Collider>();
                            if (collider != null)
                            {
                                GameObject.DestroyImmediate(collider);
                            }
                        }
                        else if (gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                        {
                            Debug.Log($"LoadAndHandleSubScene - Obstacle: {gameObject.name}");

                            var createdGameObject = GameObject.Instantiate(gameObject);
                            createdGameObject.transform.position += offsetPosition;
                            Debug.Log($"LoadAndHandleSubScene - {createdGameObject} pos: {createdGameObject.transform.position}");
                            colliderGameObjects.Add(createdGameObject);

                            var collider = gameObject.GetComponent<Collider>();
                            if (collider != null)
                            {
                                Debug.Log($"LoadAndHandleSubScene - Obstacle: {gameObject.name} has collider");
                                
                                GameObject.DestroyImmediate(collider);
                            }
                        }
                        else if (gameObject.layer == LayerMask.NameToLayer("Decoration"))
                        {
                            var collider = gameObject.GetComponents<Collider>().ToList();
                            collider.ForEach(c => GameObject.DestroyImmediate(c));
                        }
                    });

                return (colliderGameObjects, scene);
            }

            private List<GameObject> GetSceneGameObjects(Scene scene)
            {
                var gameObjects = scene.GetRootGameObjects().ToList();
                
                var childGameObjects = new List<GameObject>();
                
                gameObjects.ForEach(gameObject =>
                {
                    Debug.Log($"GetSceneGameObjects - {gameObject.name}");
                    childGameObjects.Add(gameObject);
                    var childs = GetChildGameObjects(gameObject);
                    childGameObjects.AddRange(childs);
                });

                return childGameObjects;
            }

            private List<GameObject> GetChildGameObjects(GameObject gameObject)
            {
                var childGameObjects = new List<GameObject>();
                foreach (Transform child in gameObject.transform)
                {
                    childGameObjects.Add(child.gameObject);
                    Debug.Log($"GetChildGameObjects - {child.name}");
                    var childs = GetChildGameObjects(child.gameObject);
                    childGameObjects.AddRange(childs);
                }

                return childGameObjects;
            }
            

            public void OpenLevel()
            {
                var title = Level.title;

                //
                var crossProjectData = AssetDatabase.LoadAssetAtPath<GameCommon.CrossProjectData>(
                    "Packages/walkio.game.common/Data Assets/Cross Project Data.asset");

                //
                var relativeAssetFolderName = "Assets";
                var projectBaseFolderName = crossProjectData.commonProjectData.projectBaseFolderName;
                var baseFolderName = crossProjectData.assetLevelDesignProjectData.baseFolderName;
                var levelModuleFolderName = crossProjectData.assetLevelDesignProjectData.levelModuleFolderName;
    
                var directoryPath = Path.Combine(relativeAssetFolderName, projectBaseFolderName, baseFolderName);
                var environmentDirectoryPath = Path.Combine(directoryPath, levelModuleFolderName);
            
                var levelOverallAffairAssetPath = Path.Combine(environmentDirectoryPath, "Level Overall Affair.asset");
                var levelOverallAffair = AssetDatabase.LoadAssetAtPath<LevelOverallAffair>(levelOverallAffairAssetPath);

                if (levelOverallAffair != null)
                {
                    levelOverallAffair.doGeneration = true;
                }

                var levelEnvDirectoryPath = Path.Combine(environmentDirectoryPath, Level.name);
                var levelSceneDirectoryPath = Path.Combine(levelEnvDirectoryPath, "Scenes");
                var levelScenePath = Path.Combine(levelSceneDirectoryPath, $"{Level.name} - Master.unity");
                
                var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(levelScenePath);
            }
            
            public void RemoveGenerated()
            {
                var directoryPath = Path.Combine(Application.dataPath, "_", "1 - Game - Level Design - Generated");

                if (Directory.Exists(directoryPath))
                {
                    var relativeDirectoryPath = Path.Combine("Assets", "_", "1 - Game - Level Design - Generated");
                    AssetDatabase.DeleteAsset(relativeDirectoryPath);
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
