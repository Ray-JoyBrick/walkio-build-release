namespace JoyBrick.Walkio.Game
{
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Rendering;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.Serialization;

    public struct Player : IComponentData
    {
    }

    public struct PlayerInput : IComponentData
    {
        public float2 MoveInput;
    }
    
    public struct InputDeviceIdBufferElement : IBufferElementData
    {
        public int DeviceId;
    }

    public struct SpawnCorrespondingTeam : IComponentData
    {
    }

    public class PlayerAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public Mesh playerInputMesh;
        public Material playerInputMaterial;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<Player>(entity);
            dstManager.AddComponentData<PlayerInput>(entity, new PlayerInput
            {
                MoveInput = float2.zero
            });
            dstManager.AddComponentData<PlayerMovementRequest>(entity, new PlayerMovementRequest
            {
                Direction = float2.zero,
                Strength = 0
            });
            
            //
            var deviceIdBuffers = dstManager.AddBuffer<InputDeviceIdBufferElement>(entity);
            deviceIdBuffers.Add(new InputDeviceIdBufferElement {DeviceId = 0});

            dstManager.AddComponent<SpawnCorrespondingTeam>(entity);

            dstManager.AddComponentData(entity, new LocalToWorld());
            dstManager.AddComponentData(entity, new Translation());
            
            dstManager.AddSharedComponentData<RenderMesh>(entity, new RenderMesh
            {
                mesh = playerInputMesh,
                material = playerInputMaterial
            });
            dstManager.AddComponentData<RenderBounds>(entity, new RenderBounds());
        }
    }
}
