namespace JoyBrick.Walkio.Game
{
    using System.IO;
    using System.Threading.Tasks;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Networking;

    //
    // using GameLevel = JoyBrick.Walkio.Game.Level;
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;

    public partial class Bootstrap
    {
        //
        public ReactiveProperty<string> BuildVersion => _buildVersion;
        private readonly ReactiveProperty<string> _buildVersion = new ReactiveProperty<string>("0.0.0.0");

        private static string StreamingAssetPath
        {
            get
            {
                _logger.Debug($"Bootstrap - StreamingAssetPath - get");
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
            _logger.Debug($"Bootstrap - FetchBuildVersion");

            var filePath = Path.Combine(StreamingAssetPath, "build.txt");
            if (filePath.Contains("://"))
            {
                using (var uwr = UnityWebRequest.Get(filePath))
                {
                    var uwrao = await uwr.SendWebRequest();
                    var text = uwrao.downloadHandler.text;
                    BuildVersion.Value = text;

                    Debug.Log($"Bootstrap - FetchBuildVersion - build version: {text}");
                }
            }
            else
            {
                var text = System.IO.File.ReadAllText(filePath);
                BuildVersion.Value = text;

                Debug.Log($"Bootstrap - FetchBuildVersion - build version: {text}");
            }
        }

        // Move this out of this partial file
        private async Task LoadStartData()
        {
            _logger.Debug($"Bootstrap - LoadStartData");

            //
            await FetchBuildVersion();

            await FetchFlowControlDataJsonString();
            _flowControlDataJsonUse = JsonUtility.FromJson<GameFlowControl.Template.FlowControlDataJsonUse>(FlowControlDataJsonString.Value);

            // //
            // RestoreLanguage();
        }
    }
}
