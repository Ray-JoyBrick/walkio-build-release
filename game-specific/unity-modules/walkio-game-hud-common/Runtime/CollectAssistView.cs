namespace JoyBrick.Walkio.Game.Hud
{
    using System;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.UI;

    public class CollectAssistView : MonoBehaviour
    {
        public Transform viewContainer;
        
        //
        public TMP_Dropdown viewSelectionDropdown;

        void Awake()
        {
            //
            viewSelectionDropdown.options.Clear();
        }
        
        void Start()
        {
            Assert.IsNotNull(viewContainer);
            Assert.IsNotNull(viewSelectionDropdown);
            
            //
            viewSelectionDropdown.onValueChanged.AddListener(HandleDropdownValueChanged);
        }

        private void HandleDropdownValueChanged(int v)
        {
            Debug.Log($"CollectAssistView - HandleDropdownValueChanged - v: {v}");
            
            var childCount = viewContainer.childCount;

            for (var i = 0; i < childCount; ++i)
            {
                var childView = viewContainer.GetChild(i);
                if (i != v)
                {
                    TurnOnOffView(childView.gameObject, false);
                }
                else
                {
                    TurnOnOffView(childView.gameObject, true);
                }
            }
        }

        private void TurnOnOffView(GameObject inGO, bool flag)
        {
            var canvas = inGO.GetComponent<Canvas>();
            var canvasGroup = inGO.GetComponent<CanvasGroup>();

            if (canvas != null && canvasGroup != null)
            {
                TurnOnOff(canvas, canvasGroup, flag);
            }
        }

        public void TurnOnOff(Canvas canvas, CanvasGroup canvasGroup, bool flag)
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
        
        //
        public void PlaceViewInContainer(GameObject inGO, string inDropdownName = "")
        {
            //
            inGO.transform.SetParent(viewContainer);
            var rectTransform = inGO.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;
                
                rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            
            //
            var count = viewSelectionDropdown.options.Count;
            var adjustedName = string.IsNullOrEmpty(inDropdownName) ? $"View - {(count + 1):000}" : inDropdownName;
            viewSelectionDropdown.options.Add(new TMP_Dropdown.OptionData(adjustedName));
            
            //
            if (!viewSelectionDropdown.options.Any())
            {
            }
            else if (viewSelectionDropdown.options.Count == 1)
            {
                TurnOnOffView(inGO, true);
            }
            else
            {
                TurnOnOffView(inGO, false);
            }
        }

        private void OnDestroy()
        {
            viewSelectionDropdown.onValueChanged.RemoveListener(HandleDropdownValueChanged);
        }
    }
}
