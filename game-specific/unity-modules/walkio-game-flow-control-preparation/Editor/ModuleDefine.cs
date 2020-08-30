namespace JoyBrick.Walkio.Game.FlowControl.Preparation.EditorPart
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
            Debug.Log($"Module - FlowControl - Preparation - ModuleDefine");

            GameCommon.EditorPart.Utility.DefinesHelper.AddSymbolToAllTargets("WALKIO_FLOWCONTROL_PREPARATION");
        }
    }
}
