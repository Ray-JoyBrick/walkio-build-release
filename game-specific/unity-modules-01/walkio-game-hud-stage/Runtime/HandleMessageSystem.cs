namespace JoyBrick.Walkio.Game.Hud.Stage
{
    using System;
    // using Doozy.Engine;
    using Unity.Entities;
    using UnityEngine;
    
    using Common = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class HandleMessageSystem : SystemBase
    {
        private EntityArchetype _eventEntityArchetype;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _eventEntityArchetype = 
                EntityManager.CreateArchetype(
                    typeof(Common.LoadWorldMapRequest));

            // Message.AddListener<GameEventMessage>(OnDoozyMessage);
        }

        // private void OnDoozyMessage(GameEventMessage message)
        // {
        //     if (message == null) return;
        //
        //     var validMessage = string.IsNullOrEmpty(message.EventName);
        //     if (validMessage) return;
        //
        //     var result = string.Compare(message.EventName, "Load Environment", StringComparison.Ordinal);
        //     if (result == 0)
        //     {
        //         Debug.Log($"HandleMessageSystem - OnDoozyMessage - {message.EventName}");
        //         var entity = EntityManager.CreateEntity(_eventEntityArchetype);
        //         EntityManager.AddComponentData<Common.LoadWorldMapRequest>(entity, new Common.LoadWorldMapRequest
        //         {
        //             WorldMapIndex = 0
        //         });                
        //     }
        // }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            // Message.RemoveListener<GameEventMessage>(OnDoozyMessage);
        }

        protected override void OnUpdate()
        {
        }
    }
}
