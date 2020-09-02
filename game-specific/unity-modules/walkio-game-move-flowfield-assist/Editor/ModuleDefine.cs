namespace JoyBrick.Walkio.Game.Move.FlowField.Assist.EditorPart
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
            // Debug.Log($"Module Assist - Move - FlowField - ModuleDefine");

            GameCommon.EditorPart.Utility.DefinesHelper.AddSymbolToAllTargets("WALKIO_MOVE_FLOWFIELD_ASSIST");
        }
    }
}
