namespace JoyBrick.Walkio.Game.Level.Assist
{
    using System.Collections.Generic;
    using Unity.Mathematics;
    using UnityEngine;

    public interface IGridWorldProvider
    {
        void SetCachedObstacleMeshContext(List<float3> positions);
        Mesh GetCachedObstacleMesh();
    }
}
