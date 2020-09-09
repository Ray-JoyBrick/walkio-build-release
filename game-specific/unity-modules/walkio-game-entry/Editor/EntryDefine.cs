namespace JoyBrick.Walkio.Game.EditorPart
{
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public class EntryDefine
    {
        static EntryDefine()
        {
            Debug.Log($"EntryDefine");

            // Common.EditorPart.Utility.DefinesHelper.AddSymbolToAllTargets("WALKIO_LEVEL");
        }
    }
}
