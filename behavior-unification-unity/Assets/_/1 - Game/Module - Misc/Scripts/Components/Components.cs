namespace Game
{
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
    public struct TheEnvironment : IComponentData
    {
    }
    
    public struct NeutralUnitSpawn : IComponentData
    {
        public float IntervalMax;
        public float CountDown;
    }

    public struct TeamUnitSpawn : IComponentData
    {
        public float IntervalMax;
        public float CountDown;
    }
    
    //

    public struct TeamForce : IComponentData
    {
    }

    public struct Leader : IComponentData
    {
    }
    
    public struct NeutralForce : IComponentData
    {
    }

    public struct Unit : IComponentData
    {
    }

    public struct UnitMovement : IComponentData
    {
        public float3 Direction;
        public quaternion Rot;
    }
    
    //
    public struct UnitAnimation : IComponentData
    {
        public Matrix4x4 matrix;
    }

    public struct NeutralAbsorbable : IComponentData
    {
        public bool Absorbed;
    }
    
    //
    public struct NeutralAbsorber : IComponentData
    {
        
    }
    
    //
    public struct MoveOnWaypointPath : IComponentData
    {
        public int StartPathIndex;
        public int EndPathIndex;

        public int AtIndex;
    }

    //
    public struct WaypointPathIndexPair
    {
        public int StartIndex;
        public int EndIndex;
    }

    public struct WaypointPathBlobAsset
    {
        public BlobArray<WaypointPathIndexPair> WaypointPathIndexPairs;
        public BlobArray<float3> Waypoints;
    }

    public struct LevelWaypointPathLookup : IComponentData
    {
        public BlobAssetReference<WaypointPathBlobAsset> WaypointPathBlobAssetRef;
    }
}
