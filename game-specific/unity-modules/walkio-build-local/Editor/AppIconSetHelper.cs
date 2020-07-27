namespace JoyBrick.Walkio.Build
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public class AppIconSetHelper
    {
        private static readonly string IconStartPath = Path.Combine("Assets", "_", "1 - Game", "Preprocess Assets",
            "design-use", "visual-assets", "App Icons");

        [MenuItem("Tools/Walkio/Build/Set App Icon/Android/Adaptive")]
        public static void SetAndroidAdaptive()
        {
#if UNITY_ANDROID
            SetIcons(BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Adaptive);
#endif
        }

        [MenuItem("Tools/Walkio/Build/Set App Icon/Android/Round")]
        public static void SetAndroidRound()
        {
#if UNITY_ANDROID
            SetIcons(BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Round);
#endif
        }

        [MenuItem("Tools/Walkio/Build/Set App Icon/Android/Legacy")]
        public static void SetAndroidLegacy()
        {
#if UNITY_ANDROID
            SetIcons(BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Legacy);
#endif
        }

        [MenuItem("Tools/Walkio/Build/Set App Icon/iOS/Application")]
        public static void SetiOSApplication()
        {
#if UNITY_IOS
            SetIcons(BuildTargetGroup.iOS, UnityEditor.iOS.iOSPlatformIconKind.Application);
#endif
        }

        [MenuItem("Tools/Walkio/Build/Set App Icon/iOS/Spotlight")]
        public static void SetiOSSpotlight()
        {
#if UNITY_IOS
            SetIcons(BuildTargetGroup.iOS, UnityEditor.iOS.iOSPlatformIconKind.Spotlight);
#endif
        }

        [MenuItem("Tools/Walkio/Build/Set App Icon/iOS/Settings")]
        public static void SetiOSSettings()
        {
#if UNITY_IOS
            SetIcons(BuildTargetGroup.iOS, UnityEditor.iOS.iOSPlatformIconKind.Settings);
#endif
        }

        [MenuItem("Tools/Walkio/Build/Set App Icon/iOS/Notifications")]
        public static void SetiOSNotifications()
        {
#if UNITY_IOS
            SetIcons(BuildTargetGroup.iOS, UnityEditor.iOS.iOSPlatformIconKind.Notification);
#endif
        }

        [MenuItem("Tools/Walkio/Build/Set App Icon/iOS/Marketing")]
        public static void SetiOSMarketing()
        {
#if UNITY_IOS
            SetIcons(BuildTargetGroup.iOS, UnityEditor.iOS.iOSPlatformIconKind.Marketing);
#endif
        }

        private static Texture2D[] GetIconsFromAsset(
            BuildTargetGroup target,
            PlatformIconKind kind,
            PlatformIcon[] icons)
        {
            var iconCount = icons.Length;
            if (kind == UnityEditor.Android.AndroidPlatformIconKind.Adaptive)
            {
                iconCount = iconCount * 2;
            }

            var texArray = new Texture2D[iconCount];

            // Special handling for Android platform
            var folder = kind.ToString().Split(' ')[0];

            if (kind == UnityEditor.Android.AndroidPlatformIconKind.Adaptive)
            {
                for (var i = 0; i < icons.Length; ++i)
                {
                    // Should be square, so taking one side into consideration only
                    var iconSize = icons[i].width;
                    var fileName1 = Path.Combine(IconStartPath, target.ToString(), folder.ToString(), $"background-{iconSize}.png");
                    if (!File.Exists(fileName1))
                    {
                        Debug.LogError($"Texture does not exists at path: {fileName1}");
                        continue;
                    }

                    var fileName2 = Path.Combine(IconStartPath, target.ToString(), folder.ToString(), $"foreground-{iconSize}.png");
                    if (!File.Exists(fileName2))
                    {
                        Debug.LogError($"Texture does not exists at path: {fileName2}");
                        continue;
                    }

                    var backgroundTex2D = AssetDatabase.LoadAssetAtPath<Texture2D>(fileName1);
                    var foregroundTex2D = AssetDatabase.LoadAssetAtPath<Texture2D>(fileName2);
                    texArray[i * 2 + 0] = backgroundTex2D;
                    texArray[i * 2 + 1] = foregroundTex2D;
                }
            }
            else
            {
                for (var i = 0; i < texArray.Length; ++i)
                {
                    // Should be square, so taking one side into consideration only
                    var iconSize = icons[i].width;
                    var fileName = Path.Combine(IconStartPath, target.ToString(), folder.ToString(), $"{iconSize}.png");
                    if (!File.Exists(fileName))
                    {
                        Debug.LogError($"Texture does not exists at path: {fileName}");
                        continue;
                    }

                    var tex2D = AssetDatabase.LoadAssetAtPath<Texture2D>(fileName);
                    texArray[i] = tex2D;
                }
            }

            return texArray;
        }

        private static void SetIcons(BuildTargetGroup platform, PlatformIconKind kind)
        {
            var icons = PlayerSettings.GetPlatformIcons(platform, kind);
            var iconTextures = GetIconsFromAsset(platform, kind, icons);

            if (kind == UnityEditor.Android.AndroidPlatformIconKind.Adaptive)
            {
                for (int i = 0, length = icons.Length; i < length; ++i)
                {
                    icons[i].SetTexture(iconTextures[i * 2 + 0], 0);
                    icons[i].SetTexture(iconTextures[i * 2 + 1], 1);
                }
            }
            else
            {
                for (int i = 0, length = icons.Length; i < length; ++i)
                {
                    icons[i].SetTexture(iconTextures[i]);
                }
            }

            PlayerSettings.SetPlatformIcons(platform, kind, icons);

            AssetDatabase.SaveAssets();
            Debug.Log($"Set {platform}/{kind} Icon Complete");
        }
    }
}
