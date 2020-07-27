using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Opsive.UltimateCharacterController.Input
{

    public class NewUnityInput : PlayerInput
    {              
        /// <summary>
        /// Specifies if any input type should be forced.
        /// </summary>
        public enum ForceInputType { None, Standalone, Virtual }

        [Tooltip("Specifies if any input type should be forced.")]
        [SerializeField] protected ForceInputType m_ForceInput;
        [Tooltip("Should the cursor be disabled?")]
        [SerializeField] protected bool m_DisableCursor = true;
        [Tooltip("Should the cursor be enabled when the escape key is pressed?")]
        [SerializeField] protected bool m_EnableCursorWithEscape = true;
        [Tooltip("If the cursor is enabled with escape should the look vector be prevented from updating?")]
        [SerializeField] protected bool m_PreventLookVectorChanges = true;

        private UnityEngine.InputSystem.PlayerInput playerInput;

        public bool DisableCursor
        {
            get { return m_DisableCursor; }
            set
            {

                m_DisableCursor = value;
                if (m_DisableCursor && Cursor.visible)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else if (!m_DisableCursor && !Cursor.visible)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }
        public bool EnableCursorWithEscape { get { return m_EnableCursorWithEscape; } set { m_EnableCursorWithEscape = value; } }
        public bool PreventLookMovementWithEscape { get { return m_PreventLookVectorChanges; } set { m_PreventLookVectorChanges = value; } }


        /// <summary>
        /// Initialize the default values.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        }

        /// <summary>
        /// The component has been enabled.
        /// </summary>
        private void OnEnable()
        {
            if (m_DisableCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }


        /// <summary>
        /// Update the joystick and cursor state values.
        /// </summary>
        private void LateUpdate()
        {
            // Enable the cursor if the escape key is pressed. Disable the cursor if it is visbile but should be disabled upon press.
            if (m_EnableCursorWithEscape && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (m_PreventLookVectorChanges)
                {
                    OnApplicationFocus(false);
                }
            }
            else if (Cursor.visible && m_DisableCursor && !IsPointerOverUI() && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (m_PreventLookVectorChanges)
                {
                    OnApplicationFocus(true);
                }
            }
#if UNITY_EDITOR
            // The cursor should be visible when the game is paused.
            if (!Cursor.visible && Time.deltaTime == 0)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
#endif

        }


        /// <summary>
        /// Internal method which returns true if the button is being pressed.
        /// </summary>
        /// <param name="name">The name of the button.</param>
        /// <returns>True of the button is being pressed.</returns>
        protected override bool GetButtonInternal(string name)
        {
            var btn = playerInput.currentActionMap.FindAction(name);
            if (btn != null)
            {
                if (btn.activeControl is ButtonControl button && button.isPressed)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Internal method which returns true if the button was pressed this frame.
        /// </summary>
        /// <param name="name">The name of the button.</param>
        /// <returns>True if the button is pressed this frame.</returns>
        protected override bool GetButtonDownInternal(string name)
        {
            var btn = playerInput.currentActionMap.FindAction(name);
            if (btn != null)
            {
                if (btn.activeControl is ButtonControl button && button.wasPressedThisFrame)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Internal method which returnstrue if the button is up.
        /// </summary>
        /// <param name="name">The name of the button.</param>
        /// <returns>True if the button is up.</returns>
        protected override bool GetButtonUpInternal(string name)
        {
            var btn = playerInput.currentActionMap.FindAction(name);
            if (btn != null)
            {
                if (btn.activeControl is ButtonControl button && button.wasReleasedThisFrame)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Internal method which returns the value of the axis with the specified name.
        /// </summary>
        /// <param name="name">The name of the axis.</param>
        /// <returns>The value of the axis.</returns>
        protected override float GetAxisInternal(string name)
        {
            var btn = playerInput.currentActionMap.FindAction(name);
            if (btn != null)
            {
                return btn.ReadValue<float>();
            }
            return 0.0f;
        }

        /// <summary>
        /// Internal method which returns the value of the raw axis with the specified name.
        /// </summary>
        /// <param name="name">The name of the axis.</param>
        /// <returns>The value of the raw axis.</returns>
        protected override float GetAxisRawInternal(string name)
        {
            var btn = playerInput.currentActionMap.FindAction(name);

            if (btn != null)
            {
                return btn.ReadValue<float>();
                
            }
            return 0.0f;
        }

        /// <summary>
        /// Returns true if the pointer is over a UI element.
        /// </summary>
        /// <returns>True if the pointer is over a UI element.</returns>
        public override bool IsPointerOverUI()
        {

            return base.IsPointerOverUI();
        }

        /// <summary>
        /// Enables or disables gameplay input. An example of when it will not be enabled is when there is a fullscreen UI over the main camera.
        /// </summary>
        /// <param name="enable">True if the input is enabled.</param>
        protected override void EnableGameplayInput(bool enable)
        {
            base.EnableGameplayInput(enable);

            if (enable && m_DisableCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        /// <summary>
        /// Does the game have focus?
        /// </summary>
        /// <param name="hasFocus">True if the game has focus.</param>
        protected override void OnApplicationFocus(bool hasFocus)
        {
            base.OnApplicationFocus(hasFocus);

            if (enabled && hasFocus && m_DisableCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}

