namespace JoyBrick.Walkio.Game.Environment.Creature
{
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
    using HellTap.PoolKit;
#endif
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public class LeaderAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LeaderAuthoring));
        
        private Entity _entity = Entity.Null;

        public Entity OwnedEntity => _entity;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            //
            _entity = entity;
            
            //
            dstManager.AddComponentData(entity, new CopyTransformFromGameObject());

            //
            dstManager.AddComponentData(entity, new Leader());
            
            dstManager.AddComponentData(entity, new MonitorFlowFieldTileChange());
            dstManager.AddComponentData(entity, new FlowFieldGoal());
            
            //
            dstManager.AddComponentData<GameCommon.StageUse>(entity, new GameCommon.StageUse());

#if UNITY_EDITOR

            dstManager.SetName(entity, "Team Leader");
            
#endif
        }

        //
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
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
#endif


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
        }
    }
}
