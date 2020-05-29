namespace JoyBrick.Walkio.Game.Extension.PlayMaker.Actions
{
#if PLAYMAKER
    using HutongGames.PlayMaker;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    [ActionCategory("Walkio/Game Flow Control")]
    public class GetFlowControl : FsmStateAction
    {
        [RequiredField]
        // [Tooltip("The GameObject to check.")]
        public FsmOwnerDefault gameObject;

        public FsmString flowAction;
        
        // [Tooltip("Text to send to the log.")]
        public FsmString flowName;
        
        public override void Reset()
        {
            gameObject = null;
            flowName = null;
        }

        public override void OnEnter()
        {
            if (gameObject.GameObject.Value != null)
            {
                var flowControl = gameObject.GameObject.Value.GetComponent<GameCommon.IFlowControl>();
                if (flowControl != null)
                {
                    if (string.CompareOrdinal(flowAction.Value, "Loading") == 0)
                    {
                        flowControl.StartLoadingAsset(flowName.Value);
                    }
                    else if (string.CompareOrdinal(flowAction.Value, "Cleaning") == 0)
                    {
                        flowControl.StartCleaningAsset(flowName.Value);
                    }
                }
            
                Finish();
            }
        }        
    }
#endif
}
