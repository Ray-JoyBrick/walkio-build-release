namespace JoyBrick.Walkio.Game.Common
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public interface IAssetLoadingService
    {
        void LoadAssets<T>(string label, System.Action<IList<T>> callback);  
        void LoadAsset<T>(string address, System.Action<T> callback);
        
        void LoadAssets(IEnumerable<string> addresses, System.Action<IEnumerable<UnityEngine.Object>> callback);
        void LoadAssets(IEnumerable<string> addresses, EntityArchetype toCreate);
    }

    public interface IEnvironmentSetupRequester
    {
        IObservable<int> InitializingEnvironment { get; }

        void SetEnvironmentData(ScriptableObject scriptableObject);
    }
    
    //
    public interface IWorldLoadingRequester
    {
        IObservable<int> LoadingWorld { get; }
        // Subject<int> RequestToLoadWorld(int index);

        void SetZoneScene(Scene scene);
        void SetupPathfindingData(TextAsset textAsset);
    }
}
