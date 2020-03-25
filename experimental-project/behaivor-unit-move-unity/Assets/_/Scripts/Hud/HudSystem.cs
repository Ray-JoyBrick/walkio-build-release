namespace JoyBrick.Walkio.Game
{
    using System;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;

    [DisableAutoCreation]
    public class HudSystem : SystemBase
    {
        private IAssetLoadingService _assetLoadingService;

        private GameObject _hudGraphInstance;
        private GameObject _hudInstance;

        protected override void OnCreate()
        {
            base.OnCreate();

            var als = World.GetOrCreateSystem<AssetLoadingSystem>();
            _assetLoadingService = als as IAssetLoadingService;
            
            _assetLoadingService.LoadAssetAsync<GameObject>("Hud Graph", (graphPrefab) =>
            {
                _hudGraphInstance = GameObject.Instantiate(graphPrefab);
                
                //
                _assetLoadingService.LoadAssetAsync<GameObject>("Hud", (hudPrefab) =>
                {
                    //
                    _hudInstance = GameObject.Instantiate(hudPrefab);
                });
            });
        }
        
        protected override void OnUpdate()
        {
        }
    }
}