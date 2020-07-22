namespace JoyBrick.Walkio.Game.Extension
{
    public static class BridgeExtension
    {
        public static void SendEvent(string eventName)
        {
            #if PLAYMAKER
            
            PlayMakerFSM.BroadcastEvent(eventName);
            
            #endif
        }
    }
}
