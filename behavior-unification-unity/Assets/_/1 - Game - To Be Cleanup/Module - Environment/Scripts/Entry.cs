// namespace JoyBrick.Walkio.Game.Environment
// {
//     using System;
//     using HellTap.PoolKit;
//     using UniRx;
//     using UnityEngine;

//     public class Entry : MonoBehaviour
//     {
//         //
//         private Pool _teamForcePool;
//         private Pool _freeUnitPool;
        
//         //
//         private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
//         private void Start()
//         {
//             SetupTeamForcePool();
//             SetupFreeUnitPool();
            
//             Observable.Timer(System.TimeSpan.FromMilliseconds(5500))
//                 .Subscribe(_ =>
//                 {
//                     //
//                     var teamForce = _teamForcePool.Spawn("Team Force");
//                 })
//                 .AddTo(_compositeDisposable);
//         }
        
//         private void SetupTeamForcePool()
//         {
//             _teamForcePool = PoolKit.Find("Team Force Pool");
            
//             var onPoolSpawnObservable =
//                 Observable
//                     .FromEvent<Pool.OnPoolSpawnDelegate, (Transform, Pool)>(
//                         h => (t, p) => h.Invoke((t, p)),
//                         h => _teamForcePool.onPoolSpawn += h,
//                         h => _teamForcePool.onPoolSpawn -= h);

//             var onPoolDespawnObservable =
//                 Observable
//                     .FromEvent<Pool.OnPoolDespawnDelegate, (Transform, Pool)>(
//                         h => (t, p) => h.Invoke((t, p)),
//                         h => _teamForcePool.onPoolDespawn += h,
//                         h => _teamForcePool.onPoolDespawn -= h);

//             var combined = onPoolSpawnObservable.Merge(onPoolDespawnObservable);
//             combined
//                 .Subscribe(x =>
//                 {
//                     //
//                     Debug.Log($"Team Force Pool: spawn or despawn");
//                 })
//                 .AddTo(_compositeDisposable);
//         }
        
//         private void SetupFreeUnitPool()
//         {
//             _freeUnitPool = PoolKit.Find("Free Unit Pool");
            
//             var onPoolSpawnObservable =
//                 Observable
//                     .FromEvent<Pool.OnPoolSpawnDelegate, (Transform, Pool)>(
//                         h => (t, p) => h.Invoke((t, p)),
//                         h => _freeUnitPool.onPoolSpawn += h,
//                         h => _freeUnitPool.onPoolSpawn -= h);

//             var onPoolDespawnObservable =
//                 Observable
//                     .FromEvent<Pool.OnPoolDespawnDelegate, (Transform, Pool)>(
//                         h => (t, p) => h.Invoke((t, p)),
//                         h => _freeUnitPool.onPoolDespawn += h,
//                         h => _freeUnitPool.onPoolDespawn -= h);

//             var combined = onPoolSpawnObservable.Merge(onPoolDespawnObservable);
//             combined
//                 .Subscribe(x =>
//                 {
//                     //
//                     Debug.Log($"Free Unit Pool: spawn or despawn");
//                 })
//                 .AddTo(_compositeDisposable);
//         }


//         private void OnDestroy()
//         {
//             _compositeDisposable?.Dispose();
//         }
//     }
// }
