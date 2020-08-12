namespace JoyBrick.Walkio.Game.FlowControl.Template
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Flow Control Data", menuName = "Walkio/Game/Flow Control/Flow Control Data")]
    public class FlowControlData : ScriptableObject
    {
        public int doneLoadingAssetWaitForApp;
        public int doneLoadingAssetWaitForAppAssist;
        
        public int doneSettingAssetWaitForApp;
        public int doneSettingAssetWaitForAppAssist;
        
        public int doneLoadingAssetWaitForPreparation;
        public int doneLoadingAssetWaitForPreparationAssist;
        
        public int doneSettingAssetWaitForPreparation;
        public int doneSettingAssetWaitForPreparationAssist;

        public int doneLoadingAssetWaitForStage;
        public int doneLoadingAssetWaitForStageAssist;
        
        public int doneSettingAssetWaitForStage;
        public int doneSettingAssetWaitForStageAssist;

    }
}
