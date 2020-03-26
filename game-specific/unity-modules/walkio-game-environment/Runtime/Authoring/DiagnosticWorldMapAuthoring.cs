namespace JoyBrick.Walkio.Game.Environment
{

    using Unity.Entities;
    using UnityEngine;

    public class DiagnosticWorldMapAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public Material material;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            
        }
    }
}