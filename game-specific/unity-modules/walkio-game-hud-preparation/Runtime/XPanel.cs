namespace JoyBrick.Walkio.Game.Hud.Preparation
{
    using System;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;

    public class XPanel : MonoBehaviour
    {
        public UnityEngine.UI.Button loadZone00;
        public UnityEngine.UI.Button loadZone01;
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private void Start()
        {
            var hudSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<HudLoadSceneSystem>();
            
            loadZone00.onClick.AsObservable()
                .Subscribe(x =>
                {
                    //
                    hudSystem.LoadZone(0);
                })
                .AddTo(_compositeDisposable);

            loadZone01.onClick.AsObservable()
                .Subscribe(x =>
                {
                    //
                    hudSystem.LoadZone(1);
                })
                .AddTo(_compositeDisposable);
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
