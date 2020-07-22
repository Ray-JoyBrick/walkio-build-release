namespace JoyBrick.Walkio.Game
{
    using System.Collections;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;

    public partial class Bootstrap : MonoBehaviour
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(Bootstrap));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        void Awake()
        {
            //
            SetupUniRxLogger();

            //
            SetupAppCenterCrashes();
            
            //
            SetupRemoteConfiguration();
            
            //
            SetupAuth();
        }

        void Start()
        {
            SetupAddressable();
        }

        void OnDestroy()
        {
            CleanUpEcsWorldContext();

            _compositeDisposable?.Dispose();
        }
    }
}
