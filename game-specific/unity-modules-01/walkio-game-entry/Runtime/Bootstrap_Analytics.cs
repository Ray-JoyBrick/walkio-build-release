namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
#if COMPLETE_PROJECT
    using Microsoft.AppCenter.Unity.Analytics;
#endif

    public partial class Bootstrap
    {
        private void SetupAppCenterAnalytics()
        {
#if COMPLETE_PROJECT
            // Analytics.TrackEvent(
            //     "Video clicked",
            //     new Dictionary<string, string> {
            //         { "Category", "Music" },
            //         { "FileName", "favorite.avi" }
            //     }
            // );
#endif
        }
    }
}
