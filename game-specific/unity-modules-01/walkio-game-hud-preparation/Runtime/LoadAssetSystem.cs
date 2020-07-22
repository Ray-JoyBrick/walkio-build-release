namespace JoyBrick.Walkio.Game.Hud.Preparation
{
    using UniRx;
    using Unity.Entities;

    public class LoadAssetSystem :
        SystemBase
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        protected override void OnUpdate()
        {
        }
    }
}
