namespace JoyBrick.Walkio.Tool.LevelDesign.EditorPart
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(LevelOperator))]
    public class LevelOperatorEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            var tree = this.Tree;
            var target = this.target as LevelOperator;

            // InspectorUtilities.BeginDrawPropertyTree(tree, true);      // This and EndDrawPropertyTree automatically handles a lot of stuff like prefab instance modifications and undo
            //
            // // if (target.SomeCondition)
            // if (true)
            // {
            //     var someProp1 = tree.GetPropertyAtPath("subScenes");   // Like serializedObject.FindProperty("path")
            //     someProp1.Draw();                                      // Like EditorGUILayout.PropertyField(property);
            // }
            // else
            // {
            //     var someProp2 = tree.GetPropertyAtPath("someProp2");
            //     someProp2.Draw(new GUIContent("My Label"));            // You can also pass custom labels into Draw()
            // }
            //
            // InspectorUtilities.EndDrawPropertyTree(tree);

            base.OnInspectorGUI();
        }

        [Button(ButtonSizes.Gigantic), GUIColor(0, 1, 0)]
        public void PerformSomeAction()
        {

        }
    }
}
