﻿namespace Game
{
    using Unity.Entities;
    using UnityEngine;

    public class TeamForceAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TeamForce());
        }
    }
}

