namespace JoyBrick.Walkio.Game.AI
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Pathfinding;
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;

    public class TeamSet : MonoBehaviour
    {
        public GameObject teamLeader;
        public float moveSpeed;

        public Vector3 nextTargetRange;
        
        private Seeker _seeker;

        private bool _workOnNextTarget;
        private bool _canPickNextTarget;

        private Vector3 _nextTarget;

        private int _currentOnPathIndex;
        private readonly List<Vector3> _onPathPoints = new List<Vector3>();
        private bool _canMove;

        void Start()
        {
            _seeker = GetComponent<Seeker>();

            _canPickNextTarget = true;
            
            // All the steps here are experimental for testing A* path.
            StartCoroutine(FindingNextTarget());
            StartCoroutine(FindWaypointOnPath());
            StartCoroutine(MoveOnPath());
            
            // The path planning should be done by AI Planner
            // Planned path is from the examination of force heat map
            // The plan will be updated every few seconds
            // It is possible that the use of reactive AI will have immediate change, so both AI Planner and
            // Behavior Tree might be necessary
            // Then team leader check if path planned is reachable by using A*
            // Once the path if valid, which should be the case, use flow field as the actual resort to move agent
            // While each agent is moving, use PBD or simple flock to simulate crowd movement
        }
        
        IEnumerator FindingNextTarget()
        {
            while (true)
            {
                if (_canPickNextTarget)
                {
                    _nextTarget = new Vector3(Random.Range(0, nextTargetRange.x), 0, Random.Range(0, nextTargetRange.z));
                    _canPickNextTarget = false;
                    _workOnNextTarget = true;
                }
                
                yield return null;
            }
        }

        IEnumerator FindWaypointOnPath()
        {
            while (true)
            {
                if (_workOnNextTarget)
                {
                    _workOnNextTarget = false;
                    _seeker.StartPath(teamLeader.transform.position, _nextTarget, path =>
                    {
                        _onPathPoints.Clear();
                        _onPathPoints.AddRange(path.vectorPath);
                        _canMove = true;
                        _currentOnPathIndex = 0;
                        // foreach (var p in path.vectorPath)
                        // {
                        //     Debug.Log($"p: {p}");
                        // }
                        //
                        // teamLeader.transform.position = path.vectorPath.Last();
                        //
                        // _canPickNextTarget = true;
                    });
                }
                
                // yield return new WaitForSeconds(3.0f + Random.Range(2.0f, 5.0f));
                yield return null;
            }
        }

        IEnumerator MoveOnPath()
        {
            while (true)
            {
                // TODO: Adjust how move is interpolated
                // TODO: The condition is confusing
                if (!_workOnNextTarget && _onPathPoints.Count > 0 && _canMove && _currentOnPathIndex >= 0)
                {
                    teamLeader.transform.position = Vector3.MoveTowards(teamLeader.transform.position,
                        _onPathPoints[_currentOnPathIndex], Time.deltaTime * moveSpeed);

                    var nearPathPoint =
                        Vector3.SqrMagnitude(teamLeader.transform.position - _onPathPoints[_currentOnPathIndex]) <
                        0.04f;

                    if (nearPathPoint)
                    {
                        ++_currentOnPathIndex;
                    }

                    if (_currentOnPathIndex >= _onPathPoints.Count)
                    {
                        _canMove = false;
                        _canPickNextTarget = true;
                        _onPathPoints.Clear();
                        _currentOnPathIndex = -1;
                    }
                }
                
                yield return null;
            }
        }
    }
}
