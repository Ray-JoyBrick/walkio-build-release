namespace JoyBrick.Walkio.Game.Extension
{
    using UnityEngine;

    public interface IExtensionService
    {
        void SetReferenceToExtension(GameObject inGO);

        void SendExtensionEvent(string eventName);
    }
}
