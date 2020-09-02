namespace JoyBrick.Walkio.Game.Hud.Preparation.EditorPart
{
    using UnityEditor;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    [InitializeOnLoad]
    public class ModuleDefine
    {
        static ModuleDefine()
        {
            // Debug.Log($"Module - Hud - Preparation - ModuleDefine");

            GameCommon.EditorPart.Utility.DefinesHelper.AddSymbolToAllTargets("WALKIO_HUD_PREPARATION");
        }
    }
}
