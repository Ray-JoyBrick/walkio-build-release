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
            // Should just get free seeker from the pool rather than relies on one
            var s = seeker.GetComponent<Pathfinding.Seeker>();
            
            s.StartMultiTargetPath(startPoints.ToArray(), targetPoint, true, path =>
            {
                if (path.error)
                {
                    _logger.Error(path.errorLog);
                    
                    // What to do if encountering error? As callback does not get called, is this going to impact
                    // flow field move part?
                }
                else
                {
                    if (path is MultiTargetPath mtp)
                    {
                        callback(teamId, timeTick, targetPoint, mtp.vectorPaths.ToList());
                    }
                }
            });            }
    }
}
