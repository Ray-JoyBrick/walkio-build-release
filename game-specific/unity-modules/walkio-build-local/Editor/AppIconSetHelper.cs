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
            SetIcons(BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Adaptive);
        }

        [MenuItem("Tools/Walkio/Build/Set App Icon/Android/Round")]
        public static void SetAndroidRound()
        {
            SetIcons(BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Round);
        }

        [MenuItem("Tools/Walkio/Build/Set App Icon/Android/Legacy")]
        public static void SetAndroidLegacy()
        {
            SetIcons(BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Legacy);
        }

        [MenuItem("Tools/Walkio/Build/Set App Icon/iOS/Application")]
        public static void SetiOSApplication()
        {
            SetIcons(BuildTargetGroup.iOS, UnityEditor.iOS.iOSPlatformIconKind.Application);
        }

        [MenuItem("Tools/Walkio/Build/Set App Icon/iOS/Spotlight")]
        public static void SetiOSSpotlight()
        {
            SetIcons(BuildTargetGroup.iOS, UnityEditor.iOS.iOSPlatformIconKind.Spotlight);
        }

        [MenuItem("Tools/Walkio/Build/Set App Icon/iOS/Settings")]
        public static void SetiOSSettings()
        {
            SetIcons(BuildTargetGroup.iOS, UnityEditor.iOS.iOSPlatformIconKind.Settings);
        }

        [MenuItem("Tools/Walkio/Build/Set App Icon/iOS/Notifications")]
        public static void SetiOSNotifications()
        {
            SetIcons(BuildTargetGroup.iOS, UnityEditor.iOS.iOSPlatformIconKind.Notification);
        }

        [MenuItem("Tools/Walkio/Build/Set App Icon/iOS/Marketing")]
        public static void SetiOSMarketing()
        {
            SetIcons(BuildTargetGroup.iOS, UnityEditor.iOS.iOSPlatformIconKind.Marketing);
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
