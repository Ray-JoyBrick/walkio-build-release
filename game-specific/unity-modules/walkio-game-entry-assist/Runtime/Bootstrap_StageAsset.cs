namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public partial class Bootstrap :
        GameCommon.ISceneAssistProvider
    {
        //
        // public GameObject waypointPathBlobAssetAuthoringPrefab;
        // public ScriptableObject waypointData;

        public string startLevel;
        
        public ScriptableObject levelSettingData;
        public SceneInstance sceneInstance;

        public ScriptableObject flowFieldSettingData;
        
        //
        public Vector2Int segmentCount;
        public Vector3 startOffset;
        public Material planeMaterial;

        public Vector2Int SegmentCount => segmentCount;
        public Vector3 StartOffset => startOffset;
        public Material PlanMaterial => planeMaterial;

#if CREATURE_DESIGN_PROJECT
        private void CreateStubStageAsset()
        {
            // waypointPathBlobAssetAuthoringPrefab.GetComponent<>()
        }
#endif
    }
}
