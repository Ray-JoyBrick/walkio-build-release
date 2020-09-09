namespace JoyBrick.Walkio.Tool.LevelDesign.EditorPart
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    //
    using GameLevel = JoyBrick.Walkio.Game.Level;
    
    public partial class LevelTable
    {
        private static void CreateLevelData(string levelName)
        {
            var absolutePathStart = Application.dataPath;
            var relativePathStart = "Assets";

            var generatedLevelPartialPath = Path.Combine("_", "_Generated - Level");
            var absoluteGeneratedToLevelFolder =
                Path.Combine(absolutePathStart, generatedLevelPartialPath, levelName);
            var relativeGeneratedToLevelFolder =
                Path.Combine(relativePathStart, generatedLevelPartialPath, levelName);

            
            var levelData = ScriptableObject.CreateInstance<GameLevel.Template.LevelData>();

            var levelDataPath = Path.Combine(relativeGeneratedToLevelFolder, $"Level Data.asset");
            AssetDatabase.CreateAsset(levelData, levelDataPath);
            
            AssetDatabase.SaveAssets();
        }
    }
}
