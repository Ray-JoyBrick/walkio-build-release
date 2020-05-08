namespace JoyBrick.Walkio.Game.Battle
{
    using HellTap.PoolKit;
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;

    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameInputControl = JoyBrick.Walkio.Game.InputControl;

    public class TeamLeaderAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity,
        
        IPoolKitListener
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(TeamLeaderAuthoring));
        
        private Entity _entity = Entity.Null;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            //
            _entity = entity;
            
            //
            dstManager.AddComponentData(entity, new CopyTransformFromGameObject());
            dstManager.AddComponentData<GameCommon.StageUse>(entity, new GameCommon.StageUse());
            
#if UNITY_EDITOR
            
            dstManager.SetName(entity, "Team Leader");
            
#endif
        }
        
        //
        public void OnSpawn(Pool pool)
        {
            _logger.Debug($"TeamLeaderAuthoring - OnSpawn - pool: {pool.name}");
            if (_entity != Entity.Null)
            {
                World.DefaultGameObjectInjectionWorld.EntityManager.RemoveComponent<Disabled>(_entity);

#if UNITY_EDITOR

                var entityName = World.DefaultGameObjectInjectionWorld.EntityManager.GetName(_entity);
                World.DefaultGameObjectInjectionWorld.EntityManager.SetName(_entity, $"{entityName}");

#endif
            }
        }

        public void OnDespawn()
        {
            _logger.Debug($"TeamLeaderAuthoring - OnDespawn");
            if (_entity != Entity.Null)
            {
                World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<Disabled>(_entity);

#if UNITY_EDITOR

                var entityName = World.DefaultGameObjectInjectionWorld.EntityManager.GetName(_entity);
                World.DefaultGameObjectInjectionWorld.EntityManager.SetName(_entity, $"{entityName} - Disabled");

#endif
            }

            // Dependencies to Game Input Control, better approach to get rid of this?
            var playerInputControl = GetComponent<GameInputControl.PlayerInputControl>();
            if (playerInputControl != null)
            {
                Destroy(playerInputControl);
            }
            
            var formRouteFromInput = GetComponent<GameInputControl.FormRouteFromInput>();
            if (formRouteFromInput != null)
            {
                Destroy(formRouteFromInput);
            }
            
        }
    }
}
