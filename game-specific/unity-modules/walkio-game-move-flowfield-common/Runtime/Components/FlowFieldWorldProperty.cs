namespace JoyBrick.Walkio.Game.Move.FlowField.Common
{
    using Unity.Entities;

    public struct FlowFieldWorldProperty : IComponentData
    {
        public BlobAssetReference<FlowFieldTileBlobAsset> FlowFieldTileBlobAssetRef;
    }
}
