namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
#if COMPLETE_PROJECT
    using Microsoft.AppCenter.Unity.Crashes;
#endif

    public partial class Bootstrap
    {
        private void SetupAppCenterCrashes()
        {
#if COMPLETE_PROJECT
            Crashes.ShouldProcessErrorReport = (ErrorReport report) =>
            {
                // Check the report in here and return true or false depending on the ErrorReport.
                return true;
            };
#endif
        }
    }
}
