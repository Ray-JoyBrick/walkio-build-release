namespace JoyBrick.Walkio.Build
{
    using UnityEditor;

    public static class BuildConfigurationHelper
    {
        public static void StartBuild(string configurationPath)
        {
            // var path = $"Assets/_/AndroidClassicBuildConfiguration.buildconfiguration";
            // var path = $"Packages/com.walkio.build.local/Data Assets/AndroidClassicBuildConfiguration.buildconfiguration";
            
             var buildConfiguration = AssetDatabase.LoadAssetAtPath<Unity.Build.BuildConfiguration>(configurationPath);
            if (buildConfiguration != null)
            {
                var buildResult = buildConfiguration.CanBuild();
                if (buildResult.Result)
                {
                    buildConfiguration.Build();
                }
            }
        }
        
        [MenuItem("Assets/Walkio/Build/Build Android")]
        public static void StartBuild_Android()
        {
            var path = $"Packages/com.walkio.build.local/Data Assets/AndroidClassicBuildConfiguration.buildconfiguration";
            StartBuild(path);
        }

        
        [MenuItem("Assets/Walkio/Build/Build iOS")]
        public static void StartBuild_iOS()
        {
            var path = $"Packages/com.walkio.build.local/Data Assets/iOSClassicBuildConfiguration.buildconfiguration";
            StartBuild(path);
        }
    }
}
