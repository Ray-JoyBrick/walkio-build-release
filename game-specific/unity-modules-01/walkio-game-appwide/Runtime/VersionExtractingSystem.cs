namespace JoyBrick.Walkio.Game.Appwide
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Cysharp.Threading.Tasks;
    using UniRx;
    // using UniRx.Async;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.Networking;
    using Common = JoyBrick.Walkio.Game.Common;
    
    [DisableAutoCreation]
    public class VersionExtractingSystem : SystemBase
    {
        //
        public ReactiveProperty<bool> AllModuleDoneSetup => new ReactiveProperty<bool>(false);
        public ReactiveProperty<string> BuildVersion => _buildVersion;
        private readonly ReactiveProperty<string> _buildVersion = new ReactiveProperty<string>("0.0.0.0");
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        protected override void OnCreate()
        {
            LoadStartData().ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(x =>
                {
                    //
                })
                .AddTo(_compositeDisposable);
        }

        private async Task LoadStartData()
        {
            Debug.Log($"VersionExtractingSystem - LoadStartData");

            //
            await FetchBuildVersion();

            // //
            // RestoreLanguage();
        }

        private static string StreamingAssetPath
        {
            get
            {
                Debug.Log($"VersionExtractingSystem - StreamingAssetPath");
                var path = "";
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    path = $"{Application.streamingAssetsPath}";
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    path = $"jar:file://{Application.dataPath}!/assets";
                }
                else if (
                    Application.platform == RuntimePlatform.LinuxEditor ||
                    Application.platform == RuntimePlatform.LinuxPlayer ||
                    Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WindowsPlayer ||
                    Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer)
                {
                    path = $"file://{Application.dataPath}/StreamingAssets";
                }

                return path;
            }
        }

        // From Stackoverflow
        // https://stackoverflow.com/questions/50400634/unity-streaming-assets-ios-not-working
        private async Task FetchBuildVersion()
        {
            Debug.Log($"VersionExtractingSystem - FetchBuildVersion");

            var filePath = Path.Combine(StreamingAssetPath, "build.txt");
            if (filePath.Contains("://"))
            {
                using (var uwr = UnityWebRequest.Get(filePath))
                {
                    var uwrao = await uwr.SendWebRequest();
                    var text = uwrao.downloadHandler.text;
                    BuildVersion.Value = text;

                    Debug.Log($"VersionExtractingSystem - FetchBuildVersion - build version: {text}");
                }
            }
            else
            {
                var text = System.IO.File.ReadAllText(filePath);
                BuildVersion.Value = text;

                Debug.Log($"VersionExtractingSystem - FetchBuildVersion - build version: {text}");
            }
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
