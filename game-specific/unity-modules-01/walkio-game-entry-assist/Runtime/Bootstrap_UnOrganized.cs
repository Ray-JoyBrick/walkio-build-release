namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public partial class Bootstrap
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Unorgianized")]
#endif        
        public float timeToSpawnTeamUnit = 0.1f;
    }
}
