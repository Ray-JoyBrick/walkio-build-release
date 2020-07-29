namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using System.Linq;
    // using HellTap.PoolKit;
    using Pathfinding;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
#if WALKIO_LEVEL
    using GameLevel = JoyBrick.Walkio.Game.Level;
#endif

    public partial class Bootstrap

#if WALKIO_LEVEL
        : GameLevel.IGridWorldProvider,
            GameLevel.ILevelPropProvider
#endif

    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Level")]
#endif
        public ScriptableObject gridWorldData;

        public ScriptableObject GridWorldData => gridWorldData;

        private void SetupLevelPart()
        {
        }

        public Camera LevelCamera { get; set; }
    }
}
