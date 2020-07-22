namespace JoyBrick.Walkio.Game
{
    using UniRx;
    using UniRx.Diagnostics;

    public partial class Bootstrap
    {
        private void SetupUniRxLogger()
        {
            ObservableLogger.Listener
                .SubscribeOn(Scheduler.ThreadPool)
                .LogToUnityDebug()
                .AddTo(_compositeDisposable);

            ObservableLogger.Listener
                // .Where(x => x.LogType == LogType.Exception)
                .Subscribe(x =>
                {
                    // ObservableWWW.Post("", null).Subscribe();
                })
                .AddTo(_compositeDisposable);
        }
    }
}
