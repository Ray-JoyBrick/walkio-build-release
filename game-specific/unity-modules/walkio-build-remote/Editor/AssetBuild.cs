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

    public class AssetBuild
    {
#if UNITY_CLOUD_BUILD
        public static void HandleCommon(UnityEngine.CloudBuild.BuildManifestObject manifest)
        {
            // Will get version according to platform that is built. But, except using define,
            // is there better approach for this?
        }

        public static void HandleAndroidVersion(UnityEngine.CloudBuild.BuildManifestObject manifest)
        {
            // Add something to prepare build for android
        }

        public static void HandleIOSVersion(UnityEngine.CloudBuild.BuildManifestObject manifest)
        {
        }

#endif
    }
}
