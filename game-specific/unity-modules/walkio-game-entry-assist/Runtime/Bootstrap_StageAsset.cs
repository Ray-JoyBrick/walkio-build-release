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
        //
        public GameObject waypointPathBlobAssetAuthoringPrefab;
        public ScriptableObject waypointData;
        
#if CREATURE_DESIGN_PROJECT
        private void CreateStubStageAsset()
        {
            // waypointPathBlobAssetAuthoringPrefab.GetComponent<>()
        }
#endif
    }
}
