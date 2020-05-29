namespace JoyBrick.Walkio.Game.Hud.App
{
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class ProcessDoozyMessageSystem : SystemBase
    {
        public GameCommon.IServiceManagement ServiceManagement { get; set; }
        
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public void Setup()
        {
        }
        
        protected override void OnUpdate()
        {
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
}
