namespace JoyBrick.Walkio.Game.Extension.PlayMaker.Actions
{
#if PLAYMAKER
    using HutongGames.PlayMaker;
    using UnityEngine;
    
    using GameCommand = JoyBrick.Walkio.Game.Command;

    [ActionCategory("Walkio/Game Command Service")]
    public class GetCommandService : FsmStateAction
    {
        [RequiredField]
        // [Tooltip("The GameObject to check.")]
        public FsmOwnerDefault gameObject;
        
        // [Tooltip("Text to send to the log.")]
        public FsmString commandName;
        
        public override void Reset()
        {
            gameObject = null;
            commandName = null;
        }

        public override void OnEnter()
        {
            if (gameObject.GameObject.Value != null)
            {
                var commandService = gameObject.GameObject.Value.GetComponent<GameCommand.ICommandService>();
                if (commandService != null)
                {
                    commandService.SendCommand(commandName.Value);
                }
            }
            
            Finish();
        }        
    }
#endif
}
