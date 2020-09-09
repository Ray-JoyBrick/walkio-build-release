namespace JoyBrick.Walkio.Game.Move.CrowdSimulate
{
    using Unity.Entities;

    [RequireComponentTag(typeof(CrowdSimulationWorld))]
    public struct CrowdSimulationWorldProperty : IComponentData
    {
        public BlobAssetReference<SPHSolverBlobAsset> SPHSolverBlobAssetRef;
    }
}
