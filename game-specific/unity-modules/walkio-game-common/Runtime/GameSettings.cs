namespace JoyBrick.Walkio.Game.Common
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Game Settings", menuName = "Walkio/Game/Common/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        public int doneLoadingAssetWaitForApp;
        public int doneSettingAssetWaitForApp;

        public int doneLoadingAssetWaitForPreparation;
        public int doneSettingAssetWaitForPreparation;

        public int doneLoadingAssetWaitForStage;
        public int doneSettingAssetWaitForStage;

    }
}
