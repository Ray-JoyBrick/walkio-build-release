namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using System.Linq;
    using HellTap.PoolKit;
    using Pathfinding;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
    using GameLevel = JoyBrick.Walkio.Game.Level;

    public partial class Bootstrap :
        GameLevel.IGridWorldProvider
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Level")]
#endif
        public ScriptableObject gridWorldData;

        public ScriptableObject GridWorldData => gridWorldData;

        private void SetupLevelPart()
        {
        }

    }
}
