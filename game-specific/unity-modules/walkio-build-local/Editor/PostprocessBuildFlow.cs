namespace JoyBrick.Walkio.Build
{
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    class PostprocessBuildFlow : IPostprocessBuildWithReport
    {
        public int callbackOrder { get; }
        public void OnPostprocessBuild(BuildReport report)
        {
            Debug.Log($"PostprocessBuildFlow - OnPostprocessBuild - report: {report}");
        }
    }
}
