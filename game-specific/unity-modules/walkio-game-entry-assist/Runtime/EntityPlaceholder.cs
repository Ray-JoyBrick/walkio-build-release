namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;

    public class EntityPlaceholder :
        MonoBehaviour
    {
        public Entity RefEntity { get; set; }

        private EntityManager _entityManager;
        
        void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        void Update()
        {
            if (RefEntity != Entity.Null)
            {
                var hasTranslationComp = _entityManager.HasComponent<Translation>(RefEntity);
                
                if (hasTranslationComp)
                {
                    var translation = _entityManager.GetComponentData<Translation>(RefEntity);

                    transform.position = (Vector3) translation.Value;
                }
            }
        }

        private void OnDestroy()
        {
            RefEntity = Entity.Null;
        }
    }
}
