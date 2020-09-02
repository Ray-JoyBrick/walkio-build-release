namespace JoyBrick.Walkio.Game.FlowControl.Stage.EditorPart
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
            // Debug.Log($"Module - FlowControl - Stage - ModuleDefine");

            GameCommon.EditorPart.Utility.DefinesHelper.AddSymbolToAllTargets("WALKIO_FLOWCONTROL_STAGE");
        }
    }
}
