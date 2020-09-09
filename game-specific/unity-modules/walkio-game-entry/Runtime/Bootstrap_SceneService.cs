namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    //
    public partial class Bootstrap :
        GameCommon.ISceneService
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Scene Service")]
#endif
        public string sceneName = "Entry";        
        
        public void MoveToCurrentScene(GameObject inGo)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid())
            {
                SceneManager.MoveGameObjectToScene(inGo, scene);
            }
        }
    }
}
