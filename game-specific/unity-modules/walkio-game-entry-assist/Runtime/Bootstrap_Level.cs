namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    // using UIWidgets;
    using UniRx;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;
    using UnityEngine.SceneManagement;

#if WALKIO_LEVEL_ASSIST
    using GameLevelAssist = JoyBrick.Walkio.Game.Level.Assist;
#endif

    public partial class Bootstrap
#if WALKIO_LEVEL_ASSIST
        : GameLevelAssist.IGridWorldProvider
#endif
    {
        //
        public GameObject obstaclePrefab;

        //
        private Mesh _combinedObstacleMesh = null;
        // private List<GameObject> _createdInstances;

        public void SetCachedObstacleMeshContext(List<float3> positions)
        {
            var createdInstances =
                positions.Select(p =>
                    {
                        var createdInstance = GameObject.Instantiate(obstaclePrefab, (Vector3) p, Quaternion.identity);

                        return createdInstance;
                    })
                    .ToList();

            var combine = new CombineInstance[createdInstances.Count];

            int i = 0;
            while (i < createdInstances.Count)
            {
                var mf = createdInstances[i].GetComponent<MeshFilter>();
                combine[i].mesh = mf.sharedMesh;
                combine[i].transform = mf.transform.localToWorldMatrix;
                mf.gameObject.SetActive(false);

                i++;
            }

            var mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.CombineMeshes(combine);
            _combinedObstacleMesh = mesh;

            createdInstances.ForEach(x => Destroy(x));
            createdInstances.Clear();
        }

        public Mesh GetCachedObstacleMesh()
        {
            return _combinedObstacleMesh;
            // return null;
        }
    }
}
