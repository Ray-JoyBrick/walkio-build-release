namespace JoyBrick.Walkio.Build.LevelDesignExport.EditorPart
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Build.LevelDesignExport;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    using UnityEditor;
    using UnityEngine;

    //
    using CrossProject = JoyBrick.Walkio.CrossProject;

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
            
            
            public void OpenLevel()
            {
                var title = Level.title;

                //
                var crossProjectData = AssetDatabase.LoadAssetAtPath<CrossProject.CrossProjectData>(
                    "Packages/walkio.cross-project/Data Assets/Cross Project Data.asset");

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
