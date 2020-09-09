namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Linq;
#if COMPLETE_PROJECT
    using Microsoft.AppCenter.Unity.Distribute;
#endif
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

#if WALKIO_HUD_APP_ASSIST

    using GameHudAppAssist = JoyBrick.Walkio.Game.Hud.App.Assist;
    // using GameHudPreparation = JoyBrick.Walkio.Game.Hud.Preparation;
#endif

#if WALKIO_HUD_STAGE_ASSIST
    using GameHudStageAssist = JoyBrick.Walkio.Game.Hud.Stage.Assist;
#endif

    public partial class Bootstrap
    {
        private void SetupAppCenterDistribute()
        {
#if COMPLETE_PROJECT

            var observable =
                Observable
                    .FromEvent<ReleaseAvailableCallback, ReleaseDetails>(
                        h => CheckValidation,
                        h => Distribute.ReleaseAvailable += h,
                        h => { });
            observable
                .Subscribe(x =>
                {
                    //
                })
                .AddTo(_compositeDisposable);

#endif
        }

#if COMPLETE_PROJECT

        UpdateAction GetUserUpdateAction()
        {
            return UpdateAction.Postpone;
        }

        bool CheckValidation(ReleaseDetails releaseDetails)
        {
            // Look at releaseDetails public properties to get version information, release notes text or release notes URL
            var versionName = releaseDetails.ShortVersion;
            var versionCodeOrBuildNumber = releaseDetails.Version;
            var releaseNotes = releaseDetails.ReleaseNotes;
            var releaseNotesUrl = releaseDetails.ReleaseNotesUrl;

            // (Do something with the values if you want)

            // On mandatory update, user cannot postpone
            if (releaseDetails.MandatoryUpdate)
            {
                // Force user to update (you should probably show some custom UI here)
                Distribute.NotifyUpdateAction(UpdateAction.Update);
            }
            else
            {
                // Allow user to update or postpone (you should probably show some custom UI here)
                // "GetUserUpdateAction()" is not part of the SDK; it just represents a way of getting user response.
                // This blocks the thread while awaiting the user's response! This example should not be used literally
                UpdateAction updateAction = GetUserUpdateAction();
                Distribute.NotifyUpdateAction(updateAction);
            }
            // Return true if you are using your own UI to get user response, false otherwise
            return true;
        }

#endif
    }
}
