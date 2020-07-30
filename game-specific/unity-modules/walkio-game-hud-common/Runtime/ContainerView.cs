namespace JoyBrick.Walkio.Game.Hud
{
    using Unity.Assertions;
    using UnityEngine;

    public class ContainerView : MonoBehaviour
    {
        public Canvas canvas;
        public CanvasGroup canvasGroup;

        void Start()
        {
            Assert.IsNotNull(canvas);
            Assert.IsNotNull(canvasGroup);
        }
        
        public void TurnOnOff(bool flag)
        {
            canvas.enabled = flag;

            canvasGroup.blocksRaycasts = flag;
            canvasGroup.interactable = flag;

            if (flag)
            {
                canvasGroup.alpha = 1;
            }
            else
            {
                canvasGroup.alpha = 0;
            }
        }
    }
}
