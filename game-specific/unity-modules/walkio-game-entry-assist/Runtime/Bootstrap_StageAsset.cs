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
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Stage Asset")]
#endif        
        public string startLevel;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Stage Asset")]
#endif        
        public ScriptableObject levelSettingData;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Stage Asset")]
#endif        
        public SceneInstance sceneInstance;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Stage Asset")]
#endif        
        public ScriptableObject flowFieldSettingData;
        
        //
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Stage Asset")]
#endif        
        public Vector2Int segmentCount;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Stage Asset")]
#endif        
        public Vector3 startOffset;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Stage Asset")]
#endif        
        public Material planeMaterial;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Stage Asset")]
#endif        
        public Material groupPlaneMaterial;

        public Vector2Int SegmentCount => segmentCount;
        public Vector3 StartOffset => startOffset;
        public Material PlanMaterial => planeMaterial;
        public Material GroupPlaneMaterial => groupPlaneMaterial;

#if CREATURE_DESIGN_PROJECT
        private void CreateStubStageAsset()
        {
            // waypointPathBlobAssetAuthoringPrefab.GetComponent<>()
        }
#endif
    }
}
