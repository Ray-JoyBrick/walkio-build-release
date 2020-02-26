// using System.Collections;
// using System.Collections.Generic;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Physics;
// // using RaycastHit = UnityEngine.RaycastHit;
//
// public class CheckSystem : JobComponentSystem
// {
//     [BurstCompile]
//     public struct RaycastJob : IJobParallelFor
//     {
//         //
//         [ReadOnly] public CollisionWorld world;
//         [ReadOnly] public NativeArray<RaycastInput> inputs;
//         
//         //
//         public NativeArray<RaycastHit> results;
//  
//         public unsafe void Execute(int index)
//         {
//             world.CastRay(inputs[index], out var hit);
//             results[index] = hit;
//         }
//     }
//     
//     protected override JobHandle OnUpdate(JobHandle inputDeps)
//     {
//         
//         return inputDeps;
//     }
// }

namespace JoyBrick.Walkio.Game.Main
{
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Physics;

    [DisableAutoCreation]
    public class CheckSystem : JobComponentSystem
    {
        private BlobAssetReference<Collider> collider;

        protected override void OnCreate()
        {
            // BoxCollider.Create()
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return inputDeps;
        }
    }
}