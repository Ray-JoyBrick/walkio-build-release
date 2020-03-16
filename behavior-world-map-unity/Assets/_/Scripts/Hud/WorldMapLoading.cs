namespace JoyBrick.Walkio.Game.Hud
{
    using System;
    using Unity.Entities;
    using UnityEngine;
    
    using Environment = JoyBrick.Walkio.Game.Environment;

    public class WorldMapLoading : MonoBehaviour
    {
        public UnityEngine.UI.Button _loadMap;
        
        //
        private EntityManager _entityManager;
        private EntityArchetype _worldMapArchetype;

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _worldMapArchetype = 
                _entityManager.CreateArchetype(
                    typeof(Environment.LoadWorldMapRequest));
        }
        
        //
        public void HandleLoadMapButtonPressed(int worldMapIndex)
        {
            var entity = _entityManager.CreateEntity(_worldMapArchetype);
            _entityManager.AddComponentData<Environment.LoadWorldMapRequest>(entity, new Environment.LoadWorldMapRequest
            {
                WorldMapIndex = worldMapIndex
            });
        }

        private void OnDestroy()
        {
        }
    }
}