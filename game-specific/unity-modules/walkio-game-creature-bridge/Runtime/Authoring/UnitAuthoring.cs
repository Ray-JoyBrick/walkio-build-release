namespace JoyBrick.Walkio.Game.Creature
{
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Rendering;
    using Unity.Transforms;
    using UnityEngine;

    public class UnitAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public bool isLeader;
        
        // public EMoveStyle moveStyle;

        // public int startPathIndex;
        // public int endPathIndex;
        public Vector3 startingPosition;

        //
        private EntityManager _entityManager;
        private Entity _entity;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            //
            dstManager.AddComponentData(entity, new Unit());
            dstManager.AddComponentData(entity, new UnitMovement());
            
            //
            dstManager.AddComponentData(entity, new UnitIndication());
            dstManager.RemoveComponent<RenderMesh>(entity);
            
            //
            if (isLeader)
            {
                //
                dstManager.AddComponentData(entity, new Leader());
                dstManager.AddComponentData(entity, new CopyTransformFromGameObject());

                // //
                // dstManager.RemoveComponent<RenderMesh>(entity);

                _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                _entity = entity;
            }

            //
            transform.position = new Vector3(startingPosition.x, startingPosition.y, startingPosition.z);

            // if (moveStyle == EMoveStyle.MoveOnWayPointPath)
            // {
            //     dstManager.AddComponentData(entity, new MoveOnWaypointPath
            //     {
            //         StartPathIndex = startPathIndex,
            //         EndPathIndex = endPathIndex,
            //         
            //         AtIndex = startPathIndex
            //     });
            // }
            // else if (moveStyle == EMoveStyle.MoveOnFlowField)
            // {
            //     //
            // }

            //
            var neutralForceAuthoring = GetComponent<NeutralForceAuthoring>();
            if (neutralForceAuthoring != null)
            {
                dstManager.AddComponentData(entity, new NeutralAbsorbable());
            }

            var teamForceAuthoring = GetComponent<TeamForceAuthoring>();
            if (teamForceAuthoring != null)
            {
                dstManager.AddComponentData(entity, new NeutralAbsorber());
            }
        }

        void Update()
        {
            if (isLeader)
            {
                _entityManager.SetComponentData(_entity, new Translation
                {
                    Value = (float3)transform.position
                });
            }
        }
    }
}
