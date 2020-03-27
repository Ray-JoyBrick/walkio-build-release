// GENERATED AUTOMATICALLY FROM 'Assets/_/Data Assets/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace JoyBrick.Walkio.Game.Input.Generated
{
    public class @Controls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @Controls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Default Mapping"",
            ""id"": ""a0f4bc38-2ae1-4a6f-8b92-405ef668f7ee"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""19b4c5d4-8424-4289-a80f-6f0870898008"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""AWSD"",
                    ""id"": ""0ca7c60f-d084-46a7-8c31-0ca3080a5878"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1dc0d1d4-92ef-4883-9361-0171827e5f92"",
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
                    ""id"": ""5c5a9bd8-2542-48b9-b869-cb54039e48de"",
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
                    ""id"": ""107c00bc-a78c-4891-858d-9dc7883e6581"",
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
                    ""id"": ""0b0d4f7b-9d8f-444f-b0cc-5eeb30281c68"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Default Mapping
            m_DefaultMapping = asset.FindActionMap("Default Mapping", throwIfNotFound: true);
            m_DefaultMapping_Move = m_DefaultMapping.FindAction("Move", throwIfNotFound: true);
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

        // Default Mapping
        private readonly InputActionMap m_DefaultMapping;
        private IDefaultMappingActions m_DefaultMappingActionsCallbackInterface;
        private readonly InputAction m_DefaultMapping_Move;
        public struct DefaultMappingActions
        {
            private @Controls m_Wrapper;
            public DefaultMappingActions(@Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_DefaultMapping_Move;
            public InputActionMap Get() { return m_Wrapper.m_DefaultMapping; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(DefaultMappingActions set) { return set.Get(); }
            public void SetCallbacks(IDefaultMappingActions instance)
            {
                if (m_Wrapper.m_DefaultMappingActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_DefaultMappingActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_DefaultMappingActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_DefaultMappingActionsCallbackInterface.OnMove;
                }
                m_Wrapper.m_DefaultMappingActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                }
            }
        }
        public DefaultMappingActions @DefaultMapping => new DefaultMappingActions(this);
        public interface IDefaultMappingActions
        {
            void OnMove(InputAction.CallbackContext context);
        }
    }
}
