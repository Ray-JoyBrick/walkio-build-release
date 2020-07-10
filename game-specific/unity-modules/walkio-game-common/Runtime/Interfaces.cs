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

    public interface IServiceManagement
    {
        IObservable<int> LoadAppHud { get; }

        void LoadAppHudDone();
        
        IObservable<int> LoadPreparationHud { get; }
        IObservable<int> UnloadPreparationHud { get; }
    }

    public interface ICommandHandler
    {
        void LoadZone(int index);
    }

    //
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
    
    public class FlowControlContext
    {
        public string Name { get; set; }

        public override string ToString()
        {
            var desc = $"Name: {Name}";

            return desc;
        }
    }
    
    public interface IFlowControl
    {
        IObservable<FlowControlContext> LoadingAsset { get; }
        IObservable<FlowControlContext> DoneLoadingAsset { get; }

        IObservable<FlowControlContext> SettingAsset { get; }
        IObservable<FlowControlContext> DoneSettingAsset { get; }

        IObservable<FlowControlContext> AllDoneSettingAsset { get; }

        IObservable<FlowControlContext> CleaningAsset { get; }
        
        //
        void StartLoadingAsset(string flowName);

        //
        void FinishLoadingAsset(FlowControlContext context);
        void StartSetting(FlowControlContext context);
        void FinishSetting(FlowControlContext context);

        void FinishAllSetting(FlowControlContext context);
        
        //
        void StartCleaningAsset(string flowName);

    }

    public interface IAStarPathService
    {
        void CalculatePath(int teamId, int timeTick, List<Vector3> startPoints, Vector3 targetPoint,
            System.Action<int, int, Vector3, List<List<Vector3>>> callback);
    }

    public interface IGameSettingProvider
    {
        GameSettings GameSettings { get; }
    }

    public interface IEcsSettingProvider
    {
        GameObjectConversionSettings GameObjectConversionSettings { get; }
    }

    public interface ISceneProvider
    {
        Camera SceneCamera { get; }
    }

    public interface ISceneAssistProvider
    {
        Vector2Int SegmentCount { get; }
        Vector3 StartOffset { get; }
        Material PlanMaterial { get; }
        
        Material GroupPlaneMaterial { get; }
    }

    public interface IGizmoService
    {
        void AddEntityToGameObject(Entity entity);
        void RemoveEntityFromGameObject(Entity entity);
    }
    
    // public interface IInfoPresenter
    // {
    //     
    // }

    public interface IGroupingProvider
    {
        int GroupingId { get; }
    }
}
