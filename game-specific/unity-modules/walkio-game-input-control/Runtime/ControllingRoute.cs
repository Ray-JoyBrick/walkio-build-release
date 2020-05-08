namespace JoyBrick.Walkio.Game.InputControl
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Pathfinding;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Random = UnityEngine.Random;

    // This class is used for both npc and player
    [RequireComponent(typeof(AILerp))]
    public class ControllingRoute : MonoBehaviour
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(ControllingRoute));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        private readonly Subject<int> _notifyGoalPathReset = new Subject<int>();
        
        //
        private AILerp _aiLerp;
        
        //
        private readonly List<Vector3> _goalPath = new List<Vector3>();

        private int _currentGoalPathIndex = -1;

        // private bool _autoGenerate = true;

        public bool autoGenerate = true;

        private Coroutine _timedGeneration;
        
        void Start()
        {
            _aiLerp = GetComponent<AILerp>();
            
            Assert.IsNotNull(_aiLerp);

            // _notifyGoalPathReset
            //     .Subscribe(x =>
            //     {
            //         //
            //     })
            //     .AddTo(_compositeDisposable);
        }

        public void AddPositionToPath(Vector3 pos, bool clearPrevious)
        {
            if (clearPrevious)
            {
                _goalPath.Clear();
            }
            
            _goalPath.Add(pos);
            _currentGoalPathIndex = 0;
            
            // _notifyGoalPathReset.OnNext(1);

            InformAStarPathGoalChanged();
        }

        private void InformAStarPathGoalChanged()
        {
            _aiLerp.destination = _goalPath[_currentGoalPathIndex];
        }

        private void OnEnable()
        {
            _timedGeneration = StartCoroutine(TimedGeneration());
        }

        private void OnDisable()
        {
            StopCoroutine(_timedGeneration);
            _timedGeneration = null;
        }

        private IEnumerator TimedGeneration()
        {
            while (true)
            {
                if (autoGenerate)
                {
                    _goalPath.Clear();

                    var pathPointCount = Random.Range(1, 10);
                    for (var i = 0; i < pathPointCount; ++i)
                    {
                        var randomPoint = new Vector3(
                            Random.Range(0, 100),
                            0,
                            Random.Range(0, 100));
                        
                        _goalPath.Add(randomPoint);
                    }

                    _currentGoalPathIndex = 0;
                }
                
                yield return new WaitForSeconds(10.0f);
            }
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
