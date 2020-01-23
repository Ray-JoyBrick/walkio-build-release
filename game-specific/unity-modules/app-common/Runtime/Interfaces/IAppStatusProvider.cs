namespace JoyBrick.Walkio.Game.App.Common.Main
{
    using UniRx;

    public interface IAppStatusProvider
    {
        ReactiveProperty<bool>  AllModuleDoneSetup { get; }
        ReactiveProperty<string> BuildVersion { get; }
    }
}