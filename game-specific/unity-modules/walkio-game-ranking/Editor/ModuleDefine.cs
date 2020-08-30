namespace JoyBrick.Walkio.Game.Ranking.EditorPart
{
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public class ModuleDefine
    {
        static ModuleDefine()
        {
            Debug.Log($"Module - Ranking - ModuleDefine");

            Common.EditorPart.Utility.DefinesHelper.AddSymbolToAllTargets("WALKIO_RANKING");
        }
    }
}
