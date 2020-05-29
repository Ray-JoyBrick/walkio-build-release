namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pathfinding;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    
    public partial class Bootstrap :
        GameCommon.IAStarPathService
    {
        public GameObject seeker;

        public void CalculatePath(int teamId, int timeTick, List<Vector3> startPoints, Vector3 targetPoint, Action<int, int, Vector3, List<List<Vector3>>> callback)
        {
            var s = seeker.GetComponent<Pathfinding.Seeker>();
            
            s.StartMultiTargetPath(startPoints.ToArray(), targetPoint, true, path =>
            {
                if (path.error)
                {
                }
                else
                {
                    var mtp = path as MultiTargetPath;

                    if (mtp != null)
                    {
                        // foreach (var vp in mtp.vectorPaths)
                        // {
                        //     var desc = vp.Aggregate("", (acc, next) => { return $"{acc} -> {next}"; });
                        //     Debug.Log(desc);
                        // }

                        callback(teamId, timeTick, targetPoint, mtp.vectorPaths.ToList());
                    }
                }
            });            }
    }
}
