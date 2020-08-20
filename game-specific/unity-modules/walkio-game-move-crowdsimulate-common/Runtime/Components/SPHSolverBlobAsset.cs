namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using Unity.Entities;
    using UnityEngine;

    public struct SPHSolverContext
    {
        public ComputeShader SPHShader;
        public float KernelRadius;
        public float Stiffness;
        public float RestDensity;
        public float Viscosity;

        public float KPoly6Const;
    }

    public struct SPHSolverBlobAsset
    {
        // public BlobArray<FlowFieldTileContext> FlowFieldTileContextArray;
        public BlobPtr<SPHSolverContext> Context;
    }
}
