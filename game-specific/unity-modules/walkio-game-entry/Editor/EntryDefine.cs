namespace JoyBrick.Walkio.Game.EditorPart
{
    using UnityEditor;

    [InitializeOnLoad]
    public class EntryDefine
    {
        static EntryDefine()
        {
            Common.EditorPart.Utility.DefinesHelper.AddSymbolToAllTargets("WALKIO_LEVEL");
        }
    }
}
