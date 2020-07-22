namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    public partial class Bootstrap :
        IBootstrapAssistant
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Assistant")]
#endif
        public string assistableSceneName;

        private IBootstrapAssistable _assistable;

        private void AddSelfToAssistable(Scene scene)
        {
            _logger.Debug($"Bootstrap Assist - AddSelfToAssistable");

            if (scene.IsValid())
            {
                var assistables =
                    scene.GetRootGameObjects()
                        .Where(s => s.GetComponent<IBootstrapAssistable>() != null)
                        .ToList();

                if (assistables.Any())
                {
                    _logger.Debug($"Bootstrap Assist - AddSelfToAssistable - Found Assistables on scene: {scene.name}");
                    _assistable = assistables.First().GetComponent<IBootstrapAssistable>();
                    _assistable?.AddAssistant((IBootstrapAssistant) this);
                }
            }
        }

        void HandleSceneLoad()
        {
            //
            var sceneLoadedObservable =
                Observable.FromEvent<UnityAction<Scene, LoadSceneMode>, Tuple<Scene, LoadSceneMode>>(
                    h => (x, y) => h(Tuple.Create(x, y)),
                    h => SceneManager.sceneLoaded += h,
                    h => SceneManager.sceneLoaded -= h);
            sceneLoadedObservable
                .Subscribe(x =>
                {
                    var (scene, _) = x;
                    //
                    if (string.CompareOrdinal(scene.name, assistableSceneName) == 0)
                    {
                        AddSelfToAssistable(scene);
                    }
                })
                .AddTo(_compositeDisposable);
        }

        public void ShowPoints(int groupId, List<Vector3> points, float timeInSeconds)
        {
            _ShowPoints(groupId, points, timeInSeconds);
        }
    }
}
