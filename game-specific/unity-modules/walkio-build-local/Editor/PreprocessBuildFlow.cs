namespace JoyBrick.Walkio.Build
{
    using System.IO;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommonEditor = JoyBrick.Walkio.Game.Common.EditorPart;

    class PreprocessBuildFlow : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }
        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log($"PreprocessBuildFlow - OnPreprocessBuild - report: {report}");

#if COMPLETE_PROJECT

            // Setup app icon
            AppIconSetHelper.SetAndroidAdaptive();
            AppIconSetHelper.SetAndroidRound();
            AppIconSetHelper.SetAndroidLegacy();

#endif

            // Setup app settings
            GameCommonEditor.CountHelper.CountDoneLoadingAssetWaitAttribute();
        }
    }
}
