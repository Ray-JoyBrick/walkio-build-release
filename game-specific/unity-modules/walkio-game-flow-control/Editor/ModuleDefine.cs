namespace JoyBrick.Walkio.Game.FlowControl.EditorPart
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
            Debug.Log($"Module - FlowControl - ModuleDefine");

            GameCommon.EditorPart.Utility.DefinesHelper.AddSymbolToAllTargets("WALKIO_FLOWCONTROL");
        }
    }
}
