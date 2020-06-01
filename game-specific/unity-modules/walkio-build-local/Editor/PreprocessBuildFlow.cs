namespace JoyBrick.Walkio.Build
{
    using System.IO;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    class PreprocessBuildFlow : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }
        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log($"PreprocessBuildFlow - OnPreprocessBuild - report: {report}");
            AppIconSetHelper.SetAndroidAdaptive();
            AppIconSetHelper.SetAndroidRound();
            AppIconSetHelper.SetAndroidLegacy();
        }
    }
}
