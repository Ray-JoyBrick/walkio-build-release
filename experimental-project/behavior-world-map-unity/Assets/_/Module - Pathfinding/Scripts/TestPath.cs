using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

public class TestPath : MonoBehaviour
{
    public GameObject seeker;
    
    void Start()
    {
        var s = seeker.GetComponent<Pathfinding.Seeker>();

        // s.StartPath(new Vector3(2.0f, 0, 3.0f), new Vector3(7.0f, 0, 19.0f), path =>
        // {
        //     if (path.error)
        //     {
        //     }
        //     else
        //     {
        //         var desc = path.vectorPath.Aggregate("", (acc, next) => { return $"{acc} -> {next}"; });
        //         Debug.Log(desc);
        //     }
        // });
        
        var targetPosition = new Vector3(16.0f, 0, 30.0f);
        var sourcePositions = new List<Vector3>
        {
            new Vector3(6.0f, 0, 5.0f),
            new Vector3(4.0f, 0, 4.0f),
            new Vector3(3.0f, 0, 7.0f),
            new Vector3(3.0f, 0, 8.0f)
        };
        s.StartMultiTargetPath(sourcePositions.ToArray(), targetPosition, true, path =>
        {
            if (path.error)
            {
            }
            else
            {
                var mtp = path as MultiTargetPath;

                if (mtp != null)
                {
                    foreach (var vp in mtp.vectorPaths)
                    {
                        var desc = vp.Aggregate("", (acc, next) => { return $"{acc} -> {next}"; });
                        Debug.Log(desc);
                    }
                }
                
                // var desc = path.vectorPath.Aggregate("", (acc, next) => { return $"{acc} -> {next}"; });
                // Debug.Log(desc);
            }
        });
    }
}
