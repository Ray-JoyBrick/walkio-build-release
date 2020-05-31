namespace JoyBrick.Walkio.Game
{
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
    using Unity.RemoteConfig;
#endif

    public partial class Bootstrap
    {
        public struct UserAttributes
        {
            
        }

        public struct AppAttributes
        {
            
        }
        
        private void SetupRemoteConfiguration()
        {
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
            
            ConfigManager.FetchCompleted += ApplyRemoteSettings;
            ConfigManager.FetchConfigs<UserAttributes, AppAttributes>(
                new UserAttributes(), new AppAttributes());

#endif
        }

#if COMPLETE_PROJECT || BEHAVIOR_PROJECT

        private void ApplyRemoteSettings(ConfigResponse configResponse)
        {
            switch (configResponse.requestOrigin)
            {
                case ConfigOrigin.Default:
                    break;
                case ConfigOrigin.Cached:
                    break;
                case ConfigOrigin.Remote:
                    SetSettings();
                    break;
            }
        }

        private void SetSettings()
        {
            // var v = ConfigManager.appConfig.GetInt("");
        }
        
#endif
    }
}
