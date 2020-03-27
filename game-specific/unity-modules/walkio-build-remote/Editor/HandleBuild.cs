namespace JoyBrick.Walkio.Build
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Settings;

    using UnityEngine;

// #if UNITY_CLOUD_BUILD
//     using UnityEngine.CloudBuild;
//     using UnityEngine.CloudBuild.API;
// #endif

    public class HandleBuild
    {
#if UNITY_CLOUD_BUILD
        public static void HandleCommon(UnityEngine.CloudBuild.BuildManifestObject manifest)
        {
            // Will get version according to platform that is built. But, except using define,
            // is there better approach for this?
        }

        public static void HandleAndroidVersion(UnityEngine.CloudBuild.BuildManifestObject manifest)
        {
            var buildNumber = GetBuildNumber(manifest);
            var buildVersion = GetCompleteBuildVersion(0, buildNumber);
            WriteBuildVersionToFile(buildVersion);

            //
            AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent();

            // Modify player settings for the specific platform
            PlayerSettings.bundleVersion = buildVersion;
            // Should be less than 100000 for buildApkPerCpuArchitecture to generate valid version codes.
            PlayerSettings.Android.bundleVersionCode = int.Parse(buildNumber);
        }

        public static void HandleIOSVersion(UnityEngine.CloudBuild.BuildManifestObject manifest)
        {
            var buildNumber = GetBuildNumber(manifest);
            var buildVersion = GetCompleteBuildVersion(1, buildNumber);
            WriteBuildVersionToFile(buildVersion);

            //
            AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent();

            // Modify player settings for the specific platform
            PlayerSettings.bundleVersion = buildVersion;
            // It is saved to the 'CFBundleVersion' field in the built app's info.plist file.
            PlayerSettings.iOS.buildNumber = buildNumber;

            // Delete _Generated folder
            // FileUtil.DeleteFileOrDirectory($"{Application.dataPath}/_/_Generated");
        }

        public static string GetBuildNumber(UnityEngine.CloudBuild.BuildManifestObject manifest)
        {
            var buildNumber = "";
            var hasValue = manifest.TryGetValue<string>("buildNumber", out buildNumber);
            if (!hasValue)
            {
                // Set the default to string 0
                buildNumber = "0";
            }

            return buildNumber;
        }

        public static string GetCompleteBuildVersion(int platformIndex, string buildNumber)
        {
            var version = GetPlatformVersion(platformIndex);
            var parts = version.Split('.');
            var major = parts[0];
            var minor = parts[1];
            var patch = parts[2];

            var buildVersion = $"{major}.{minor}.{patch}.{buildNumber}";

            return buildVersion;
        }

        public static string GetPlatformVersion(int platformIndex)
        {
            var path = $"Assets/_/1 - Game/App Related/Prefabs/Main/AppwideSOI.asset";
            var appwideSOI = (JoyBrick.DailyTalk.Game.App.Entry.Main.AppwideSOI)AssetDatabase.LoadAssetAtPath(path, typeof(JoyBrick.DailyTalk.Game.App.Entry.Main.AppwideSOI));
            // Resources.Load<JoyBrick.DailyTalk.Game.App.Entry.Main.AppwideSOI>("AppwideSOI");
            var version = "";
            if (appwideSOI == null)
            {
                version = "0.0.0";
            }
            else
            {
                var pv = appwideSOI.settings.platformVersion;
                if (pv == null)
                {
                    version = "0.0.0.0";
                }
                else
                {
                    version = pv.versions[platformIndex];
                }
            }

            return version;
        }

        public static void WriteBuildVersionToFile(string buildVersion)
        {
            var filePath = Path.Combine(Application.streamingAssetsPath, "build.txt");

            using (var outfile = new StreamWriter(filePath))
            {
                outfile.Write(buildVersion);
            }
        }

        public static void HandleVersion(UnityEngine.CloudBuild.BuildManifestObject manifest)
        {
            var appwideSOI = Resources.Load<JoyBrick.DailyTalk.Game.App.Entry.Main.AppwideSOI>("AppwideSOI");
            var pv = appwideSOI.settings.platformVersion;
            var buildNumber = "";
            var hasValue = manifest.TryGetValue<string>("buildNumber", out buildNumber);
            if (!hasValue)
            {
                // Set the default to string 0
                buildNumber = "0";
            }

            // Adjust player settings
            // PlayerSettings.Android.bundleVersionCode = int.Parse(buildNumber);
            // PlayerSettings.iOS.buildNumber = buildNumber;

            // Write to build txt file so the ui can get this value
            var filePath = Path.Combine(Application.streamingAssetsPath, "build.txt");

            var buildDesc = "";
            var major = 0;
            var minor = 0;
            var patch = 0;
            buildDesc = $"{major}.{minor}.{patch}.{buildNumber}";

            using (var outfile = new StreamWriter(filePath))
            {
                outfile.Write(buildDesc);
            }
        }
#endif
    }
}
