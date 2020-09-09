namespace JoyBrick.Walkio.Game.Move.CrowdSimulate.Assist.EditorPart
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
            // Debug.Log($"Module Assist - Move - CrowdSimulate - ModuleDefine");

            GameCommon.EditorPart.Utility.DefinesHelper.AddSymbolToAllTargets("WALKIO_MOVE_CROWDSIMULATE_ASSIST");
        }
    }
}
