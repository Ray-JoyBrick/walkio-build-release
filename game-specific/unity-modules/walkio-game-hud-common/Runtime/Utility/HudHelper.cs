namespace JoyBrick.Walkio.Game.Hud.Utility
{
    using UnityEngine;

    public static partial class HudHelper
    {
        public static void ExtractView(GameObject parent)
        {
            var collectAssistView = GameObject.FindObjectOfType<CollectAssistView>();
            if (collectAssistView == null) return;

            foreach (Transform v in parent.transform)
            {
                var movableView = v.GetComponent<MovableView>();
                if (movableView != null)
                {
                    collectAssistView.PlaceViewInContainer(movableView.gameObject, movableView.inDropdownName);
                }
            }
        }
    }
}
