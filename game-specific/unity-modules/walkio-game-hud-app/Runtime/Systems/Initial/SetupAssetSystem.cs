namespace JoyBrick.Walkio.Game.Hud.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    [DisableAutoCreation]
    public class SetupAssetSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupAssetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public void Construct()
        {
            _logger.Debug($"Module - SetupAssetSystem - Construct");
        }

        protected override void OnCreate()
        {
            _logger.Debug($"Module - SetupAssetSystem - OnCreate");

            base.OnCreate();
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
