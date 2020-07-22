namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalOpus.MB.Core;
    using UIWidgets;
    using UniRx;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;
    using UnityEngine.SceneManagement;

#if WALKIO_LEVEL_ASSIST
    using GameLevelAssist = JoyBrick.Walkio.Game.Level.Assist;
#endif

    public partial class Bootstrap :
        GameLevelAssist.IGridWorldProvider
    {
        public GameObject obstaclePrefab;
        public MB3_MeshBaker meshBaker;

        private Mesh _combinedObstacleMesh = null;

        private List<GameObject> _createdInstances;
        public void SetCachedObstacleMeshContext(List<float3> positions)
        {
            _createdInstances =
                positions.Select(p =>
                    {
                        var createdInstance = GameObject.Instantiate(obstaclePrefab, (Vector3) p, Quaternion.identity);

                        return createdInstance;
                    })
                    .ToList();

            CombineInstance[] combine = new CombineInstance[_createdInstances.Count];

            int i = 0;
            while (i < _createdInstances.Count)
            {
                var mf = _createdInstances[i].GetComponent<MeshFilter>();
                combine[i].mesh = mf.sharedMesh;
                combine[i].transform = mf.transform.localToWorldMatrix;
                mf.gameObject.SetActive(false);

                i++;
            }

            var mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.CombineMeshes(combine);
            _combinedObstacleMesh = mesh;

            _createdInstances.ForEach(x => Destroy(x));
            _createdInstances.Clear();


            // // meshBaker.AddDeleteGameObjects(createdInstances.ToArray(), null, true);
            // meshBaker.AddDeleteGameObjects(_createdInstances.ToArray(), null, true);
            // meshBaker.Apply(false,true,true,true,false,false,false,false,false);
            //
            // var meshCombinerSingle = meshBaker.meshCombiner as MB3_MeshCombinerSingle;
            //
            // if (meshCombinerSingle != null)
            // {
            //     _logger.Debug($"Bootstrap Assist - SetCachedObstacleMeshContext - can get mesh combiner");
            //
            //     _combinedObstacleMesh = meshCombinerSingle.GetMesh();
            //
            //     // createdInstances.ForEach(x => Destroy(x));
            // }
        }

        public Mesh GetCachedObstacleMesh()
        {
            // if (_createdInstances == null) return null;
            //
            // meshBaker.UpdateGameObjects(_createdInstances.ToArray());
            // meshBaker.Apply(false,true,true,true,false,false,false,false,false);
            //
            // var meshCombinerSingle = meshBaker.meshCombiner as MB3_MeshCombinerSingle;
            //
            // if (meshCombinerSingle != null)
            // {
            //     _logger.Debug($"Bootstrap Assist - SetCachedObstacleMeshContext - can get mesh combiner");
            //
            //     _combinedObstacleMesh = meshCombinerSingle.GetMesh();
            //
            //     // createdInstances.ForEach(x => Destroy(x));
            // }


            return _combinedObstacleMesh;
            // return null;
        }

    }
}
