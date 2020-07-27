namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Linq;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    using GameMoveFlowFieldAssist = JoyBrick.Walkio.Game.Move.FlowField.Assist;

    public partial class Bootstrap :
        GameMoveFlowFieldAssist.IFlowFieldWorldProvider
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Flow Field")]
#endif
        public ScriptableObject flowFieldWorldData;

        public ScriptableObject FlowFieldWorldData => flowFieldWorldData;
    }
}
