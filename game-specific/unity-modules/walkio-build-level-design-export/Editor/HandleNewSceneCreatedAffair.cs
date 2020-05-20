namespace JoyBrick.Walkio.Build.LevelDesignExport.EditorPart
{
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    [InitializeOnLoad]
    public static partial class HandleNewSceneCreatedAffair
    {
        [InitializeOnLoadMethod]
        public static void Setup()
        {
            EditorSceneManager.newSceneCreated += (scene, setup, mode) =>
            {
                // Debug.Log($"Created scene is: {scene.name}");
                // if (scene.name.Contains("Master"))
                // {
                //     var gameObject = new GameObject("Level Operator");
                //     gameObject.AddComponent<LevelOperator>();
                //     
                //     EditorSceneManager.MoveGameObjectToScene(gameObject, scene);
                //
                //     EditorSceneManager.SaveScene(scene, scene.path);
                // }
            };
        }
    }
}
