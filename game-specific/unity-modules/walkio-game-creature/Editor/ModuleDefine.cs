namespace JoyBrick.Walkio.Game.Creature.EditorPart
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
            // Debug.Log($"Module - Creature - ModuleDefine");

            GameCommon.EditorPart.Utility.DefinesHelper.AddSymbolToAllTargets("WALKIO_CREATURE");
        }
    }
}
