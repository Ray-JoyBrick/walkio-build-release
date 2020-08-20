namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using Unity.Entities;
    using UnityEngine;

    public class SPHSolverBlobAssetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public ComputeShader sphShader;

        public float kernelRadius;
        public float stiffness;
        public float restDensity;
        public float viscosity;

        public float kPoly6Const;
        // public float kr2;
        // public float inverseKr9;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
        }
    }
}