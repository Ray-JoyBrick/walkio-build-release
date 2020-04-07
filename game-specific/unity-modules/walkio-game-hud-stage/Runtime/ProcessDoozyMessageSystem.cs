namespace JoyBrick.Walkio.Game.Hud.AppStage
{
    using System;
    using System.Threading.Tasks;
    using Doozy.Engine;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class ProcessDoozyMessageSystem : SystemBase
    {
        public GameCommon.IServiceManagement ServiceManagement { get; set; }
        
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        protected override void OnCreate()
        {
            base.OnCreate();

            Message.AddListener<GameEventMessage>(OnDoozyMessage);
        }

        private void OnDoozyMessage(GameEventMessage message)
        {
            if (message == null) return;

            var validMessage = string.IsNullOrEmpty(message.EventName);
            if (validMessage) return;

            var result = string.Compare(message.EventName, "Load Environment", StringComparison.Ordinal);
            if (result == 0)
            {
                Debug.Log($"ProcessDoozyMessageSystem - OnDoozyMessage - {message.EventName}");
                // var entity = EntityManager.CreateEntity(_eventEntityArchetype);
                // EntityManager.AddComponentData<Common.LoadWorldMapRequest>(entity, new Common.LoadWorldMapRequest
                // {
                //     WorldMapIndex = 0
                // });
            }
        }

        public void Setup()
        {
        }
        
        protected override void OnUpdate()
        {
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Message.RemoveListener<GameEventMessage>(OnDoozyMessage);

            _compositeDisposable?.Dispose();
        }
    }
}
