// GENERATED AUTOMATICALLY FROM 'Assets/_/1 - Game/App Related/Module - Input Control/app-input-control-service/Data Assets/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace JoyBrick.Walkio.Game.App.InputControlService.Generated
{
    public class @Controls : IInputActionCollection, IDisposable
    {
        private InputActionAsset asset;
        public @Controls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player Control"",
            ""id"": ""76513c28-8de3-4459-a659-6057e30dc41f"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""5d50114e-baf3-4fb3-afc8-9b9cdf5c09d2"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause Hud"",
                    ""type"": ""Button"",
                    ""id"": ""7b6a98da-ad0a-44c1-889b-56322f8f0a43"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Button"",
                    ""id"": ""9019bc59-515f-42de-9c01-793ae5da92f8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""d9418326-dc12-4616-a8a9-b7e5d7052357"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""8bf133de-c3ba-468a-bf93-d5d19132b1a2"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3d00c274-8353-476f-bd47-e41bd86edeb1"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""596d2807-a156-48df-8c10-dd361c72068d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""1e32ef4e-ad5c-426a-b52a-03ac03be47b0"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""1f12a76b-4b92-4732-b237-fe3147e6f32e"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause Hud"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3070c39e-2f79-412f-9016-02bd7516e177"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Player Control
            m_PlayerControl = asset.FindActionMap("Player Control", throwIfNotFound: true);
            m_PlayerControl_Move = m_PlayerControl.FindAction("Move", throwIfNotFound: true);
            m_PlayerControl_PauseHud = m_PlayerControl.FindAction("Pause Hud", throwIfNotFound: true);
            m_PlayerControl_Fire = m_PlayerControl.FindAction("Fire", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Player Control
        private readonly InputActionMap m_PlayerControl;
        private IPlayerControlActions m_PlayerControlActionsCallbackInterface;
        private readonly InputAction m_PlayerControl_Move;
        private readonly InputAction m_PlayerControl_PauseHud;
        private readonly InputAction m_PlayerControl_Fire;
        public struct PlayerControlActions
        {
            private @Controls m_Wrapper;
            public PlayerControlActions(@Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_PlayerControl_Move;
            public InputAction @PauseHud => m_Wrapper.m_PlayerControl_PauseHud;
            public InputAction @Fire => m_Wrapper.m_PlayerControl_Fire;
            public InputActionMap Get() { return m_Wrapper.m_PlayerControl; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerControlActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerControlActions instance)
            {
                if (m_Wrapper.m_PlayerControlActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_PlayerControlActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_PlayerControlActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_PlayerControlActionsCallbackInterface.OnMove;
                    @PauseHud.started -= m_Wrapper.m_PlayerControlActionsCallbackInterface.OnPauseHud;
                    @PauseHud.performed -= m_Wrapper.m_PlayerControlActionsCallbackInterface.OnPauseHud;
                    @PauseHud.canceled -= m_Wrapper.m_PlayerControlActionsCallbackInterface.OnPauseHud;
                    @Fire.started -= m_Wrapper.m_PlayerControlActionsCallbackInterface.OnFire;
                    @Fire.performed -= m_Wrapper.m_PlayerControlActionsCallbackInterface.OnFire;
                    @Fire.canceled -= m_Wrapper.m_PlayerControlActionsCallbackInterface.OnFire;
                }
                m_Wrapper.m_PlayerControlActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @PauseHud.started += instance.OnPauseHud;
                    @PauseHud.performed += instance.OnPauseHud;
                    @PauseHud.canceled += instance.OnPauseHud;
                    @Fire.started += instance.OnFire;
                    @Fire.performed += instance.OnFire;
                    @Fire.canceled += instance.OnFire;
                }
            }
        }
        public PlayerControlActions @PlayerControl => new PlayerControlActions(this);
        public interface IPlayerControlActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnPauseHud(InputAction.CallbackContext context);
            void OnFire(InputAction.CallbackContext context);
        }
    }
}
