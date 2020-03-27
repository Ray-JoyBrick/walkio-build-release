namespace JoyBrick.Walkio.Game.Environment
{
    using System;
    using UniRx;
    using Unity.Entities;
    using GameCommon = JoyBrick.Walkio.Game.Common;
    
    public class LoadWorldSystem :
        SystemBase,
        GameCommon.IWorldLoading
    {
        protected override void OnUpdate()
        {
        }

        public IObservable<int> LoadingWorld => Observable.Empty<int>();
    }
}