namespace JoyBrick.Walkio.Game.Move.FlowField
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
    public class LoadAssetSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadAssetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public void Construct()
        {
            _logger.Debug($"Module - LoadAssetSystem - Construct");
        }

        protected override void OnCreate()
        {
            _logger.Debug($"Module - LoadAssetSystem - OnCreate");

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
