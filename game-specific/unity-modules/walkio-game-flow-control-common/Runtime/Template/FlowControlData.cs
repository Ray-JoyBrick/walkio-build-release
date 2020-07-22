namespace JoyBrick.Walkio.Game.FlowControl.Template
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Flow Control Data", menuName = "Walkio/Game/Flow Control/Flow Control Data")]
    public class FlowControlData : ScriptableObject
    {
        public int doneLoadingAssetWaitForApp;
        public int doneSettingAssetWaitForApp;

        public int doneLoadingAssetWaitForPreparation;
        public int doneSettingAssetWaitForPreparation;

        public int doneLoadingAssetWaitForStage;
        public int doneSettingAssetWaitForStage;

    }
}
