// namespace JoyBrick.Walkio.Build.LevelDesignExport.EditorPart
// {
//     using System.Collections.Generic;
//     using System.IO;
//     using System.Linq;
//     using Sirenix.OdinInspector;
//     using Sirenix.OdinInspector.Editor;
//     using Sirenix.Utilities;
//     using UnityEditor;
//     using UnityEditor.SceneManagement;
//     using UnityEngine;
//     using UnityEngine.SceneManagement;
//
//     // TODO: This is temp class for the task to be performed, rename or remove later
//     public class SomeWindow : OdinEditorWindow
//     {
//         [MenuItem("My Game/My Window")]
//         private static void OpenWindow()
//         {
//             GetWindow<SomeWindow>().Show();
//         }
//
//         [PropertyOrder(-10)]
//         [HorizontalGroup]
//         [Button(ButtonSizes.Large)]
//         public void OpenLevel001()
//         {
//             // var crossProject = AssetDatabase.LoadAssetAtPath<LevelOverallAffair>(levelOverallAffairAssetPath);
//
//             var directoryPath = Path.Combine("Assets", "_", "1 - Game - Level Design");
//             var environmentDirectoryPath = Path.Combine(directoryPath, "Module - Environment - Level");
//             
//             var levelOverallAffairAssetPath = Path.Combine(environmentDirectoryPath, "Level Overall Affair.asset");
//             var levelOverallAffair = AssetDatabase.LoadAssetAtPath<LevelOverallAffair>(levelOverallAffairAssetPath);
//
//             if (levelOverallAffair != null)
//             {
//                 levelOverallAffair.doGeneration = true;
//             }
//             
//             var level001EnvDirectoryPath = Path.Combine(environmentDirectoryPath, "Level 001");
//             var level001SceneDirectoryPath = Path.Combine(level001EnvDirectoryPath, "Scenes");
//             var level001ScenePath = Path.Combine(level001SceneDirectoryPath, "Level 001 - Master.unity");
//
//             // AssetDatabase.LoadAssetAtPath<Scene>(level001ScenePath);
//
//             var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(level001ScenePath);
//         }
//         
//         [PropertyOrder(-10)]
//         [HorizontalGroup]
//         [Button(ButtonSizes.Large)]
//         public void OpenLevel002()
//         {
//             var directoryPath = Path.Combine("Assets", "_", "1 - Game - Level Design");
//             var environmentDirectoryPath = Path.Combine(directoryPath, "Module - Environment - Level");
//             
//             var levelOverallAffairAssetPath = Path.Combine(environmentDirectoryPath, "Level Overall Affair.asset");
//             var levelOverallAffair = AssetDatabase.LoadAssetAtPath<LevelOverallAffair>(levelOverallAffairAssetPath);
//
//             if (levelOverallAffair != null)
//             {
//                 levelOverallAffair.doGeneration = true;
//             }
//             
//             var level001EnvDirectoryPath = Path.Combine(environmentDirectoryPath, "Level 002");
//             var level001SceneDirectoryPath = Path.Combine(level001EnvDirectoryPath, "Scenes");
//             var level001ScenePath = Path.Combine(level001SceneDirectoryPath, "Level 002 - Master.unity");
//
//             // AssetDatabase.LoadAssetAtPath<Scene>(level001ScenePath);
//
//             var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(level001ScenePath);
//         }
//
//         [HorizontalGroup]
//         [Button(ButtonSizes.Large), GUIColor(1, 0.33f, 0)]
//         public void RemoveGenerated()
//         {
//             var directoryPath = Path.Combine(Application.dataPath, "_", "1 - Game - Level Design - Generated");
//
//             if (Directory.Exists(directoryPath))
//             {
//                 var relativeDirectoryPath = Path.Combine("Assets", "_", "1 - Game - Level Design - Generated");
//                 AssetDatabase.DeleteAsset(relativeDirectoryPath);
//                 AssetDatabase.Refresh();
//             }
//         }
//
//         [HorizontalGroup]
//         [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
//         public void SomeButton4() { }
//
//         [TableList]
//         public List<SomeType> SomeTableData;
//     }
//
//     public class SomeType
//     {
//         [TableColumnWidth(50)]
//         public bool Toggle;
//
//         [AssetsOnly]
//         public GameObject SomePrefab;
//
//         public string Message;
//
//         [TableColumnWidth(160)]
//         [HorizontalGroup("Actions")]
//         public void Test1() { }
//
//         [HorizontalGroup("Actions")]
//         public void Test2() { }
//     }
// }
